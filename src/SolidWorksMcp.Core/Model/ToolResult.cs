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
    /// up to twelve faces in full were 2 to 4k characters, read for one id or none); <paramref name="faceDetail"/> keeps
    /// them in full.
    /// </summary>
    public static string Serialize<T>(T value, bool faceDetail = false)
    {
        if (faceDetail || value is not INewFaces { NewFaces: { Count: > 0 } faces })
            return JsonSerializer.Serialize(value, value?.GetType() ?? typeof(T), Options);
        var node = JsonSerializer.SerializeToNode(value, value.GetType(), Options)!.AsObject();
        var brief = new System.Text.Json.Nodes.JsonArray();
        foreach (var f in faces) brief.Add(new System.Text.Json.Nodes.JsonObject { ["id"] = f.Id, ["surface"] = f.Surface });
        node["newFaces"] = brief; // in place: the property keeps its position
        return node.ToJsonString(Options);
    }
}

/// <summary>A result that lists the faces a feature made (<see cref="ResultJson.Serialize"/> shortens the list).</summary>
public interface INewFaces
{
    IReadOnlyList<FaceInfo>? NewFaces { get; }
}
