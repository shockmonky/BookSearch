// Take home project for Matthew Maffett

using System.Text.Json.Serialization;

namespace BookSearchApi.Models;

public record GeminiCandidate(
    [property: JsonPropertyName("content")] GeminiContent? Content);
