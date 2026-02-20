// Take home project for Matthew Maffett

using BookSearchApi.Models;
using Microsoft.EntityFrameworkCore;

namespace BookSearchApi.Data;

public class FavoriteBooksContext(DbContextOptions<FavoriteBooksContext> options) : DbContext(options)
{
    public DbSet<User> Users { get; set; } = null!;

    public DbSet<FavoriteBook> FavoriteBooks { get; set; } = null!;

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<User>()
                .HasKey(u => u.Id);

        modelBuilder.Entity<User>()
            .HasMany(u => u.FavoriteBooks)
            .WithOne(b => b.User)
            .HasForeignKey(b => b.UserId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
