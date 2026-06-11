using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Riffdle.Data;
using Riffdle.Models.Domain;
using Riffdle.Models.DTO;

namespace Riffdle.Controllers.Api;

[Route("api/attachment")]
[ApiController]
public class AttachmentApiController : ControllerBase
{
    private readonly RiffdleDbContext _db;

    public AttachmentApiController(RiffdleDbContext db)
    {
        _db = db;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<AttachmentDTO>>> Get([FromQuery] string? q, [FromQuery] int? songId, [FromQuery] int? quizRoundId)
    {
        var query = _db.Attachments
            .Include(a => a.Song)
            .Include(a => a.QuizRound)
            .AsNoTracking();

        if (!string.IsNullOrWhiteSpace(q))
        {
            query = query.Where(a =>
                a.OriginalName.Contains(q) ||
                a.FileName.Contains(q) ||
                a.ContentType.Contains(q) ||
                (a.Song != null && a.Song.Title.Contains(q)));
        }

        if (songId.HasValue)
        {
            query = query.Where(a => a.SongId == songId.Value);
        }

        if (quizRoundId.HasValue)
        {
            query = query.Where(a => a.QuizRoundId == quizRoundId.Value);
        }

        var attachments = await query
            .OrderByDescending(a => a.CreatedAt)
            .ToListAsync();

        return Ok(attachments.Select(ToDTO).ToList());
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<AttachmentDTO>> Get(int id)
    {
        var attachment = await _db.Attachments
            .Include(a => a.Song)
            .Include(a => a.QuizRound)
            .FirstOrDefaultAsync(a => a.Id == id);
        if (attachment == null) return NotFound();
        return Ok(ToDTO(attachment));
    }

    [Authorize(Roles = "Admin")]
    [HttpPost]
    public async Task<ActionResult<AttachmentDTO>> Post([FromBody] AttachmentDTO model)
    {
        if (!ModelState.IsValid) return BadRequest(ModelState);
        if (!await _db.Songs.AnyAsync(s => s.Id == model.SongId)) return BadRequest("Song does not exist.");

        var quizRound = await ResolveQuizRoundAsync(model);
        if (quizRound == null) return BadRequest("Quiz round does not exist for the selected song.");

        var attachment = new Attachment
        {
            SongId = model.SongId,
            QuizRoundId = quizRound.Id,
            FileName = model.FileName,
            OriginalName = model.OriginalName,
            ContentType = model.ContentType,
            Size = model.Size,
            Url = model.Url,
            CreatedAt = model.CreatedAt == default ? DateTime.UtcNow : model.CreatedAt
        };

        _db.Attachments.Add(attachment);
        await _db.SaveChangesAsync();

        model.Id = attachment.Id;
        model.CreatedAt = attachment.CreatedAt;
        model.QuizRoundId = attachment.QuizRoundId;
        return CreatedAtAction(nameof(Get), new { id = model.Id }, model);
    }

    [Authorize(Roles = "Admin")]
    [HttpPut("{id}")]
    public async Task<ActionResult<AttachmentDTO>> Put(int id, [FromBody] AttachmentDTO model)
    {
        if (id != model.Id) return BadRequest();
        if (!ModelState.IsValid) return BadRequest(ModelState);

        var attachment = await _db.Attachments.FindAsync(id);
        if (attachment == null) return NotFound();
        if (!await _db.Songs.AnyAsync(s => s.Id == model.SongId)) return BadRequest("Song does not exist.");

        var quizRound = await ResolveQuizRoundAsync(model);
        if (quizRound == null) return BadRequest("Quiz round does not exist for the selected song.");

        attachment.SongId = model.SongId;
        attachment.QuizRoundId = quizRound.Id;
        attachment.FileName = model.FileName;
        attachment.OriginalName = model.OriginalName;
        attachment.ContentType = model.ContentType;
        attachment.Size = model.Size;
        attachment.Url = model.Url;
        attachment.CreatedAt = model.CreatedAt == default ? attachment.CreatedAt : model.CreatedAt;

        await _db.SaveChangesAsync();
        model.QuizRoundId = attachment.QuizRoundId;
        return Ok(model);
    }

    [Authorize(Roles = "Admin")]
    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        var attachment = await _db.Attachments.FindAsync(id);
        if (attachment == null) return NotFound();
        _db.Attachments.Remove(attachment);
        await _db.SaveChangesAsync();
        return NoContent();
    }

    private static AttachmentDTO ToDTO(Attachment attachment)
    {
        return new AttachmentDTO
        {
            Id = attachment.Id,
            SongId = attachment.SongId,
            SongTitle = attachment.Song?.Title ?? string.Empty,
            QuizRoundId = attachment.QuizRoundId,
            FileName = attachment.FileName,
            OriginalName = attachment.OriginalName,
            ContentType = attachment.ContentType,
            Size = attachment.Size,
            Url = attachment.Url,
            CreatedAt = attachment.CreatedAt
        };
    }

    private async Task<QuizRound?> ResolveQuizRoundAsync(AttachmentDTO model)
    {
        QuizRound? quizRound;

        if (model.QuizRoundId.HasValue)
        {
            quizRound = await _db.QuizRounds.FirstOrDefaultAsync(r => r.Id == model.QuizRoundId.Value);
            return quizRound?.SongId == model.SongId ? quizRound : null;
        }

        quizRound = await _db.QuizRounds.FirstOrDefaultAsync(r => r.SongId == model.SongId);
        if (quizRound != null)
        {
            return quizRound;
        }

        quizRound = new QuizRound { SongId = model.SongId };
        _db.QuizRounds.Add(quizRound);
        await _db.SaveChangesAsync();
        return quizRound;
    }
}
