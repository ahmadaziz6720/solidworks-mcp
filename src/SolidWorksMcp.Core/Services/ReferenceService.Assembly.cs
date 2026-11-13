using System.Runtime.InteropServices;
using SolidWorks.Interop.sldworks;
using SolidWorks.Interop.swconst;
using SolidWorksMcp.Core.Com;
using SolidWorksMcp.Core.Geometry;

namespace SolidWorksMcp.Core.Services;

/// <summary>
/// ref_plane and ref_axis in an assembly (S59 b). The new feature is the assembly's own; its references may be the
/// assembly's planes or a component's faces, edges, vertices, planes and axes. A component's geometry reads in that
/// part's space (docs/solidworks-api-notes.md §8, "Coordinate spaces in assemblies"), so it is mapped through the
/// component transform before the request is checked against it; the new feature reads in assembly space.
/// </summary>
public sealed partial class ReferenceService
{
    private static bool IsAssembly(IModelDoc2 d) => d.GetType() == (int)swDocumentTypes_e.swDocASSEMBLY;

    /// <summary>The component a resolved reference belongs to, or null (a part, or the assembly's own feature).</summary>
    private static IComponent2? OwnerOf(Resolved r)
    {
        IComponent2? c = r.Component;
        if (c is null && r.Obj is IFace2 or IEdge or IVertex)
        {
            try { c = ((IEntity)r.Obj).AsmComponentOf(); }
            catch (COMException e) when (!ComArgs.IsDisconnect(e)) { c = null; }
        }
        if (c is null) return null;
        try { return c.IsRoot() ? null : c; }
        catch (COMException e) when (!ComArgs.IsDisconnect(e)) { return c; }
    }

    /// <summary>From the reference's own coordinates to the document's: identity unless it is a component's.</summary>
    private static Xform ToDoc(Resolved r) =>
        OwnerOf(r) is IComponent2 c && c.AsmXformData() is { Length: >= 13 } t ? new Xform(t) : Xform.Identity;

    /// <summary>
    /// The feature to read a reference plane, axis or point from: for a component's feature, the feature of that name in
    /// the component's own document (part space, as its bodies; the mates read it so too), else the resolved feature.
    /// </summary>
    private static IFeature OwnFeature(Resolved r, IFeature f)
    {
        if (r.Component is IComponent2 c && c.AsmModelDoc() is IModelDoc2 md && EntityResolver.FeatureByName(md, f.Name) is IFeature own) return own;
        return f;
    }

    private static PlaneGeom Mapped(PlaneGeom g, Xform xf) =>
        xf.IsIdentity() ? g : new PlaneGeom(xf.ApplyPoint(g.Origin), xf.ApplyVector(g.Normal).Normalized(), g.XAxis is Vec3 x ? xf.ApplyVector(x).Normalized() : null);

    private static LineGeom Mapped(LineGeom l, Xform xf) =>
        xf.IsIdentity() ? l : new LineGeom(xf.ApplyPoint(l.Point), xf.ApplyVector(l.Dir).Normalized());

    /// <summary>
    /// Selects a component's plane, axis or point the VERIFIED way, by its SelectByID2 name ("Top Plane@box-1@Assem1"), as
    /// asm_mate_add does; false when that is not such a reference or SOLIDWORKS picked nothing.
    /// </summary>
    private static bool SelectComponentFeature(SwContext ctx, IModelDoc2 d, Resolved r, int mark, int countBefore)
    {
        if (r.Obj is not IFeature f || r.Component is not IComponent2 comp || r.Kind is not (EntityKind.Plane or EntityKind.Axis or EntityKind.Point)) return false;
        string type = r.Kind switch { EntityKind.Plane => "PLANE", EntityKind.Axis => "AXIS", _ => "DATUMPOINT" };
        var sel = d.Sel();
        foreach (var id in AssemblyService.SelectIds(ctx, d, f.Name, comp))
        {
            if (d.Extension.SelectByID2(id, type, 0, 0, 0, true, mark, null, 0) && sel.GetSelectedObjectCount2(-1) == countBefore + 1) return true;
            while (sel.GetSelectedObjectCount2(-1) > countBefore) sel.DeSelect2(sel.GetSelectedObjectCount2(-1), -1);
        }
        return false;
    }

    /// <summary>The check that says where the feature was made, in an assembly (null in a part).</summary>
    private static string? MadeIn(SwContext ctx, IModelDoc2 d) =>
        IsAssembly(d) ? $"made in the tree of assembly '{ctx.Title(d)}'" : null;
}
