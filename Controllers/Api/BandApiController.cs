using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using Riffdle.Data;
using Riffdle.Models.Domain;
using Riffdle.Models.DTO;

namespace Riffdle.Controllers.Api;

[Route("api/band")]
[ApiController]
public class BandApiController : ControllerBase
{
    private readonly RiffdleDbContext _db;

    public BandApiController(RiffdleDbContext db)
    {
        _db = db;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<BandDTO>>> Get([FromQuery] string? q, [FromQuery] int? genreId)
    {
        var query = _db.Bands
            .Include(b => b.Genre)
            .AsNoTracking();

        if (!string.IsNullOrWhiteSpace(q))
        {
            query = query.Where(b =>
                b.Name.Contains(q) ||
                b.Country.Contains(q) ||
                b.Description.Contains(q) ||
                (b.Genre != null && b.Genre.Name.Contains(q)));
        }

        if (genreId.HasValue)
        {
            query = query.Where(b => b.GenreId == genreId.Value);
        }

        var items = await query
            .Select(b => new BandDTO
            {
                Id = b.Id,
                Name = b.Name,
                FormedYear = b.FormedYear,
                Country = b.Country,
                Description = b.Description,
                GenreId = b.GenreId,
                GenreName = b.Genre != null ? b.Genre.Name : string.Empty
            })
            .ToListAsync();
        return Ok(items);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<BandDTO>> Get(int id)
    {
        var b = await _db.Bands.Include(x => x.Genre).FirstOrDefaultAsync(x => x.Id == id);
        if (b == null) return NotFound();
        return Ok(new BandDTO
        {
            Id = b.Id,
            Name = b.Name,
            FormedYear = b.FormedYear,
            Country = b.Country,
            Description = b.Description,
            GenreId = b.GenreId,
            GenreName = b.Genre?.Name ?? string.Empty
        });
    }

    [Authorize(Roles = "Admin")]
    [HttpPost]
    public async Task<ActionResult<BandDTO>> Post([FromBody] BandDTO model)
    {
        if (!ModelState.IsValid) return BadRequest(ModelState);
        var band = new Band
        {
            Name = model.Name,
            FormedYear = model.FormedYear,
            Country = model.Country,
            Description = model.Description,
            GenreId = model.GenreId
        };
        _db.Bands.Add(band);
        await _db.SaveChangesAsync();
        model.Id = band.Id;
        return CreatedAtAction(nameof(Get), new { id = model.Id }, model);
    }

    [Authorize(Roles = "Admin")]
    [HttpPut("{id}")]
    public async Task<ActionResult<BandDTO>> Put(int id, [FromBody] BandDTO model)
    {
        if (id != model.Id) return BadRequest();
        var band = await _db.Bands.FindAsync(id);
        if (band == null) return NotFound();
        band.Name = model.Name;
        band.FormedYear = model.FormedYear;
        band.Country = model.Country;
        band.Description = model.Description;
        band.GenreId = model.GenreId;
        await _db.SaveChangesAsync();
        return Ok(model);
    }

    [Authorize(Roles = "Admin")]
    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        var band = await _db.Bands.FindAsync(id);
        if (band == null) return NotFound();
        _db.Bands.Remove(band);
        await _db.SaveChangesAsync();
        return NoContent();
    }
}
