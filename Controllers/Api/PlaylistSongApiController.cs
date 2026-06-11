using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Riffdle.Data;
using Riffdle.Models.Domain;
using Riffdle.Models.DTO;

namespace Riffdle.Controllers.Api;

[Route("api/playlist-song")]
[ApiController]
public class PlaylistSongApiController : ControllerBase
{
    private readonly RiffdleDbContext _db;

    public PlaylistSongApiController(RiffdleDbContext db)
    {
        _db = db;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<PlaylistSongDTO>>> Get([FromQuery] string? q, [FromQuery] int? playlistId, [FromQuery] int? songId)
    {
        var query = _db.PlaylistSongs
            .Include(ps => ps.Playlist)
            .Include(ps => ps.Song)
            .AsNoTracking();

        if (!string.IsNullOrWhiteSpace(q))
        {
            query = query.Where(ps =>
                (ps.Playlist != null && ps.Playlist.Name.Contains(q)) ||
                (ps.Song != null && ps.Song.Title.Contains(q)));
        }

        if (playlistId.HasValue)
        {
            query = query.Where(ps => ps.PlaylistId == playlistId.Value);
        }

        if (songId.HasValue)
        {
            query = query.Where(ps => ps.SongId == songId.Value);
        }

        var items = await query
            .OrderBy(ps => ps.PlaylistId)
            .ThenBy(ps => ps.AddedAt)
            .ToListAsync();

        return Ok(items.Select(ToDTO).ToList());
    }

    [HttpGet("{playlistId:int}/{songId:int}")]
    public async Task<ActionResult<PlaylistSongDTO>> Get(int playlistId, int songId)
    {
        var item = await _db.PlaylistSongs
            .Include(ps => ps.Playlist)
            .Include(ps => ps.Song)
            .FirstOrDefaultAsync(ps => ps.PlaylistId == playlistId && ps.SongId == songId);

        if (item == null) return NotFound();
        return Ok(ToDTO(item));
    }

    [Authorize(Roles = "Admin")]
    [HttpPost]
    public async Task<ActionResult<PlaylistSongDTO>> Post([FromBody] PlaylistSongDTO model)
    {
        if (!ModelState.IsValid) return BadRequest(ModelState);
        if (!await _db.UserPlaylists.AnyAsync(p => p.Id == model.PlaylistId)) return BadRequest("Playlist does not exist.");
        if (!await _db.Songs.AnyAsync(s => s.Id == model.SongId)) return BadRequest("Song does not exist.");

        var exists = await _db.PlaylistSongs.AnyAsync(ps => ps.PlaylistId == model.PlaylistId && ps.SongId == model.SongId);
        if (exists) return Conflict();

        var item = new PlaylistSong
        {
            PlaylistId = model.PlaylistId,
            SongId = model.SongId,
            AddedAt = model.AddedAt == default ? DateTime.UtcNow : model.AddedAt
        };

        _db.PlaylistSongs.Add(item);
        await _db.SaveChangesAsync();

        model.AddedAt = item.AddedAt;
        return CreatedAtAction(nameof(Get), new { playlistId = model.PlaylistId, songId = model.SongId }, model);
    }

    [Authorize(Roles = "Admin")]
    [HttpPut("{playlistId:int}/{songId:int}")]
    public async Task<ActionResult<PlaylistSongDTO>> Put(int playlistId, int songId, [FromBody] PlaylistSongDTO model)
    {
        if (playlistId != model.PlaylistId || songId != model.SongId) return BadRequest();
        if (!ModelState.IsValid) return BadRequest(ModelState);

        var item = await _db.PlaylistSongs.FindAsync(playlistId, songId);
        if (item == null) return NotFound();

        item.AddedAt = model.AddedAt == default ? item.AddedAt : model.AddedAt;
        await _db.SaveChangesAsync();

        return Ok(model);
    }

    [Authorize(Roles = "Admin")]
    [HttpDelete("{playlistId:int}/{songId:int}")]
    public async Task<IActionResult> Delete(int playlistId, int songId)
    {
        var item = await _db.PlaylistSongs.FindAsync(playlistId, songId);
        if (item == null) return NotFound();
        _db.PlaylistSongs.Remove(item);
        await _db.SaveChangesAsync();
        return NoContent();
    }

    private static PlaylistSongDTO ToDTO(PlaylistSong item)
    {
        return new PlaylistSongDTO
        {
            PlaylistId = item.PlaylistId,
            PlaylistName = item.Playlist?.Name ?? string.Empty,
            SongId = item.SongId,
            SongTitle = item.Song?.Title ?? string.Empty,
            AddedAt = item.AddedAt
        };
    }
}
