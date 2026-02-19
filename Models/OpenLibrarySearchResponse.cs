// Take home project for Matthew Maffett

using System.Text.Json.Serialization;

namespace BookSearchApi.Models;

public record OpenLibrarySearchResponse(
    [property: JsonPropertyName("numFound")] int NumFound,
    [property: JsonPropertyName("start")] int Start,
    [property: JsonPropertyName("docs")] List<OpenLibraryBook> Docs);
