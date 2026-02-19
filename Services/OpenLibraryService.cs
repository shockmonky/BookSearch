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

    public async Task<BookSearchResponse> SearchByTitleAsync(
        string query,
        int page = 1,
        int pageSize = 10,
        CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(query);

        page = Math.Max(1, page);
        pageSize = Math.Clamp(pageSize, 1, 100);

        var encodedQuery = Uri.EscapeDataString(query);
        var offset = (page - 1) * pageSize;

        var url = $"/search.json?title={encodedQuery}&limit={pageSize}&offset={offset}" +
                  "&fields=key,title,author_name,first_publish_year,isbn,publisher," +
                  "language,subject,cover_i,edition_count,number_of_pages_median";

        logger.LogInformation("Searching Open Library: {Url}", url);

        var response = await httpClient.GetFromJsonAsync<OpenLibrarySearchResponse>(
            url, cancellationToken);

        if (response is null)
        {
            return new BookSearchResponse(0, page, pageSize, []);
        }

        var results = response.Docs.Select(MapToBookSearchResult).ToList();

        return new BookSearchResponse(response.NumFound, page, pageSize, results);
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
            FirstPublishYear: book.FirstPublishYear,
            PrimaryIsbn: book.Isbn?.FirstOrDefault(),
            Publishers: book.Publishers ?? [],
            Languages: book.Languages ?? [],
            Subjects: book.Subjects?.Take(5).ToList() ?? [],
            CoverUrl: coverUrl,
            EditionCount: book.EditionCount,
            PageCount: book.PageCountMedian,
            OpenLibraryUrl: openLibraryUrl);
    }
}
