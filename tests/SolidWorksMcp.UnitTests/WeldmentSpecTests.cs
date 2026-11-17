using SolidWorksMcp.Core;
using SolidWorksMcp.Core.Model;
using SolidWorksMcp.Core.Services;
using Xunit;

namespace SolidWorksMcp.UnitTests;

/// <summary>
/// weldment_member's names (WeldmentSpec): profile sizes and types matched loosely, corner treatments; weldment_finish's
/// kinds, codes and the checks each kind makes before anything changes.
/// </summary>
public class WeldmentSpecTests
{
    [Fact]
    public void Sizes_match_whatever_the_spacing_and_case()
    {
        string[] sizes = ["20 x 20 x 2", "40 x 40 x 4", "50 X 30 X 2.6"];
        Assert.Equal("40 x 40 x 4", WeldmentSpec.MatchSize(sizes, "40x40x4"));
        Assert.Equal("40 x 40 x 4", WeldmentSpec.MatchSize(sizes, " 40 X 40 x 4 "));
        Assert.Equal("50 X 30 X 2.6", WeldmentSpec.MatchSize(sizes, "50×30×2.6"));
        Assert.Null(WeldmentSpec.MatchSize(sizes, "40 x 40 x 5"));
    }

    [Fact]
    public void Names_match_without_case_spaces_or_separators()
    {
        string[] types = ["square tube", "rectangular tube", "angle iron"];
        Assert.Equal("square tube", WeldmentSpec.MatchName(types, "Square_Tube"));
        Assert.Equal("angle iron", WeldmentSpec.MatchName(types, "angle-iron"));
        Assert.Null(WeldmentSpec.MatchName(types, "tube"));
    }

    // A standard folder: "square tube" with its sizes as configurations, "flat bar" as one file per size in its folder,
    // "solo" a one-configuration file directly in the standard folder (SOLIDWORKS ignores it there), "empty" a folder of none.
    private static readonly string[] StdFiles = [@"p\custom\square tube.sldlfp", @"p\custom\solo.sldlfp"];
    private static readonly string[] StdFolders = [@"p\custom\flat bar", @"p\custom\empty"];

    private static IReadOnlyList<string> FilesIn(string dir) => Path.GetFileName(dir) switch
    {
        "flat bar" => [@"p\custom\flat bar\40 x 5.sldlfp", @"p\custom\flat bar\50 X 8.sldlfp"],
        "solo" => [@"p\custom\solo\40 x 5.sldlfp"],
        _ => [],
    };

    private static IReadOnlyList<string> Configs(string file) => Path.GetFileNameWithoutExtension(file) switch
    {
        "square tube" => ["20 x 20 x 2", "40 x 40 x 4"],
        _ => ["Default"],
    };

    private static ProfilePick Pick(string type, string size, string[]? files = null, string[]? folders = null) =>
        WeldmentSpec.PickProfile("custom", files ?? StdFiles, folders ?? StdFolders, FilesIn, Configs, type, size);

    [Fact]
    public void A_profile_is_a_file_of_sizes_or_a_folder_of_one_file_per_size()
    {
        var file = Pick("Square_Tube", "40x40x4");
        Assert.Equal(new ProfilePick(@"p\custom\square tube.sldlfp", "square tube", "40 x 40 x 4", "40 x 40 x 4"), file);
        var folder = Pick("flat-bar", "50x8");
        Assert.Equal(new ProfilePick(@"p\custom\flat bar\50 X 8.sldlfp", "flat bar", "50 X 8", ""), folder); // a size file: no configuration named
        // A one-configuration file and a folder of the same type: the folder is what SOLIDWORKS lists.
        var both = Pick("solo", "40 x 5", folders: [@"p\custom\solo"]);
        Assert.Equal(@"p\custom\solo\40 x 5.sldlfp", both.Path);
    }

    [Fact]
    public void A_one_size_file_in_the_standard_folder_is_refused_with_the_layout_that_works()
    {
        var e = Assert.Throws<SwException>(() => Pick("solo", "Default"));
        Assert.Equal(ErrorCodes.InvalidArgument, e.Code);
        Assert.Contains("one configuration", e.Message);
        Assert.Contains(@"custom\solo\<size>.sldlfp", e.Hint);
    }

    [Fact]
    public void Unknown_types_and_sizes_list_both_layouts()
    {
        var type = Assert.Throws<SwException>(() => Pick("hexagon bar", "10"));
        Assert.Equal("Profiles: empty, flat bar, solo, square tube.", type.Hint);
        Assert.Contains("40 x 5, 50 X 8", Assert.Throws<SwException>(() => Pick("flat bar", "60 x 10")).Hint);
        Assert.Contains("20 x 20 x 2, 40 x 40 x 4", Assert.Throws<SwException>(() => Pick("square tube", "60 x 60 x 5")).Hint);
        Assert.Contains("holds no .sldlfp", Assert.Throws<SwException>(() => Pick("empty", "1")).Message);
    }

    [Fact]
    public void Corners_map_to_the_end_condition_codes()
    {
        Assert.Equal((1, "miter"), WeldmentSpec.Corner(null));
        Assert.Equal((1, "miter"), WeldmentSpec.Corner("Mitre"));
        Assert.Equal((2, "butt1"), WeldmentSpec.Corner("butt"));
        Assert.Equal((3, "butt2"), WeldmentSpec.Corner("butt-2"));
        Assert.Equal((0, "none"), WeldmentSpec.Corner("none"));
        Assert.Equal(ErrorCodes.InvalidArgument, Assert.Throws<SwException>(() => WeldmentSpec.Corner("round")).Code);
    }

    [Fact]
    public void Few_lists_a_few_names_and_says_how_many_in_all()
    {
        Assert.Equal("a, b", WeldmentSpec.Few(["a", "b"]));
        Assert.Equal("1, 2, … (5 in all)", WeldmentSpec.Few(["1", "2", "3", "4", "5"], 2));
    }

    [Fact]
    public void Finish_kinds_corners_locations_and_sides_map_to_their_codes()
    {
        Assert.Equal("end_cap", WeldmentSpec.FinishKind("End-Cap"));
        Assert.Equal("bead", WeldmentSpec.FinishKind("fillet_bead"));
        Assert.Equal("trim", WeldmentSpec.FinishKind("extend"));
        Assert.Equal(ErrorCodes.InvalidArgument, Assert.Throws<SwException>(() => WeldmentSpec.FinishKind(null)).Code);
        Assert.Equal(ErrorCodes.InvalidArgument, Assert.Throws<SwException>(() => WeldmentSpec.FinishKind("rib")).Code);
        Assert.Equal((4, "trim"), WeldmentSpec.TrimCorner(null)); // swEndConditionTrim
        Assert.Equal((1, "miter"), WeldmentSpec.TrimCorner("mitre"));
        Assert.Equal((3, "butt2"), WeldmentSpec.TrimCorner("butt_2"));
        Assert.Equal(((short)1, "center"), WeldmentSpec.GussetLocation(null)); // swGussetProfileLocationCenter
        Assert.Equal(((short)2, "end"), WeldmentSpec.GussetLocation("End"));
        Assert.Equal(((short)1, "both"), WeldmentSpec.GussetSide(null)); // swGussetThicknessBothSides
        Assert.Equal(((short)0, "inner"), WeldmentSpec.GussetSide("inside"));
        Assert.Equal((1, "selection"), WeldmentSpec.BeadSide(null)); // swCosmeticWeldBeadSide_selection
        Assert.Equal((3, "all_around"), WeldmentSpec.BeadSide("all-around"));
        Assert.Equal((0, "outward"), WeldmentSpec.CapDirection(null)); // InsertEndCapFeature3's own codes, not the enum's
        Assert.Equal((2, "internal"), WeldmentSpec.CapDirection("internal"));
    }

    private static readonly EntityRef[] One = [new EntityRef { Name = "Structural Member1[1]" }];
    private static readonly EntityRef[] Two = [new EntityRef { At = [0, 20, 0] }, new EntityRef { At = [20, 100, 0] }];

    private static string Refused(string kind, WeldmentFinishRequest q) =>
        Assert.Throws<SwException>(() => WeldmentSpec.CheckFinish(kind, q)).Message;

    [Fact]
    public void Each_finish_kind_refuses_what_it_cant_use_before_anything_changes()
    {
        Assert.Contains("'boundary'", Refused("trim", new WeldmentFinishRequest { Bodies = One }));
        Assert.Contains("gapMm", Refused("trim", new WeldmentFinishRequest { Bodies = One, Boundary = One, GapMm = -1 }));
        Assert.Contains("thicknessMm", Refused("end_cap", new WeldmentFinishRequest { Faces = One }));
        Assert.Contains("exactly two faces", Refused("gusset", new WeldmentFinishRequest { Faces = One, ThicknessMm = 5, LegsMm = [50, 50] }));
        Assert.Contains("legsMm", Refused("gusset", new WeldmentFinishRequest { Faces = Two, ThicknessMm = 5, LegsMm = [50] }));
        Assert.Contains("insetMm is for direction internal", Refused("end_cap", new WeldmentFinishRequest { Faces = One, ThicknessMm = 5, InsetMm = 2 }));
        Assert.Contains("without tangent propagation", Refused("bead", new WeldmentFinishRequest { Faces = One, Faces2 = One, SizeMm = 4, Tangent = true, Side = "both" }));
        Assert.Contains("faces2", Refused("bead", new WeldmentFinishRequest { Faces = One, SizeMm = 4 }));
        Assert.Contains("both lengthMm and pitchMm", Refused("bead", new WeldmentFinishRequest { Faces = One, Faces2 = One, SizeMm = 4, LengthMm = 20 }));
        Assert.Contains("longer than lengthMm", Refused("bead", new WeldmentFinishRequest { Faces = One, Faces2 = One, SizeMm = 4, LengthMm = 20, PitchMm = 20 }));
    }

    [Fact]
    public void A_finish_reports_the_parameters_its_kind_ignores()
    {
        Assert.Empty(WeldmentSpec.CheckFinish("trim", new WeldmentFinishRequest { Bodies = One, Boundary = One, GapMm = 2, Corner = "miter" }));
        Assert.Equal(["legsMm", "sizeMm"], WeldmentSpec.CheckFinish("end_cap",
            new WeldmentFinishRequest { Faces = One, ThicknessMm = 5, SizeMm = 4, LegsMm = [50, 50] }));
        Assert.Equal(["thicknessMm"], WeldmentSpec.CheckFinish("bead",
            new WeldmentFinishRequest { Faces = One, Faces2 = One, SizeMm = 4, ThicknessMm = 5 }));
    }
}
