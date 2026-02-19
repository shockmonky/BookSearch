# BookSearchApi

A .NET 10 Web API for searching books using the [Open Library Search API](https://openlibrary.org/developers/api).

## Requirements

- [.NET 10 SDK](https://dotnet.microsoft.com/download/dotnet/10.0)

## Getting Started

```bash
cd BookSearchApi
dotnet run
```

The API will be available at `https://localhost:5001` (or `http://localhost:5000`).  
Swagger UI: `https://localhost:5001/swagger`

---

## Endpoints

### `GET /api/books/search`

Search for books by title.

#### Query Parameters

| Parameter  | Type   | Required | Default | Description                          |
|------------|--------|----------|---------|--------------------------------------|
| `q`        | string | ✅ Yes   | —       | Book title or keyword to search for  |
| `page`     | int    | No       | `1`     | Page number (1-based)                |
| `pageSize` | int    | No       | `10`    | Results per page (max: 100)          |

#### Example Request

```
GET /api/books/search?q=the+great+gatsby&page=1&pageSize=5
```

#### Example Response

```json
{
  "totalResults": 142,
  "page": 1,
  "pageSize": 5,
  "results": [
    {
      "key": "/works/OL468431W",
      "title": "The Great Gatsby",
      "authors": ["F. Scott Fitzgerald"],
      "firstPublishYear": 1925,
      "primaryIsbn": "9780743273565",
      "publishers": ["Scribner"],
      "languages": ["eng"],
      "subjects": ["Fiction", "American literature", "Social classes"],
      "coverUrl": "https://covers.openlibrary.org/b/id/8410894-M.jpg",
      "editionCount": 564,
      "pageCount": 180,
      "openLibraryUrl": "https://openlibrary.org/works/OL468431W"
    }
  ]
}
```

---

## Project Structure

```
BookSearchApi/
├── Controllers/
│   └── BooksController.cs      # API endpoint definitions
├── Models/
│   └── BookModels.cs           # Request/response models
├── Services/
│   ├── IOpenLibraryService.cs  # Service interface
│   └── OpenLibraryService.cs  # Open Library API integration
├── Program.cs                  # App setup and DI registration
├── appsettings.json
└── BookSearchApi.csproj
```
