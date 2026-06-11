using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Riffdle.Data;
using Riffdle.Models.Domain;
using Riffdle.Models.DTO;

namespace Riffdle.Controllers.Api;

[Route("api/playlist")]
[ApiController]
public class UserPlaylistApiController : ControllerBase
{
    private readonly RiffdleDbContext _db;

    public UserPlaylistApiController(RiffdleDbContext db)
    {
        _db = db;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<UserPlaylistDTO>>> Get([FromQuery] string? q, [FromQuery] bool? isPublic)
    {
        var query = _db.UserPlaylists
            .Include(p => p.PlaylistSongs)
            .ThenInclude(ps => ps.Song)
            .AsNoTracking();

        if (!string.IsNullOrWhiteSpace(q))
        {
            query = query.Where(p =>
                p.Name.Contains(q) ||
                p.OwnerUserName.Contains(q) ||
                p.Description.Contains(q) ||
                p.PlaylistSongs.Any(ps => ps.Song != null && ps.Song.Title.Contains(q)));
        }

        if (isPublic.HasValue)
        {
            query = query.Where(p => p.IsPublic == isPublic.Value);
        }

        var playlists = await query
            .OrderBy(p => p.Name)
            .ToListAsync();

        return Ok(playlists.Select(ToDTO).ToList());
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<UserPlaylistDTO>> Get(int id)
    {
        var playlist = await _db.UserPlaylists
            .Include(p => p.PlaylistSongs)
            .ThenInclude(ps => ps.Song)
            .FirstOrDefaultAsync(p => p.Id == id);

        if (playlist == null) return NotFound();
        return Ok(ToDTO(playlist));
    }

    [Authorize(Roles = "Admin")]
    [HttpPost]
    public async Task<ActionResult<UserPlaylistDTO>> Post([FromBody] UserPlaylistDTO model)
    {
        if (!ModelState.IsValid) return BadRequest(ModelState);

        var playlist = new UserPlaylist
        {
            Name = model.Name,
            OwnerUserName = model.OwnerUserName,
            Description = model.Description,
            CreatedAt = model.CreatedAt == default ? DateTime.UtcNow : model.CreatedAt,
            IsPublic = model.IsPublic,
            Likes = model.Likes
        };

        _db.UserPlaylists.Add(playlist);
        await _db.SaveChangesAsync();

        model.Id = playlist.Id;
        model.CreatedAt = playlist.CreatedAt;
        return CreatedAtAction(nameof(Get), new { id = model.Id }, model);
    }

    [Authorize(Roles = "Admin")]
    [HttpPut("{id}")]
    public async Task<ActionResult<UserPlaylistDTO>> Put(int id, [FromBody] UserPlaylistDTO model)
    {
        if (id != model.Id) return BadRequest();
        if (!ModelState.IsValid) return BadRequest(ModelState);

        var playlist = await _db.UserPlaylists.FindAsync(id);
        if (playlist == null) return NotFound();

        playlist.Name = model.Name;
        playlist.OwnerUserName = model.OwnerUserName;
        playlist.Description = model.Description;
        playlist.CreatedAt = model.CreatedAt == default ? playlist.CreatedAt : model.CreatedAt;
        playlist.IsPublic = model.IsPublic;
        playlist.Likes = model.Likes;

        await _db.SaveChangesAsync();
        return Ok(model);
    }

    [Authorize(Roles = "Admin")]
    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        var playlist = await _db.UserPlaylists.FindAsync(id);
        if (playlist == null) return NotFound();
        _db.UserPlaylists.Remove(playlist);
        await _db.SaveChangesAsync();
        return NoContent();
    }

    private static UserPlaylistDTO ToDTO(UserPlaylist playlist)
    {
        return new UserPlaylistDTO
        {
            Id = playlist.Id,
            Name = playlist.Name,
            OwnerUserName = playlist.OwnerUserName,
            Description = playlist.Description,
            CreatedAt = playlist.CreatedAt,
            IsPublic = playlist.IsPublic,
            Likes = playlist.Likes,
            Songs = playlist.PlaylistSongs
                .OrderBy(ps => ps.AddedAt)
                .Select(ps => new PlaylistSongDTO
                {
                    PlaylistId = ps.PlaylistId,
                    PlaylistName = playlist.Name,
                    SongId = ps.SongId,
                    SongTitle = ps.Song?.Title ?? string.Empty,
                    AddedAt = ps.AddedAt
                })
                .ToList()
        };
    }
}
