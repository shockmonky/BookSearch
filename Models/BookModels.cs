// Copyright (c) YourCompany. All rights reserved.

using System.Text.Json.Serialization;

namespace BookSearchApi.Models;

public record OpenLibrarySearchResponse(
    [property: JsonPropertyName("numFound")] int NumFound,
    [property: JsonPropertyName("start")] int Start,
    [property: JsonPropertyName("docs")] List<OpenLibraryBook> Docs);

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

public record BookSearchResponse(
    int TotalResults,
    int Page,
    int PageSize,
    List<BookSearchResult> Results);
