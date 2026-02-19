// Take home project for Matthew Maffett

using System.Text.Json.Serialization;

namespace BookSearchApi.Models;

public record OpenLibraryBook(
    [property: JsonPropertyName("key")] string Key,
    [property: JsonPropertyName("title")] string Title,
    [property: JsonPropertyName("author_name")] List<string>? AuthorNames,
    [property: JsonPropertyName("first_publish_year")] int? FirstPublishYear,
    [property: JsonPropertyName("isbn")] List<string>? Isbn,
    [property: JsonPropertyName("publisher")] List<string>? Publishers,
    [property: JsonPropertyName("language")] List<string>? Languages,
    [property: JsonPropertyName("subject")] List<string>? Subjects,
    [property: JsonPropertyName("cover_i")] int? CoverId,
    [property: JsonPropertyName("edition_count")] int? EditionCount,
    [property: JsonPropertyName("number_of_pages_median")] int? PageCountMedian);
