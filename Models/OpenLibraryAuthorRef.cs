// Take home project for Matthew Maffett
using System.Text.Json.Serialization;

namespace BookSearchApi.Models;

public record OpenLibraryAuthorRef(
    [property: JsonPropertyName("key")] string? Key);
