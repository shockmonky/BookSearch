// Take home project for Matthew Maffett

using System.Text;
using BookSearchApi.Data;
using BookSearchApi.Models;
using BookSearchApi.Services;
using Microsoft.AspNetCore.Mvc;

namespace BookSearchApi.Controllers;

[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
public class BooksFavoriteController() : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<List<OpenLibraryBook>>> GetAll()
    {
        return new List<OpenLibraryBook>(0);
    }

    [HttpPost]
    public async Task<ActionResult<FavoriteBook>> Add(FavoriteBook book)
    {
        return this.CreatedAtAction("filler", book);
    }
}
