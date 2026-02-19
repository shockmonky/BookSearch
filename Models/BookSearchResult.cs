// Take home project for Matthew Maffett

using System.Text.Json.Serialization;

namespace BookSearchApi.Models;

public record BookSearchResult(
    string Key,
    string Title,
    List<string> Authors,
    int? FirstPublishYear,
    string? PrimaryIsbn,
    List<string> Publishers,
    List<string> Languages,
    List<string> Subjects,
    string? CoverUrl,
    int? EditionCount,
    int? PageCount,
    string OpenLibraryUrl);
