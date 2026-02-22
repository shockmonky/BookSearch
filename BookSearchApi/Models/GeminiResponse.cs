// Take home project for Matthew Maffett

using System.Text.Json.Serialization;

namespace BookSearchApi.Models;

public record GeminiResponse(
    [property: JsonPropertyName("candidates")] List<GeminiCandidate>? Candidates);
