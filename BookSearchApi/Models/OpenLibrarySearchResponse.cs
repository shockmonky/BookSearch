// Take home project for Matthew Maffett

using System.Text.Json.Serialization;

namespace BookSearchApi.Models;

public record OpenLibrarySearchResponse(

    /// <summary>
    /// The number of entries found for the query.
    /// </summary>
    [property: JsonPropertyName("numFound")] int NumFound,

    /// <summary>
    /// The starting index of the of the first result.
    /// </summary>
    [property: JsonPropertyName("start")] int Start,

    /// <summary>
    /// The List of works that match the query.
    /// </summary>
    [property: JsonPropertyName("docs")] List<OpenLibraryBook> Docs);
