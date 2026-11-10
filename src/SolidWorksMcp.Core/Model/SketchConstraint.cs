using System.ComponentModel;

namespace SolidWorksMcp.Core.Model;

/// <summary>
/// One step for sketch_constrain: a relation (as sketch_relation_add takes it) or a dimension (as sketch_dimension_add
/// takes it). Exactly one of <see cref="Relation"/> and <see cref="Dimension"/> is set.
/// </summary>
public sealed class SketchConstraint
{
    [Description("A relation: horizontal | vertical | coincident | collinear | parallel | perpendicular | tangent | concentric | equal | " +
                 "symmetric | midpoint | intersection | fix | pierce | merge.")]
    public string? Relation { get; set; }

    [Description("A dimension: linear | horizontal | vertical | radial | diameter | angular.")]
    public string? Dimension { get; set; }

    [Description("Segments {name:'Line1'}, sketch points {at:[x,y]} (where they are before this call) or {name:'origin'}.")]
    public EntityRef[]? Entities { get; set; }

    [Description("Dimension value in mm (all kinds but angular); omit to keep the current size.")]
    public double? ValueMm { get; set; }

    [Description("angular: value in degrees.")]
    public double? ValueDeg { get; set; }

    [Description("Dimension text position [x,y] sketch mm; for angular it picks the angle.")]
    public double[]? TextAt { get; set; }
}
