using System.Text.Json;
using System.Text.Json.Serialization;

namespace SolidWorksMcp.Core.Model;

/// <summary>
/// Base of every service result. <see cref="SwSession.RunAsync{T}"/> fills <see cref="Warnings"/> and
/// <see cref="DialogsHandled"/> from the operation context, so services only set their own data and <see cref="Doc"/>.
/// </summary>
public abstract class ToolResult
{
    /// <summary>The document the tool acted on (title).</summary>
    [JsonPropertyOrder(-100)]
    public string? Doc { get; set; }

    /// <summary>Non-fatal issues (rebuild warnings, restore problems, ...).</summary>
    [JsonPropertyOrder(1000)]
    public IReadOnlyList<string>? Warnings { get; set; }

    /// <summary>Dialogs the server answered automatically during the call.</summary>
    [JsonPropertyOrder(1001)]
    public IReadOnlyList<string>? DialogsHandled { get; set; }
}

/// <summary>JSON conventions for tool results: camelCase, no nulls, compact, enums as camelCase strings.</summary>
public static class ResultJson
{
    public static readonly JsonSerializerOptions Options = new(JsonSerializerDefaults.Web)
    {
        DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
        WriteIndented = false,
        NumberHandling = JsonNumberHandling.AllowNamedFloatingPointLiterals,
        Converters = { new JsonStringEnumConverter(JsonNamingPolicy.CamelCase) },
        Encoder = System.Text.Encodings.Web.JavaScriptEncoder.UnsafeRelaxedJsonEscaping,
    };

    /// <summary>
    /// The result as compact JSON. A result that lists a feature's new faces gives each one's id and surface only (S45:
    /// up to twelve faces in full were 2 to 4k characters, read for one id or none), and one with <see cref="IShortRows"/>
    /// its short rows (S48 d); <paramref name="detail"/> (faceDetail, equationDetail) keeps them in full.
    /// </summary>
    public static string Serialize<T>(T value, bool detail = false)
    {
        var brief = detail ? null : value switch
        {
            INewFaces { NewFaces: { Count: > 0 } faces } => ("newFaces", new System.Text.Json.Nodes.JsonArray(
                [.. faces.Select(f => (System.Text.Json.Nodes.JsonNode)new System.Text.Json.Nodes.JsonObject { ["id"] = f.Id, ["surface"] = f.Surface })])),
            IShortRows rows => rows.ShortRows(),
            _ => ((string, System.Text.Json.Nodes.JsonArray)?)null,
        };
        if (brief is not (string property, var items)) // null: nothing to shorten
            return JsonSerializer.Serialize(value, value?.GetType() ?? typeof(T), Options);
        var node = JsonSerializer.SerializeToNode(value, value!.GetType(), Options)!.AsObject();
        node[property] = items; // in place: the property keeps its position
        return node.ToJsonString(Options);
    }
}

/// <summary>A result whose rows <see cref="ResultJson.Serialize"/> gives in short unless detail is asked for.</summary>
public interface IShortRows
{
    /// <summary>The JSON property the short rows replace, and the rows; null = nothing to shorten.</summary>
    (string Property, System.Text.Json.Nodes.JsonArray Rows)? ShortRows();
}

/// <summary>A result that lists the faces a feature made (<see cref="ResultJson.Serialize"/> shortens the list).</summary>
public interface INewFaces
{
    IReadOnlyList<FaceInfo>? NewFaces { get; }
}
