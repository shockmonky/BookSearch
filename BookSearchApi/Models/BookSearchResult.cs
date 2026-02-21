// Take home project for Matthew Maffett

using System.Text.Json.Serialization;

namespace BookSearchApi.Models;

/// <summary>
/// Class represents a book from the Open Library API.
/// </summary>
public record BookSearchResult(

    /// <summary>
    /// The Open Library Key of the entry.
    /// </summary>
    string Key,

    /// <summary>
    /// The title of the work.
    /// </summary
    string Title,

    /// <summary>
    /// The authors of the work.
    /// </summary>
    List<string> Authors,

    /// <summary>
    /// The International Standard Book Number (ISBN) of the work.
    /// </summary>
    string? PrimaryIsbn,

    /// <summary>
    /// The languages the work is availalbe in.
    /// </summary>
    List<string> Languages,

    /// <summary>
    /// The subject of the work.
    /// </summary>
    List<string> Subjects,

    /// <summary>
    /// The URL in Open Library the work can be found.
    /// </summary>
    string OpenLibraryUrl);
