using SolidWorksMcp.Core;
using SolidWorksMcp.Core.Model;
using SolidWorksMcp.Core.Services;
using Xunit;

namespace SolidWorksMcp.LiveTests;

/// <summary>
/// Section views in screenshots against a live SOLIDWORKS (skipped without SWMCP_LIVE_TESTS=1). Oracles: the sectioned
/// image differs from the plain one, and the plain one is the same before and after.
/// </summary>
public class VisualsLiveTests(LiveFixture fx) : LiveTest(fx)
{
    private ViewService V => new(S);

    private async Task<string> HoledBlockAsync(string name)
    {
        string doc = await NewPartAsync(name);
        var sk = new SketchService(S);
        var fs = new FeatureService(S);
        await sk.StartAsync(doc, new EntityRef { Name = "top" }, false);
        await sk.AddAsync(doc, [new SketchEntity { Kind = "rectangle", Center = [0, 0], WidthMm = 100, HeightMm = 60 }]);
        await fs.ExtrudeAsync(new ExtrudeRequest { Doc = doc, DepthMm = 20 });
        await sk.StartAsync(doc, new EntityRef { Name = "top" }, false);
        await sk.AddAsync(doc, [new SketchEntity { Kind = "circle", Center = [0, 0], DiameterMm = 20 }]);
        await fs.ExtrudeAsync(new ExtrudeRequest { Doc = doc, Cut = true, End = "through_all" });
        return doc;
    }

    [Fact]
    public async Task A_section_cuts_the_image_and_is_removed_afterwards()
    {
        string doc = await HoledBlockAsync("vis_section");
        var plain = await V.ScreenshotAsync(doc, "iso", true, "shaded_edges", true, false, 512, null, false);
        var cut = await V.ScreenshotAsync(doc, "iso", true, "shaded_edges", true, false, 512, null, false,
            new EntityRef { Name = "front" }, 0, false);
        Assert.NotNull(cut.Section);
        Assert.NotEqual(plain.Png, cut.Png);
        var again = await V.ScreenshotAsync(doc, "iso", true, "shaded_edges", true, false, 512, null, false);
        Assert.Equal(plain.Png, again.Png); // the section is gone
        await Assert.ThrowsAsync<SwException>(() => V.ScreenshotAsync(doc, "iso", true, "shaded_edges", true, false, 512, null, false,
            new EntityRef { Type = "face", At = [0, 10, 10] }, 0, false)); // the hole's face isn't planar
    }

    /// <summary>
    /// S65 (d): zoom to fit took in geometry far from the part that the image does not show, leaving the part a sliver of
    /// it. SOLIDWORKS' fit takes in every sketch, shown or hidden (a construction line in an absorbed sketch, the 3D sketch
    /// holding an axis's points), not reference axes. Oracle: the block's share of the image (pixels that differ from the
    /// empty part's image) against its share before the far geometry was added.
    /// </summary>
    [Fact]
    public async Task Zoom_to_fit_takes_what_the_image_shows_and_axes_are_hidden()
    {
        const int AxesToggle = 4; // swDisplayAxes
        string doc = await NewPartAsync("vis_fit");
        Assert.True(await TestHooks.DocToggleAsync(S, doc, AxesToggle, true)); // axes shown, as in the session that met it
        var blank = await V.ScreenshotAsync(doc, "iso", true, "shaded_edges", true, false, 512, null, false);
        var sk = new SketchService(S);
        var fs = new FeatureService(S);
        await sk.StartAsync(doc, new EntityRef { Name = "top" }, false);
        await sk.AddAsync(doc, [new SketchEntity { Kind = "rectangle", Center = [0, 0], WidthMm = 40, HeightMm = 40 }]);
        await fs.ExtrudeAsync(new ExtrudeRequest { Doc = doc, DepthMm = 20 });
        var block = await V.ScreenshotAsync(doc, "iso", true, "shaded_edges", true, false, 512, null, false);
        double alone = Share(blank.Png, block.Png);

        // An axis through the block: hidden by default and the toggle back on after; shown with hideAxes:false.
        await new ReferenceService(S).AxisAsync(doc, "two_planes", [new EntityRef { Name = "front" }, new EntityRef { Name = "right" }], null, null);
        var hidden = await V.ScreenshotAsync(doc, "iso", true, "shaded_edges", true, false, 512, null, false);
        Assert.True(await TestHooks.DocToggleAsync(S, doc, AxesToggle));
        var shown = await V.ScreenshotAsync(doc, "iso", true, "shaded_edges", true, false, 512, null, false, false);
        Assert.True(await TestHooks.DocToggleAsync(S, doc, AxesToggle));
        double axisPixels = Share(block.Png, shown.Png) - Share(block.Png, hidden.Png);
        Assert.True(axisPixels > 2e-4, $"the axis drawn with hideAxes:false: {axisPixels:0.00000} of the image");
        Assert.False(await TestHooks.DocToggleAsync(S, doc, AxesToggle, false)); // hidden by the document: stays so
        await V.ScreenshotAsync(doc, "iso", true, "shaded_edges", true, false, 512, null, false);
        Assert.False(await TestHooks.DocToggleAsync(S, doc, AxesToggle));
        Assert.True(await TestHooks.DocToggleAsync(S, doc, AxesToggle, true));

        // A hole whose sketch holds a construction line 150 mm out: the sketch is absorbed, hidden, and out of the fit.
        await sk.StartAsync(doc, new EntityRef { Name = "top" }, false);
        await sk.AddAsync(doc, [new SketchEntity { Kind = "circle", Center = [0, 0], DiameterMm = 10 },
            new SketchEntity { Kind = "centerline", From = [150, -60], To = [150, 60] }]);
        await fs.ExtrudeAsync(new ExtrudeRequest { Doc = doc, Cut = true, End = "through_all" });
        double withLine = Share(blank.Png, (await V.ScreenshotAsync(doc, "iso", true, "shaded_edges", true, false, 512, null, false)).Png);
        Assert.True(withLine >= 0.95 * alone && withLine <= 1.05 * alone,
            $"the block's share of the image with a hidden construction line 150 mm out: {withLine:0.0000}, without it {alone:0.0000}");

        // An axis 300 mm out through coordinates: its points' 3D sketch is shown, so in the fit, until hideSketches.
        await new ReferenceService(S).AxisAsync(doc, "two_points", null, [[300, 0, -200], [300, 0, 200]], null);
        double sketchShown = Share(blank.Png, (await V.ScreenshotAsync(doc, "iso", true, "shaded_edges", true, false, 512, null, false)).Png);
        Assert.True(sketchShown < 0.2 * alone, $"the shown 3D sketch left out of the fit: {sketchShown:0.0000} of {alone:0.0000}");
        double sketchHidden = Share(blank.Png, (await V.ScreenshotAsync(doc, "iso", true, "shaded_edges", true, true, 512, null, false)).Png);
        Assert.True(sketchHidden >= 0.95 * alone && sketchHidden <= 1.05 * alone,
            $"with hideSketches the block's share {sketchHidden:0.0000}, without the axis {alone:0.0000}");
        Assert.True(await TestHooks.DocToggleAsync(S, doc, AxesToggle));    }

    /// <summary>The share of pixels that differ from <paramref name="blank"/>'s (same size) by more than a faint shade.</summary>
    private static double Share(byte[] blank, byte[] png)
    {
        var (w, h, a) = Decode(blank);
        var (w2, h2, b) = Decode(png);
        Assert.Equal((w, h), (w2, h2));
        int n = 0;
        for (int i = 0; i < a.Length; i += 3)
            if (Math.Abs(a[i] - b[i]) + Math.Abs(a[i + 1] - b[i + 1]) + Math.Abs(a[i + 2] - b[i + 2]) > 48) n++;
        return (double)n / (w * h);
    }

    /// <summary>Decodes the server's own PNG (8-bit RGB, filters None or Sub).</summary>
    private static (int W, int H, byte[] Rgb) Decode(byte[] png)
    {
        int w = 0, h = 0;
        using var idat = new MemoryStream();
        for (int p = 8; p < png.Length;)
        {
            int len = System.Buffers.Binary.BinaryPrimitives.ReadInt32BigEndian(png.AsSpan(p));
            string type = System.Text.Encoding.ASCII.GetString(png, p + 4, 4);
            if (type == "IHDR")
            {
                w = System.Buffers.Binary.BinaryPrimitives.ReadInt32BigEndian(png.AsSpan(p + 8));
                h = System.Buffers.Binary.BinaryPrimitives.ReadInt32BigEndian(png.AsSpan(p + 12));
            }
            else if (type == "IDAT") idat.Write(png, p + 8, len);
            p += 12 + len;
        }
        idat.Position = 0;
        using var z = new System.IO.Compression.ZLibStream(idat, System.IO.Compression.CompressionMode.Decompress);
        using var raw = new MemoryStream();
        z.CopyTo(raw);
        var data = raw.ToArray();
        int stride = w * 3;
        var rgb = new byte[stride * h];
        for (int y = 0; y < h; y++)
        {
            byte filter = data[y * (stride + 1)];
            Assert.InRange(filter, (byte)0, (byte)1);
            for (int i = 0; i < stride; i++)
            {
                byte v = data[y * (stride + 1) + 1 + i];
                rgb[y * stride + i] = (byte)(filter == 1 && i >= 3 ? v + rgb[y * stride + i - 3] : v);
            }
        }
        return (w, h, rgb);
    }
}
