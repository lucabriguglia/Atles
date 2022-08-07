using System.Text.Json.Serialization;

namespace Atles.Models;

public class ProblemDetails
{
    [JsonPropertyName("title")]
    public string Title { get; set; }

    [JsonPropertyName("status")]
    public int? Status { get; set; }

    [JsonPropertyName("detail")]
    public string Detail { get; set; }
}
