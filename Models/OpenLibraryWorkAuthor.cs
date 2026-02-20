// Take home project for Matthew Maffett
using System.Text.Json.Serialization;

namespace BookSearchApi.Models;

public record OpenLibraryWorkAuthor(
    [property: JsonPropertyName("author")] OpenLibraryAuthorRef? Author);
