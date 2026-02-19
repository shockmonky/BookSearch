// Take home project for Matthew Maffett

using System.Text.Json.Serialization;

namespace BookSearchApi.Models;

public record BookSearchResponse(
    int TotalResults,
    int Page,
    int PageSize,
    List<BookSearchResult> Results);
