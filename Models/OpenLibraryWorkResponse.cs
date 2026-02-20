// Take home project for Matthew Maffett
using System.Text.Json.Serialization;

namespace BookSearchApi.Models;

public record OpenLibraryWorkResponse(
    [property: JsonPropertyName("key")] string Key,
    [property: JsonPropertyName("title")] string Title,
    [property: JsonPropertyName("covers")] List<int>? Covers,
    [property: JsonPropertyName("subjects")] List<string>? Subjects,
    [property: JsonPropertyName("first_publish_date")] string? FirstPublishDate,
    [property: JsonPropertyName("authors")] List<OpenLibraryWorkAuthor>? Authors);
