using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using Riffdle.Data;
using Riffdle.Models.Domain;
using Riffdle.Models.DTO;

namespace Riffdle.Controllers.Api;

[Route("api/album")]
[ApiController]
public class AlbumApiController : ControllerBase
{
    private readonly RiffdleDbContext _db;

    public AlbumApiController(RiffdleDbContext db)
    {
        _db = db;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<AlbumDTO>>> Get([FromQuery] string? q, [FromQuery] int? bandId)
    {
        var query = _db.Albums
            .Include(a => a.Band)
            .AsNoTracking();

        if (!string.IsNullOrWhiteSpace(q))
        {
            query = query.Where(a =>
                a.Title.Contains(q) ||
                (a.Band != null && a.Band.Name.Contains(q)));
        }

        if (bandId.HasValue)
        {
            query = query.Where(a => a.BandId == bandId.Value);
        }

        var items = await query
            .Select(a => new AlbumDTO
            {
                Id = a.Id,
                Title = a.Title,
                ReleaseYear = a.ReleaseYear,
                BandId = a.BandId,
                BandName = a.Band != null ? a.Band.Name : string.Empty
            })
            .ToListAsync();
        return Ok(items);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<AlbumDTO>> Get(int id)
    {
        var a = await _db.Albums.Include(x => x.Band).FirstOrDefaultAsync(x => x.Id == id);
        if (a == null) return NotFound();
        return Ok(new AlbumDTO
        {
            Id = a.Id,
            Title = a.Title,
            ReleaseYear = a.ReleaseYear,
            BandId = a.BandId,
            BandName = a.Band?.Name ?? string.Empty
        });
    }

    [Authorize(Roles = "Admin")]
    [HttpPost]
    public async Task<ActionResult<AlbumDTO>> Post([FromBody] AlbumDTO model)
    {
        if (!ModelState.IsValid) return BadRequest(ModelState);
        var album = new Album
        {
            Title = model.Title,
            ReleaseYear = model.ReleaseYear,
            BandId = model.BandId
        };
        _db.Albums.Add(album);
        await _db.SaveChangesAsync();
        model.Id = album.Id;
        return CreatedAtAction(nameof(Get), new { id = model.Id }, model);
    }

    [Authorize(Roles = "Admin")]
    [HttpPut("{id}")]
    public async Task<ActionResult<AlbumDTO>> Put(int id, [FromBody] AlbumDTO model)
    {
        if (id != model.Id) return BadRequest();
        var album = await _db.Albums.FindAsync(id);
        if (album == null) return NotFound();
        album.Title = model.Title;
        album.ReleaseYear = model.ReleaseYear;
        album.BandId = model.BandId;
        await _db.SaveChangesAsync();
        return Ok(model);
    }

    [Authorize(Roles = "Admin")]
    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        var album = await _db.Albums.FindAsync(id);
        if (album == null) return NotFound();
        _db.Albums.Remove(album);
        await _db.SaveChangesAsync();
        return NoContent();
    }
}
