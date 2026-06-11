using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Riffdle.Data;
using Riffdle.Models.Domain;
using Riffdle.Models.DTO;

namespace Riffdle.Controllers.Api;

[Route("api/quiz-round")]
[ApiController]
public class QuizRoundApiController : ControllerBase
{
    private readonly RiffdleDbContext _db;

    public QuizRoundApiController(RiffdleDbContext db)
    {
        _db = db;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<QuizRoundDTO>>> Get([FromQuery] string? q, [FromQuery] int? songId)
    {
        var query = _db.QuizRounds
            .Include(r => r.Song)
            .Include(r => r.Hints)
            .AsNoTracking();

        if (!string.IsNullOrWhiteSpace(q))
        {
            query = query.Where(r =>
                (r.Song != null && r.Song.Title.Contains(q)) ||
                r.Hints.Any(h => h.Content.Contains(q)));
        }

        if (songId.HasValue)
        {
            query = query.Where(r => r.SongId == songId.Value);
        }

        var rounds = await query
            .OrderBy(r => r.Id)
            .ToListAsync();

        return Ok(rounds.Select(ToDTO).ToList());
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<QuizRoundDTO>> Get(int id)
    {
        var round = await _db.QuizRounds
            .Include(r => r.Song)
            .Include(r => r.Hints)
            .FirstOrDefaultAsync(r => r.Id == id);

        if (round == null) return NotFound();
        return Ok(ToDTO(round));
    }

    [Authorize(Roles = "Admin")]
    [HttpPost]
    public async Task<ActionResult<QuizRoundDTO>> Post([FromBody] QuizRoundDTO model)
    {
        if (!ModelState.IsValid) return BadRequest(ModelState);
        if (!await _db.Songs.AnyAsync(s => s.Id == model.SongId)) return BadRequest("Song does not exist.");

        var round = new QuizRound { SongId = model.SongId };
        _db.QuizRounds.Add(round);
        await _db.SaveChangesAsync();

        model.Id = round.Id;
        return CreatedAtAction(nameof(Get), new { id = model.Id }, model);
    }

    [Authorize(Roles = "Admin")]
    [HttpPut("{id}")]
    public async Task<ActionResult<QuizRoundDTO>> Put(int id, [FromBody] QuizRoundDTO model)
    {
        if (id != model.Id) return BadRequest();
        if (!ModelState.IsValid) return BadRequest(ModelState);

        var round = await _db.QuizRounds.FindAsync(id);
        if (round == null) return NotFound();
        if (!await _db.Songs.AnyAsync(s => s.Id == model.SongId)) return BadRequest("Song does not exist.");

        round.SongId = model.SongId;
        await _db.SaveChangesAsync();

        return Ok(model);
    }

    [Authorize(Roles = "Admin")]
    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        var round = await _db.QuizRounds.FindAsync(id);
        if (round == null) return NotFound();
        _db.QuizRounds.Remove(round);
        await _db.SaveChangesAsync();
        return NoContent();
    }

    private static QuizRoundDTO ToDTO(QuizRound round)
    {
        return new QuizRoundDTO
        {
            Id = round.Id,
            SongId = round.SongId,
            SongTitle = round.Song?.Title ?? string.Empty,
            Hints = round.Hints
                .OrderBy(h => h.Order)
                .Select(h => new HintDTO
                {
                    Id = h.Id,
                    QuizRoundId = h.QuizRoundId,
                    Type = h.Type,
                    Order = h.Order,
                    Content = h.Content
                })
                .ToList()
        };
    }
}
