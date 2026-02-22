# BookSearchApi

A service for searching the Open Library Api for books, storing them in a user favorites list, and getting a summary of a given list of books.

## Requirements

- [.NET 10 SDK](https://dotnet.microsoft.com/en-us/download/dotnet/10.0)
- [Docker](https://www.docker.com/) (optional)

## Getting Started

### Building

To build both unit tests and executable go to the root directory and run:

```bash
dotnet build
```

### Running Tests

In the root directory run:

```bash
dotnet test
```

### Environment Variables

The following secrets must be set as environment variables before running:

```powershell
$env:Jwt__Key = "your-secret-key-at-least-32-characters"
$env:Gemini__ApiKey = "your-gemini-api-key"
```

### Running Locally

```bash
cd BookSearchApi
dotnet run
```

### Running with Docker

The https endpoint must be removed from the appsettings when running the docker container due to a lack of certificates. The https endpoint can be used
once valid certificates (not developer certificates) are available for the service.

```bash
docker run -p 5000:5000 `
  -e Jwt__Key="your-secret-key" `
  -e Gemini__ApiKey="your-gemini-api-key" `
  -e Kestrel__Endpoints__Https__Url="" `
  booksearchapi
```

---

## Authentication

All endpoints require a JWT token. Token provided in delivered materials.


### Using the Token

Add the token to the `Authorization` header of every request:

| Header Key | Header Value |
|---|---|
| `Authorization` | `Bearer eyJhbGci...` |

---

## Endpoints

### Book Search

#### Search by Title

```
GET /https://localhost:5001/api/book/query?title={bookName}&limit={limit}
```

| Parameter | Type | Required | Default | Description |
|---|---|---|---|---|
| `title` | string | Yes | — | The book title to search for |
| `limit` | int | No | 100 | Number of results to return (max 100) |

**Example:**
```
GET https://localhost:5001/api/book/query?title=Winnie the Pooh&limit=3
```

#### Search by Subject

```
GET /api/book/querysubject?subjectName={subjectName}
```

| Parameter | Type | Required | Default | Description |
|---|---|---|---|---|
| `subjectName` | string | Yes | — | The subject to search for |

**Example:**
```
GET https://localhost:5001/api/book/querysubject?subjectName=horror
```

---

### Users

#### Create a User

```
POST /api/user?name={name}
```

| Parameter | Type | Required | Description |
|---|---|---|---|
| `name` | string | Yes | The name of the user |

**Example:**
```
POST https://localhost:5001/api/user?name=Matthew
```

#### Get All Users

```
GET /api/user/all
```

**Example:**
```
GET https://localhost:5001/api/user/all
```
---

### Favourite Books

#### Get All Favourite Books for a User

```
GET /api/booksfavorite/{userId}
```

| Parameter | Type | Required | Description |
|---|---|---|---|
| `userId` | Guid | Yes | The ID of the user |

**Example:**
The provided UserId will not work as userId is a new Guid for each User added to the syste.
```
GET https://localhost:5001/api/favorites/3fa85f64-5717-4562-b3fc-2c963f66afa6
```

#### Add a Favourite Book

```
POST /api/favorites?userId={userId}&key={key}
```

| Parameter | Type | Required | Description |
|---|---|---|---|
| `userId` | Guid | Yes | The ID of the user |
| `key` | string | Yes | The Open Library key for the book e.g. `/works/OL468431W` |

**Example:**
```
POST https://localhost:5001/api/favorites?userId=3fa85f64-5717-4562-b3fc-2c963f66afa6&key=/works/OL468431W
```

#### Remove a Favourite Book

```
DELETE /api/favorites?userId={userId}&key={key}
```

| Parameter | Type | Required | Description |
|---|---|---|---|
| `userId` | Guid | Yes | The ID of the user |
| `key` | string | Yes | The Open Library key of the book to remove |

**Example:**
```
DELETE https://localhost:5001/api/favorite?userId=3fa85f64-5717-4562-b3fc-2c963f66afa6&key=/works/OL468431W
```

---

### Summarize

#### Summarize Search Results

```
POST /api/summarize
```

**Body:** The body of the request should be a json list with each json entry having a list of authors and a title.
Example:
```
[
    {
        "title": "A book title",
        "authors": [
            "Jon Doe",
            "Jane Doe"
        ]
    }
]
```

**Example:**
```
POST https://localhost:5001/api/summarize
```
---

### Code Coverage

```bash
dotnet test --collect:"XPlat Code Coverage" --results-directory ./coverage
reportgenerator -reports:"./coverage/**/coverage.cobertura.xml" -targetdir:"./coverage/report" -reporttypes:Html
```

Then open `./coverage/report/index.html` in your browser.

---

## CI/CD

The project uses GitHub Actionsfor continuous integration. The pipeline runs on every push to any
branch and on pull requests targeting `main` or `develop`. It performs the following steps:

- Restore dependencies
- Build the solution
- Run all unit tests with code coverage
- Generate and publish a coverage summary to the GitHub Actions job summary page

The pipeline for this project can be found [here](https://github.com/shockmonky/BookSearch/actions)
