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

    public async Task<List<BookSearchResult>> SearchByTitleAsync(
        string bookName,
        CancellationToken cancellationToken = default)
    {
        // Make the bookName safe to use in a URL
        var encodedBookName = Uri.EscapeDataString(bookName);

        // var url = $"/search.json?title={encodedQuery}&limit={pageSize}&offset={offset}" +
        //          "&fields=key,title,author_name,first_publish_year,isbn,publisher," +
        //          "language,subject,cover_i,edition_count,number_of_pages_median";
        var url = $"/search.json?title={encodedBookName}&fields=key,title,author_name,isbn" +
                  "language,subject,cover_i";

        logger.LogInformation("Searching Open Library: {Url}", url);

        // hit openlib and turn json into a SearchResponse
        var response = await httpClient.GetFromJsonAsync<OpenLibrarySearchResponse>(url, cancellationToken);

        if (response is null)
        {
            return [];
        }

        return response.Docs.Select(MapToBookSearchResult).ToList();
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
