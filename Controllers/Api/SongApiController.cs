using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using Riffdle.Data;
using Riffdle.Models.Domain;
using Riffdle.Models.DTO;

namespace Riffdle.Controllers.Api;

[Route("api/song")]
[ApiController]
public class SongApiController : ControllerBase
{
    private readonly RiffdleDbContext _db;

    public SongApiController(RiffdleDbContext db)
    {
        _db = db;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<SongDTO>>> Get([FromQuery] string? q, [FromQuery] int? albumId)
    {
        var query = _db.Songs
            .Include(s => s.Album)
            .AsNoTracking();

        if (!string.IsNullOrWhiteSpace(q))
        {
            query = query.Where(s =>
                s.Title.Contains(q) ||
                s.OpeningLyric.Contains(q) ||
                (s.Album != null && s.Album.Title.Contains(q)));
        }

        if (albumId.HasValue)
        {
            query = query.Where(s => s.AlbumId == albumId.Value);
        }

        var items = await query
            .Select(s => new SongDTO
            {
                Id = s.Id,
                Title = s.Title,
                DurationSeconds = s.DurationSeconds,
                AlbumId = s.AlbumId,
                AlbumTitle = s.Album != null ? s.Album.Title : string.Empty,
                OpeningLyric = s.OpeningLyric,
                IsDailyQuizSong = s.IsDailyQuizSong,
                AudioSnippetUrl = s.AudioSnippetUrl,
                AlbumCoverUrl = s.AlbumCoverUrl
            })
            .ToListAsync();
        return Ok(items);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<SongDTO>> Get(int id)
    {
        var s = await _db.Songs.Include(x => x.Album).FirstOrDefaultAsync(x => x.Id == id);
        if (s == null) return NotFound();
        return Ok(new SongDTO
        {
            Id = s.Id,
            Title = s.Title,
            DurationSeconds = s.DurationSeconds,
            AlbumId = s.AlbumId,
            AlbumTitle = s.Album?.Title ?? string.Empty,
            OpeningLyric = s.OpeningLyric,
            IsDailyQuizSong = s.IsDailyQuizSong,
            AudioSnippetUrl = s.AudioSnippetUrl,
            AlbumCoverUrl = s.AlbumCoverUrl
        });
    }

    [Authorize(Roles = "Admin")]
    [HttpPost]
    public async Task<ActionResult<SongDTO>> Post([FromBody] SongDTO model)
    {
        if (!ModelState.IsValid) return BadRequest(ModelState);
        var song = new Song
        {
            Title = model.Title,
            DurationSeconds = model.DurationSeconds,
            AlbumId = model.AlbumId,
            OpeningLyric = model.OpeningLyric,
            IsDailyQuizSong = model.IsDailyQuizSong,
            AudioSnippetUrl = model.AudioSnippetUrl,
            AlbumCoverUrl = model.AlbumCoverUrl
        };
        _db.Songs.Add(song);
        await _db.SaveChangesAsync();
        model.Id = song.Id;
        return CreatedAtAction(nameof(Get), new { id = model.Id }, model);
    }

    [Authorize(Roles = "Admin")]
    [HttpPut("{id}")]
    public async Task<ActionResult<SongDTO>> Put(int id, [FromBody] SongDTO model)
    {
        if (id != model.Id) return BadRequest();
        var song = await _db.Songs.FindAsync(id);
        if (song == null) return NotFound();
        song.Title = model.Title;
        song.DurationSeconds = model.DurationSeconds;
        song.AlbumId = model.AlbumId;
        song.OpeningLyric = model.OpeningLyric;
        song.IsDailyQuizSong = model.IsDailyQuizSong;
        song.AudioSnippetUrl = model.AudioSnippetUrl;
        song.AlbumCoverUrl = model.AlbumCoverUrl;
        await _db.SaveChangesAsync();
        return Ok(model);
    }

    [Authorize(Roles = "Admin")]
    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        var song = await _db.Songs.FindAsync(id);
        if (song == null) return NotFound();
        _db.Songs.Remove(song);
        await _db.SaveChangesAsync();
        return NoContent();
    }
}
