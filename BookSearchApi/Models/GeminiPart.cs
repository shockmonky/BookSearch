// Take home project for Matthew Maffett

using System.Text.Json.Serialization;

namespace BookSearchApi.Models;

public record GeminiPart(
    [property: JsonPropertyName("text")] string Text);
