// Take home project for Matthew Maffett

using System.Text;
using BookSearchApi.Authenticators;
using BookSearchApi.Data;
using BookSearchApi.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();

// Set to use the JWT auth class
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options => JwtAuthentication.Configure(options, builder.Configuration));

builder.Services.AddAuthorization();

builder.Services.AddScoped<IFavoriteBooksService, FavoriteBooksService>();
builder.Services.AddScoped<IUserService, UserService>();

builder.Services.AddDbContext<FavoriteBooksContext>(options =>
    options.UseInMemoryDatabase("FavoriteBooksDb"));

// Register HttpClient and OpenLibrary service
builder.Services.AddHttpClient<IOpenLibraryService, OpenLibraryService>(client =>
{
    client.BaseAddress = new Uri("https://openlibrary.org");
    client.DefaultRequestHeaders.Add("Accept", "application/json");
});

var app = builder.Build();

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

app.Run();
