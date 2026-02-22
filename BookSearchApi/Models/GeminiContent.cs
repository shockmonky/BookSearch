// Take home project for Matthew Maffett

using System.Text.Json.Serialization;

namespace BookSearchApi.Models;

public record GeminiContent(
    [property: JsonPropertyName("parts")] List<GeminiPart> Parts);
