using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Riffdle.Data;
using Riffdle.Models.Domain;
using Riffdle.Models.DTO;

namespace Riffdle.Controllers.Api;

[Route("api/genre")]
[ApiController]
public class GenreApiController : ControllerBase
{
    private readonly RiffdleDbContext _db;

    public GenreApiController(RiffdleDbContext db)
    {
        _db = db;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<GenreDTO>>> Get()
    {
        var items = await _db.Genres.AsNoTracking()
            .Select(g => new GenreDTO { Id = g.Id, Name = g.Name, Description = g.Description })
            .ToListAsync();
        return Ok(items);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<GenreDTO>> Get(int id)
    {
        var g = await _db.Genres.FindAsync(id);
        if (g == null) return NotFound();
        return Ok(new GenreDTO { Id = g.Id, Name = g.Name, Description = g.Description });
    }

    [HttpPost]
    public async Task<ActionResult<GenreDTO>> Post([FromBody] GenreDTO model)
    {
        if (!ModelState.IsValid) return BadRequest(ModelState);
        var genre = new Genre { Name = model.Name, Description = model.Description };
        _db.Genres.Add(genre);
        await _db.SaveChangesAsync();
        model.Id = genre.Id;
        return CreatedAtAction(nameof(Get), new { id = model.Id }, model);
    }

    [HttpPut("{id}")]
    public async Task<ActionResult<GenreDTO>> Put(int id, [FromBody] GenreDTO model)
    {
        if (id != model.Id) return BadRequest();
        var genre = await _db.Genres.FindAsync(id);
        if (genre == null) return NotFound();
        genre.Name = model.Name;
        genre.Description = model.Description;
        await _db.SaveChangesAsync();
        return Ok(model);
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        var genre = await _db.Genres.FindAsync(id);
        if (genre == null) return NotFound();
        _db.Genres.Remove(genre);
        await _db.SaveChangesAsync();
        return NoContent();
    }
}
