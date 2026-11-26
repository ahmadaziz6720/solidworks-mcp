using SolidWorks.Interop.sldworks;
using SolidWorks.Interop.swconst;
using SolidWorksMcp.Core.Com;
using SolidWorksMcp.Core.Geometry;

namespace SolidWorksMcp.Core.Services;

/// <summary>
/// A sketch's relations read by what they relate (S56): what SOLIDWORKS related and merged while sketch_add drew, and
/// whether a relation that sketch_relation_add or sketch_constrain asked for, and SOLIDWORKS ignored, already holds. A
/// segment of the sketch is known by its name and a point by its id, both stable while the sketch is open.
/// </summary>
internal static class SketchRelations
{
    private const int ListMax = 20; // relations listed per entity

    /// <summary>A relation: its type and the keys of what it relates.</summary>
    internal sealed record Rel(int Type, IReadOnlyList<string> Keys, IReadOnlyList<object> Entities)
    {
        public string Signature => $"{Type}|{string.Join("|", Keys.Order(StringComparer.Ordinal))}";
    }

    public static int Count(ISketch s)
    {
        try { return s.RelationManager?.GetRelationsCount((int)swSketchRelationFilterType_e.swAll) ?? 0; }
        catch (Exception ex) when (!ComArgs.IsDisconnect(ex)) { return 0; }
    }

    private static string NameOf(ISketch s) => ((IFeature)s).Name;

    public static string PointKey(ISketchPoint p) => "p:" + string.Join(",", ComArgs.Ints((object?)p.GetID()));

    /// <summary>
    /// The key of a relation's entity, or of a referenced one: "s:Line1", "p:…" in this sketch; "origin"; "x:…" for
    /// another sketch's; "f:Name" for a feature; "?…" for model geometry, which a key can't tell apart.
    /// </summary>
    public static string Key(object? o, string sketch)
    {
        try
        {
            switch (o)
            {
                case ISketchSegment s:
                {
                    string own = (object?)s.GetSketch() is ISketch os ? NameOf(os) : sketch;
                    return own.Equals(sketch, StringComparison.OrdinalIgnoreCase) ? "s:" + s.GetName() : $"x:{s.GetName()}@{own}";
                }
                case ISketchPoint p:
                {
                    if ((object?)p.GetSketch() is not ISketch os) return "?point";
                    string own = NameOf(os);
                    if (own.Equals(sketch, StringComparison.OrdinalIgnoreCase)) return PointKey(p);
                    // Verified (2023): the origin's point, in a relation to it, belongs to the origin feature's sketch.
                    if (((IFeature)os).GetTypeName2() == "OriginProfileFeature") return "origin";
                    return $"x:{PointKey(p)}@{own}";
                }
                case IFeature f:
                    return f.GetTypeName2() == "OriginProfileFeature" ? "origin" : "f:" + f.Name;
                case IEdge:
                    return "?edge";
                case IFace2:
                    return "?face";
            }
        }
        catch (Exception ex) when (!ComArgs.IsDisconnect(ex)) { /* an entity that can't be read: told apart from nothing */ }
        return "?";
    }

    /// <summary>A segment's end and centre points: ("start", p), ("end", p), ("centre", p).</summary>
    public static List<(string Which, ISketchPoint P)> Ends(ISketchSegment s)
    {
        var list = new List<(string, ISketchPoint)>();
        void Add(string w, ISketchPoint? p) { if (p is not null) list.Add((w, p)); }
        switch (s.GetType())
        {
            case (int)swSketchSegments_e.swSketchLINE when s is ISketchLine l:
                Add("start", l.SkStart()); Add("end", l.SkEnd());
                break;
            case (int)swSketchSegments_e.swSketchARC when s is ISketchArc a:
                Add("start", a.SkStart()); Add("end", a.SkEnd()); Add("centre", a.SkCenter());
                break;
            case (int)swSketchSegments_e.swSketchELLIPSE when s is ISketchEllipse e:
                Add("start", e.SkStart()); Add("end", e.SkEnd()); Add("centre", e.SkCenter());
                break;
            case (int)swSketchSegments_e.swSketchSPLINE when s is ISketchSpline sp:
                var pts = sp.SkFitPoints();
                if (pts.Count > 0) { Add("start", pts[0]); Add("end", pts[^1]); }
                break;
        }
        return list;
    }

    /// <summary>Who has each point: its key → "Line1 start", "Arc2 centre".</summary>
    public static Dictionary<string, List<string>> Owners(IEnumerable<ISketchSegment> segs)
    {
        var map = new Dictionary<string, List<string>>(StringComparer.Ordinal);
        foreach (var s in segs)
        {
            string n = s.GetName();
            foreach (var (w, p) in Ends(s))
            {
                string k = PointKey(p);
                if (!map.TryGetValue(k, out var l)) map[k] = l = [];
                if (!l.Contains($"{n} {w}")) l.Add($"{n} {w}");
            }
        }
        return map;
    }

    /// <summary>
    /// S144: who has a point: the ends it is ("Line1 end", from <see cref="Owners"/>), else the segments it is coincident
    /// with ("on Line5, Line6": a centre rectangle's centre on its diagonals), else "a free point".
    /// </summary>
    public static string Who(ISketchPoint p, IReadOnlyDictionary<string, List<string>> owners)
    {
        if (owners.TryGetValue(PointKey(p), out var o) && o.Count > 0) return string.Join(", ", o);
        var on = new List<string>();
        try
        {
            foreach (var r in p.SkRelations())
                if (r.GetRelationType() == (int)swConstraintType_e.swConstraintType_COINCIDENT)
                    foreach (var e in Read(r, "").Entities)
                        if (e is ISketchSegment s && !on.Contains(s.GetName())) on.Add(s.GetName());
        }
        catch (Exception ex) when (!ComArgs.IsDisconnect(ex)) { /* informational */ }
        return on.Count > 0 ? "on " + string.Join(", ", on) : "a free point";
    }

    public static HashSet<string> PointKeys(ISketch s) => s.Points().Select(PointKey).ToHashSet(StringComparer.Ordinal);

    /// <summary>
    /// S101: who has each point of the sketch through one sketch_add (<see cref="Owners"/>), read at its first use and kept
    /// up as each entity draws: read again per entity, it took 115 of the 165 s of 100 lines.
    /// </summary>
    internal sealed class PointOwners(ISketch sketch)
    {
        private Dictionary<string, List<string>>? _map;

        /// <summary>The owners of <paramref name="key"/> that are no segment in <paramref name="except"/>.</summary>
        public List<string>? Others(string key, IReadOnlySet<string> except) =>
            (_map ??= Owners(sketch.Segments())).TryGetValue(key, out var l)
                ? l.Where(o => !except.Contains(o[..o.LastIndexOf(' ')])).ToList() : null;

        /// <summary>A point of a segment just drawn ("Line5 start"); nothing before the first read, which reads it.</summary>
        public void Add(string key, string owner)
        {
            if (_map is null) return;
            if (!_map.TryGetValue(key, out var l)) _map[key] = l = [];
            if (!l.Contains(owner)) l.Add(owner);
        }
    }

    private static List<ISketchRelation> On(object o) => o switch
    {
        ISketchSegment s => s.SkRelations(),
        ISketchPoint p => p.SkRelations(),
        _ => [],
    };

    private static Rel Read(ISketchRelation r, string sketch)
    {
        // Verified (2023): GetEntities gives model geometry as the sketch's own external point or segment (point type 10);
        // GetDefinitionEntities2 gives what it is, e.g. the origin's point (in the origin's sketch) for a corner on it.
        static List<object> Objs(object? v)
        {
            var list = new List<object>();
            if (v is Array a)
                foreach (var o in a)
                    if (o is not null) list.Add(o);
            return list;
        }
        var ents = Objs((object?)r.GetDefinitionEntities2());
        if (ents.Count == 0) ents = Objs((object?)r.GetEntities());
        return new Rel(r.GetRelationType(), ents.Select(e => Key(e, sketch)).ToList(), ents);
    }

    /// <summary>The relations on these entities, each once.</summary>
    private static List<Rel> RelationsOn(IEnumerable<object> objs, string sketch)
    {
        var seen = new HashSet<string>(StringComparer.Ordinal);
        var list = new List<Rel>();
        foreach (var o in objs)
            foreach (var r in On(o))
            {
                var rel = Read(r, sketch);
                if (seen.Add(rel.Signature)) list.Add(rel);
            }
        return list;
    }

    private static string Fmt(ISketchPoint p, bool is3D) =>
        TopoQuery.Fmt(is3D ? [Units.Mm(p.X), Units.Mm(p.Y), Units.Mm(p.Z)] : [Units.Mm(p.X), Units.Mm(p.Y)]);

    private static string Describe(Rel r, Func<string, object, string> label) =>
        $"{TypeName(r.Type)}: {string.Join(", ", r.Keys.Select((k, i) => label(k, r.Entities[i])))}";

    // ------------------------------------------------------------------ sketch_delete

    /// <summary>Every relation of the sketch with the keys of what it relates (empty when they can't be listed).</summary>
    public static List<Rel> All(ISketch s)
    {
        var list = new List<Rel>();
        try
        {
            if (s.RelationManager is not ISketchRelationManager rm) return list;
            string sk = NameOf(s);
            foreach (var r in rm.SkRelations((int)swSketchRelationFilterType_e.swAll)) list.Add(Read(r, sk));
        }
        catch (Exception ex) when (!ComArgs.IsDisconnect(ex)) { /* informational */ }
        return list;
    }

    /// <summary>
    /// S86 (a): the relations (from <see cref="All"/>, read before a delete) that touched one of the segments in
    /// <paramref name="gone"/> or a point only they owned (<paramref name="owners"/> from <see cref="Owners"/>), as
    /// "equal: Arc1, Arc2", each once.
    /// </summary>
    public static List<string> Touching(IReadOnlyList<Rel> rels, IReadOnlyDictionary<string, List<string>> owners, IReadOnlySet<string> gone)
    {
        bool Mine(string k) => k.StartsWith("s:", StringComparison.Ordinal)
            ? gone.Contains(k[2..])
            : owners.TryGetValue(k, out var o) && o.Count > 0 && o.All(x => gone.Contains(x.Split(' ')[0]));
        string Label(string k) => k switch
        {
            _ when k.StartsWith("s:", StringComparison.Ordinal) => k[2..],
            _ when k.StartsWith("p:", StringComparison.Ordinal) => owners.TryGetValue(k, out var o) && o.Count > 0 ? o[0] : "a point",
            "origin" => "origin",
            _ when k.StartsWith("x:", StringComparison.Ordinal) => "a point of another sketch",
            _ when k.StartsWith("f:", StringComparison.Ordinal) => k[2..],
            _ => "a model entity",
        };
        return rels.Where(r => r.Keys.Any(Mine)).Select(r => $"{TypeName(r.Type)}: {string.Join(", ", r.Keys.Select(Label))}")
                   .Distinct(StringComparer.Ordinal).ToList();
    }

    // ------------------------------------------------------------------ sketch_add

    /// <summary>
    /// What SOLIDWORKS made on the segments one sketch_add entity drew: the relations on them and on their points (the
    /// sketch had <paramref name="relationsBefore"/> before), and their points it merged onto one in
    /// <paramref name="known"/> (the sketch's points before this entity; this entity's are added to it).
    /// </summary>
    public static (List<string>? Relations, List<string>? Merged) Made(ISketch sketch, IReadOnlyList<ISketchSegment> segs, int newPoints,
        int relationsBefore, HashSet<string> known, PointOwners owners)
    {
        string sk = NameOf(sketch);
        bool is3D = sketch.Is3D();
        var names = new HashSet<string>(StringComparer.Ordinal);
        var pts = new Dictionary<string, (string Label, ISketchPoint P)>(StringComparer.Ordinal);
        var mine = new List<(string Key, string Owner)>();
        foreach (var s in segs)
        {
            string n = s.GetName();
            names.Add(n);
            foreach (var (w, p) in Ends(s))
            {
                string k = PointKey(p);
                pts.TryAdd(k, ($"{n} {w}", p));
                mine.Add((k, $"{n} {w}"));
            }
        }
        var mergedKeys = pts.Keys.Where(known.Contains).ToHashSet(StringComparer.Ordinal);
        List<string>? Others(string k) => owners.Others(k, names);

        List<string>? merged = null;
        if (mergedKeys.Count > 0)
            merged = mergedKeys.Select(k =>
            {
                var (lbl, p) = pts[k];
                var o = Others(k);
                return $"{lbl} merged onto [{Fmt(p, is3D)}]" + (o is { Count: > 0 } ? $" ({string.Join(", ", o.Take(3))})" : "");
            }).ToList();
        known.UnionWith(pts.Keys);
        if (newPoints > pts.Count - mergedKeys.Count) known.UnionWith(PointKeys(sketch)); // free points (a point, a centre)
        foreach (var (k, o) in mine) owners.Add(k, o);

        int now = Count(sketch);
        if (now <= relationsBefore) return (null, merged);
        var objs = segs.Cast<object>().Concat(pts.Where(kv => !mergedKeys.Contains(kv.Key)).Select(kv => (object)kv.Value.P));
        var rels = RelationsOn(objs, sk);
        string Label(string k, object o) => k switch
        {
            _ when k.StartsWith("s:", StringComparison.Ordinal) => k[2..],
            _ when k.StartsWith("p:", StringComparison.Ordinal) =>
                pts.TryGetValue(k, out var v) ? v.Label
                : Others(k) is { Count: > 0 } ow ? ow[0]
                : o is ISketchPoint p ? $"point [{Fmt(p, is3D)}]" : "a point",
            "origin" => "origin",
            _ when k.StartsWith("x:", StringComparison.Ordinal) && o is ISketchSegment xs => k[2..],
            _ when k.StartsWith("x:", StringComparison.Ordinal) => $"a point of {k[(k.LastIndexOf('@') + 1)..]}",
            _ when k.StartsWith("f:", StringComparison.Ordinal) => k[2..],
            "?edge" => "a model edge",
            "?face" => "a model face",
            _ => "a model entity",
        };
        var list = rels.Take(ListMax).Select(r => Describe(r, Label)).ToList();
        if (rels.Count > ListMax) list.Add($"{rels.Count - ListMax} more (sketch_info counts them)");
        int rest = now - relationsBefore - rels.Count;
        if (rest > 0) list.Add($"{rest} more on the merged points (sketch_info counts them)");
        return (list, merged);
    }

    // ------------------------------------------------------------------ an ignored relation

    private static readonly Dictionary<string, swConstraintType_e[]> Equivalent = new(StringComparer.Ordinal)
    {
        ["horizontal"] = [swConstraintType_e.swConstraintType_HORIZONTAL, swConstraintType_e.swConstraintType_HORIZPOINTS],
        ["vertical"] = [swConstraintType_e.swConstraintType_VERTICAL, swConstraintType_e.swConstraintType_VERTPOINTS],
        ["coincident"] = [swConstraintType_e.swConstraintType_COINCIDENT, swConstraintType_e.swConstraintType_MERGEPOINTS, swConstraintType_e.swConstraintType_ATMIDDLE],
        ["merge"] = [swConstraintType_e.swConstraintType_MERGEPOINTS, swConstraintType_e.swConstraintType_COINCIDENT],
        ["collinear"] = [swConstraintType_e.swConstraintType_COLINEAR],
        ["parallel"] = [swConstraintType_e.swConstraintType_PARALLEL, swConstraintType_e.swConstraintType_COLINEAR],
        ["perpendicular"] = [swConstraintType_e.swConstraintType_PERPENDICULAR],
        ["tangent"] = [swConstraintType_e.swConstraintType_TANGENT],
        ["concentric"] = [swConstraintType_e.swConstraintType_CONCENTRIC, swConstraintType_e.swConstraintType_CORADIAL],
        ["equal"] = [swConstraintType_e.swConstraintType_SAMELENGTH, swConstraintType_e.swConstraintType_CORADIAL],
        ["symmetric"] = [swConstraintType_e.swConstraintType_SYMMETRIC],
        ["midpoint"] = [swConstraintType_e.swConstraintType_ATMIDDLE],
        ["intersection"] = [swConstraintType_e.swConstraintType_ATINTERSECT],
        ["fix"] = [swConstraintType_e.swConstraintType_FIXED],
        ["pierce"] = [swConstraintType_e.swConstraintType_ATPIERCE],
    };

    /// <summary>Relations that carry over a chain (a = b and b = c hold a = c); the others need one relation on all of them.</summary>
    private static readonly HashSet<string> Chains = new(StringComparer.Ordinal) { "equal", "parallel", "collinear", "concentric", "merge" };

    /// <summary>
    /// Whether the sketch already holds <paramref name="rel"/> on these entities, which SOLIDWORKS ignored: a relation of
    /// its type (or an equivalent one: collinear for parallel, coradial for equal), a chain of them for those that carry
    /// over, a horizontal and a vertical line for perpendicular, a point that is the segment's own end or centre, or two
    /// points that are one. Returns how it holds, or null. Model geometry (edges, faces) is never claimed.
    /// </summary>
    public static string? Held(string rel, IReadOnlyList<Resolved> refs, ISketch sketch)
    {
        if (!Equivalent.TryGetValue(rel, out var types)) return null;
        string sk = NameOf(sketch);
        var keys = refs.Select(r => r.Kind == EntityKind.Origin ? "origin" : Key(r.Obj, sk)).ToList();
        if (keys.Any(k => k.StartsWith('?'))) return null;
        string L(int i) => refs[i].Label;

        // A point that is the segment's own end (coincident) or centre (concentric).
        if (rel is "coincident" or "concentric" && refs.Count == 2)
        {
            int pi = refs[0].Obj is ISketchPoint ? 0 : refs[1].Obj is ISketchPoint ? 1 : -1;
            if (pi >= 0 && refs[1 - pi].Obj is ISketchSegment seg && keys[1 - pi].StartsWith("s:", StringComparison.Ordinal))
            {
                var own = Ends(seg).Where(e => rel == "coincident" ? e.Which != "centre" : e.Which == "centre")
                    .FirstOrDefault(e => PointKey(e.P) == keys[pi]);
                if (own.P is not null)
                    return $"{rel} ({L(pi)} is the {own.Which} of {keys[1 - pi][2..]}); no relation added";
            }
        }

        var rels = RelationsOn(refs.Where((r, i) => keys[i][0] is 's' or 'p').Select(r => r.Obj), sk);
        var of = rels.Where(r => types.Contains((swConstraintType_e)r.Type)).ToList();
        string Names(IEnumerable<Rel> rs) => string.Join("/", rs.Select(r => TypeName(r.Type)).Distinct());

        bool lines = refs.All(r => r.Obj is ISketchSegment);
        if ((rel is "horizontal" or "vertical" or "fix") && (lines || rel == "fix"))
        {
            // One relation per entity is enough (a horizontal line each, a fix each).
            var each = keys.Select(k => of.FirstOrDefault(r => r.Keys.Contains(k))).ToList();
            return each.All(r => r is not null) ? $"{rel} (the sketch's {Names(each!)} relation on each); no relation added" : null;
        }
        if (Chains.Contains(rel) || rel is "horizontal" or "vertical")
        {
            // Union of the related entities: all of them in one group.
            var group = new Dictionary<string, string>(StringComparer.Ordinal);
            string Find(string k) { while (group.TryGetValue(k, out var p) && p != k) k = p; return k; }
            foreach (var r in of)
                for (int i = 1; i < r.Keys.Count; i++)
                {
                    string a = Find(r.Keys[0]), b = Find(r.Keys[i]);
                    group.TryAdd(a, a);
                    if (a != b) group[b] = a;
                }
            if (keys.Count > 1 && keys.All(k => Find(k) == Find(keys[0])) && group.ContainsKey(Find(keys[0])))
                return $"{rel} (the sketch's {Names(of.Where(r => r.Keys.Any(keys.Contains)))} relation(s) on {string.Join(", ", refs.Select((_, i) => L(i)))}); no relation added";
        }
        else
        {
            var one = of.FirstOrDefault(r => keys.All(r.Keys.Contains));
            if (one is not null)
                return $"{rel} (the sketch's {TypeName(one.Type)} relation on {string.Join(", ", refs.Select((_, i) => L(i)))}); no relation added";
        }
        if (rel is "parallel" or "perpendicular" && lines && refs.Count >= 2)
        {
            // Lines each held horizontal or vertical.
            string? Dir(string k) =>
                rels.Any(r => r.Type == (int)swConstraintType_e.swConstraintType_HORIZONTAL && r.Keys.Contains(k)) ? "horizontal"
                : rels.Any(r => r.Type == (int)swConstraintType_e.swConstraintType_VERTICAL && r.Keys.Contains(k)) ? "vertical" : null;
            var dirs = keys.Select(Dir).ToList();
            bool ok = dirs.All(d => d is not null) &&
                      (rel == "parallel" ? dirs.Distinct().Count() == 1 : refs.Count == 2 && dirs[0] != dirs[1]);
            if (ok)
                return $"{rel} ({string.Join(", ", refs.Select((_, i) => $"{L(i)} {dirs[i]}"))}); no relation added";
        }
        return null;
    }

    /// <summary>A relation type's name as the tools give it ("equal", "midpoint", …).</summary>
    public static string TypeName(int t)
    {
        if (!Enum.IsDefined(typeof(swConstraintType_e), t)) return $"type_{t}";
        string raw = ((swConstraintType_e)t).ToString().Replace("swConstraintType_", "").ToLowerInvariant();
        return raw switch
        {
            "samelength" => "equal",
            "atmiddle" => "midpoint",
            "atintersect" => "intersection",
            "colinear" => "collinear",
            "fixed" => "fix",
            "atpierce" => "pierce",
            "mergepoints" => "merge",
            "horizpoints" => "horizontal_points",
            "vertpoints" => "vertical_points",
            _ => raw,
        };
    }
}
