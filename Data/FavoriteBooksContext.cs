// Take home project for Matthew Maffett
using BookSearchApi.Models;
using Microsoft.EntityFrameworkCore;

namespace BookSearchApi.Data;

public class FavoriteBooksContext : DbContext
{
    public FavoriteBooksContext(DbContextOptions<FavoriteBooksContext> options)
        : base(options)
    {
    }

    public DbSet<OpenLibraryBook> FavoriteBooks { get; set; }
}
