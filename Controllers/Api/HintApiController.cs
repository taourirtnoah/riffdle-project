using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Riffdle.Data;
using Riffdle.Models.Domain;
using Riffdle.Models.DTO;

namespace Riffdle.Controllers.Api;

[Route("api/hint")]
[ApiController]
public class HintApiController : ControllerBase
{
    private readonly RiffdleDbContext _db;

    public HintApiController(RiffdleDbContext db)
    {
        _db = db;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<HintDTO>>> Get([FromQuery] string? q, [FromQuery] int? quizRoundId)
    {
        var query = _db.Hints.AsNoTracking();

        if (!string.IsNullOrWhiteSpace(q))
        {
            query = query.Where(h => h.Content.Contains(q));
        }

        if (quizRoundId.HasValue)
        {
            query = query.Where(h => h.QuizRoundId == quizRoundId.Value);
        }

        var hints = await query
            .OrderBy(h => h.QuizRoundId)
            .ThenBy(h => h.Order)
            .ToListAsync();

        return Ok(hints.Select(ToDTO).ToList());
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<HintDTO>> Get(int id)
    {
        var hint = await _db.Hints.FindAsync(id);
        if (hint == null) return NotFound();
        return Ok(ToDTO(hint));
    }

    [Authorize(Roles = "Admin")]
    [HttpPost]
    public async Task<ActionResult<HintDTO>> Post([FromBody] HintDTO model)
    {
        if (!ModelState.IsValid) return BadRequest(ModelState);
        if (!await _db.QuizRounds.AnyAsync(q => q.Id == model.QuizRoundId)) return BadRequest("Quiz round does not exist.");

        var hint = new Hint
        {
            QuizRoundId = model.QuizRoundId,
            Type = model.Type,
            Order = model.Order,
            Content = model.Content
        };

        _db.Hints.Add(hint);
        await _db.SaveChangesAsync();

        model.Id = hint.Id;
        return CreatedAtAction(nameof(Get), new { id = model.Id }, model);
    }

    [Authorize(Roles = "Admin")]
    [HttpPut("{id}")]
    public async Task<ActionResult<HintDTO>> Put(int id, [FromBody] HintDTO model)
    {
        if (id != model.Id) return BadRequest();
        if (!ModelState.IsValid) return BadRequest(ModelState);

        var hint = await _db.Hints.FindAsync(id);
        if (hint == null) return NotFound();
        if (!await _db.QuizRounds.AnyAsync(q => q.Id == model.QuizRoundId)) return BadRequest("Quiz round does not exist.");

        hint.QuizRoundId = model.QuizRoundId;
        hint.Type = model.Type;
        hint.Order = model.Order;
        hint.Content = model.Content;

        await _db.SaveChangesAsync();
        return Ok(model);
    }

    [Authorize(Roles = "Admin")]
    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        var hint = await _db.Hints.FindAsync(id);
        if (hint == null) return NotFound();
        _db.Hints.Remove(hint);
        await _db.SaveChangesAsync();
        return NoContent();
    }

    private static HintDTO ToDTO(Hint hint)
    {
        return new HintDTO
        {
            Id = hint.Id,
            QuizRoundId = hint.QuizRoundId,
            Type = hint.Type,
            Order = hint.Order,
            Content = hint.Content
        };
    }
}
