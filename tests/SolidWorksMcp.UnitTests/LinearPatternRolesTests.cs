using SolidWorksMcp.Core.Model;
using Xunit;

namespace SolidWorksMcp.UnitTests;

/// <summary>S146: which count and spacing of a linear pattern belongs to which direction.</summary>
public class LinearPatternRolesTests
{
    private static DimensionInfo D(string n, double v, string unit) => new() { Name = n + "@LPattern1", Value = v, Unit = unit };

    private static Dictionary<string, string?> Roles(IEnumerable<DimensionInfo> dims) => dims.ToDictionary(x => x.Name[..2], x => x.Role);

    [Fact]
    public void Two_directions_take_their_roles_from_the_name_order()
    {
        // The order SOLIDWORKS lists them in: D4, D2, D3, D1.
        var r = LinearPatternRoles.Apply([D("D4", 20, "mm"), D("D2", 3, "count"), D("D3", 30, "mm"), D("D1", 2, "count")], 2, 30, 3, 20);
        Assert.Equal(new() { ["D4"] = "spacing2", ["D2"] = "count2", ["D3"] = "spacing1", ["D1"] = "count1" }, Roles(r));
    }

    [Fact]
    public void Equal_values_are_told_apart_by_name()
    {
        var r = LinearPatternRoles.Apply([D("D1", 2, "count"), D("D2", 2, "count"), D("D3", 20, "mm"), D("D4", 20, "mm")], 2, 20, 2, 20);
        Assert.Equal(new() { ["D1"] = "count1", ["D2"] = "count2", ["D3"] = "spacing1", ["D4"] = "spacing2" }, Roles(r));
    }

    [Fact]
    public void One_direction_has_a_count_and_a_spacing()
    {
        var r = LinearPatternRoles.Apply([D("D3", 45, "mm"), D("D1", 2, "count")], 2, 45, null, null);
        Assert.Equal(new() { ["D3"] = "spacing1", ["D1"] = "count1" }, Roles(r));
    }

    [Fact]
    public void The_other_order_is_taken_when_only_it_matches_the_definition()
    {
        var r = LinearPatternRoles.Apply([D("D1", 3, "count"), D("D2", 2, "count"), D("D3", 30, "mm"), D("D4", 20, "mm")], 2, 30, 3, 20);
        Assert.Equal(new() { ["D1"] = "count2", ["D2"] = "count1", ["D3"] = "spacing1", ["D4"] = "spacing2" }, Roles(r));
    }

    [Fact]
    public void A_value_the_definition_does_not_bear_out_gets_no_role()
    {
        var r = LinearPatternRoles.Apply([D("D1", 5, "count"), D("D2", 6, "count"), D("D3", 30, "mm"), D("D4", 20, "mm")], 2, 30, 3, 20);
        Assert.Equal(new() { ["D1"] = null, ["D2"] = null, ["D3"] = "spacing1", ["D4"] = "spacing2" }, Roles(r));
    }
}
