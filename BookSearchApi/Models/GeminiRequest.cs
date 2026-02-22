// Take home project for Matthew Maffett

using System.Text.Json.Serialization;

namespace BookSearchApi.Models;

public record GeminiRequest(
    [property: JsonPropertyName("contents")] List<GeminiContent> Contents);
