// Take home project for Matthew Maffett

using BookSearchApi.Data;
using BookSearchApi.Services;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddScoped<IFavoriteBooksService, FavoriteBooksService>();

builder.Services.AddDbContext<FavoriteBooksContext>(options =>
    options.UseInMemoryDatabase("FavoriteBooksDb"));

// Register HttpClient and OpenLibrary service
builder.Services.AddHttpClient<IOpenLibraryService, OpenLibraryService>(client =>
{
    client.BaseAddress = new Uri("https://openlibrary.org");
    client.DefaultRequestHeaders.Add("Accept", "application/json");
});

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

app.Run();
