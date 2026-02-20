// Take home project for Matthew Maffett

using System.Net.Http.Json;
using System.Text;
using BookSearchApi.Models;

namespace BookSearchApi.Services;

public class OpenLibraryService(HttpClient httpClient, ILogger<OpenLibraryService> logger)
    : IOpenLibraryService
{
    private const string CoverBaseUrl = "https://covers.openlibrary.org/b/id";
    private const string OpenLibraryBaseUrl = "https://openlibrary.org";
    private StringBuilder searchFields = new StringBuilder("&fields=key,title,author_name,isbn,language,subject,cover_i");

    public async Task<List<BookSearchResult>> SearchByTitleAsync(
        string bookName,
        CancellationToken cancellationToken = default)
    {
        // Make the bookName safe to use in a URL
        var encodedBookName = Uri.EscapeDataString(bookName);

        // Add the safe book name to the front of the url query
        var url = this.searchFields.Insert(0, $"/search.json?title={encodedBookName}");

        logger.LogInformation("Searching Open Library: {Url}", url);

        // By default open library gives us 100 results. Turn the json into a SearchResponse
        var response = await httpClient.GetFromJsonAsync<OpenLibrarySearchResponse>(url.ToString(), cancellationToken);

        if (response is null)
        {
            return [];
        }

        return response.Docs.Select(MapToBookSearchResult).ToList();
    }

    public async Task<List<BookSearchResult>> SearchBySubjectAsync(
        string subjectName,
        CancellationToken cancellationToken = default)
    {
        // Make the subjectName safe to use in a URL
        var encodedSubjectName = Uri.EscapeDataString(subjectName);

        // Add the safe book name to the front of the url query
        var url = this.searchFields.Insert(0, $"/search.json?title={encodedSubjectName}");

        logger.LogInformation("Searching Open Library: {Url}", url);

        // By default open library gives us 100 results. Turn the json into a SearchResponse
        var response = await httpClient.GetFromJsonAsync<OpenLibrarySearchResponse>(url.ToString(), cancellationToken);

        if (response is null)
        {
            return [];
        }

        return response.Docs.Select(MapToBookSearchResult).ToList();
    }

    public async Task<OpenLibraryBook?> GetByKeyAsync(string key, CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(key);

        // Standardize our key
        var cleanKey = key.Trim().TrimStart('/');

        var url = $"{cleanKey}.json";

        logger.LogInformation("Getting book by key: {Url}", url);

        var addr = httpClient.BaseAddress + url;

        // By default open library gives us 100 results. Turn the json into a SearchResponse
        var response = await httpClient.GetFromJsonAsync<OpenLibraryWorkResponse>(url, cancellationToken);

        // If we got nothing back return null
        if (response is null)
        {
            return null;
        }

        return new OpenLibraryBook(
        Key: response.Key,
        Title: response.Title,
        AuthorNames: null,
        Isbn: null,
        Languages: null,
        Subjects: response.Subjects?.Take(5).ToList(),
        CoverId: response.Covers?.FirstOrDefault());
    }

    private static BookSearchResult MapToBookSearchResult(OpenLibraryBook book)
    {
        var coverUrl = book.CoverId.HasValue
            ? $"{CoverBaseUrl}/{book.CoverId}-M.jpg"
            : null;

        var openLibraryUrl = $"{OpenLibraryBaseUrl}{book.Key}";

        return new BookSearchResult(
            Key: book.Key,
            Title: book.Title,
            Authors: book.AuthorNames ?? [],
            PrimaryIsbn: book.Isbn?.FirstOrDefault(),
            Languages: book.Languages ?? [],
            Subjects: book.Subjects?.Take(5).ToList() ?? [],
            CoverUrl: coverUrl,
            OpenLibraryUrl: openLibraryUrl);
    }
}
