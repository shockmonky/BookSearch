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

builder.Services.AddScoped<IFavoritesService, FavoritesService>();
builder.Services.AddScoped<IUserService, UserService>();

builder.Services.AddDbContext<FavoriteBooksContext>(options =>
    options.UseInMemoryDatabase("FavoriteBooksDb"));

// Use HttpClientFactory and register the OpenLibrary api
builder.Services.AddHttpClient<IOpenLibraryService, OpenLibraryService>(client =>
{
    client.BaseAddress = new Uri("OpenLibrary:BaseUrl");
    client.DefaultRequestHeaders.Add("Accept", "application/json");
});

// Use HttpClientFactory and register the Google Gemini api
builder.Services.AddHttpClient<IGeminiService, GeminiService>(client =>
{
    client.BaseAddress = new Uri(builder.Configuration["Gemini:BaseUrl"]!);
    client.DefaultRequestHeaders.Add("Accept", "application/json");
});

var app = builder.Build();

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

app.Run();
