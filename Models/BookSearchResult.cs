// Take home project for Matthew Maffett

using System.Text.Json.Serialization;

namespace BookSearchApi.Models;

public record BookSearchResult(
    string Key,
    string Title,
    List<string> Authors,
    string? PrimaryIsbn,
    List<string> Languages,
    List<string> Subjects,
    string? CoverUrl,
    string OpenLibraryUrl);
