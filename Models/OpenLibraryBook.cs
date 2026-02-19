// Take home project for Matthew Maffett

using System.Text.Json.Serialization;

namespace BookSearchApi.Models;

public record OpenLibraryBook(
    [property: JsonPropertyName("key")] string Key,
    [property: JsonPropertyName("title")] string Title,
    [property: JsonPropertyName("author_name")] List<string>? AuthorNames,
    [property: JsonPropertyName("isbn")] List<string>? Isbn,
    [property: JsonPropertyName("language")] List<string>? Languages,
    [property: JsonPropertyName("subject")] List<string>? Subjects,
    [property: JsonPropertyName("cover_i")] int? CoverId);
