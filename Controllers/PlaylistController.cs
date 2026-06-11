using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Riffdle.Data;
using Riffdle.Models.Domain;
using Riffdle.Models.ViewModels;

namespace Riffdle.Controllers;

public class PlaylistController : Controller
{
    private readonly IDbContextFactory<RiffdleDbContext> _dbContextFactory;
    private const string LikedPlaylistsCookieName = "riffdle-liked-playlists";

    public PlaylistController(IDbContextFactory<RiffdleDbContext> dbContextFactory)
    {
        _dbContextFactory = dbContextFactory;
    }

    [AllowAnonymous]
    [HttpGet("playlists")]
    [HttpGet("~/Playlist/Index")]
    public IActionResult Index()
    {
        using var context = _dbContextFactory.CreateDbContext();
        var likedPlaylistIds = ReadLikedPlaylistIds();

        var playlists = context.UserPlaylists
            .AsNoTracking()
            .Include(playlist => playlist.PlaylistSongs)
            .ThenInclude(playlistSong => playlistSong.Song)
            .ThenInclude(song => song!.Album)
            .ThenInclude(album => album!.Band)
            .OrderByDescending(playlist => playlist.IsPublic)
            .ThenByDescending(playlist => playlist.Likes)
            .ThenBy(playlist => playlist.Name)
            .ToList();

        ViewData["Breadcrumbs"] = new List<(string Label, string? Url)>
        {
            ("Home", Url.Action("Index", "Home")),
            ("Playlists", null)
        };

        return View(new PlaylistIndexViewModel
        {
            Playlists = playlists,
            LikedPlaylistIds = likedPlaylistIds
        });
    }

    private HashSet<int> ReadLikedPlaylistIds()
    {
        if (!Request.Cookies.TryGetValue(LikedPlaylistsCookieName, out var cookieValue) || string.IsNullOrWhiteSpace(cookieValue))
        {
            return new HashSet<int>();
        }

        var ids = cookieValue
            .Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
            .Select(value => int.TryParse(value, out var parsedId) ? parsedId : 0)
            .Where(parsedId => parsedId > 0)
            .ToHashSet();

        return ids;
    }

    private void PersistLikedPlaylistIds(HashSet<int> likedPlaylistIds)
    {
        var cookieValue = string.Join(',', likedPlaylistIds.OrderBy(id => id));
        Response.Cookies.Append(LikedPlaylistsCookieName, cookieValue, new CookieOptions
        {
            Expires = DateTimeOffset.UtcNow.AddYears(1),
            IsEssential = true,
            HttpOnly = true,
            SameSite = SameSiteMode.Lax
        });
    }

    [Authorize(Roles = "Admin")]
    [HttpGet("playlists/create")]
    [HttpGet("~/Playlist/Create")]
    public IActionResult Create()
    {
        ViewData["Breadcrumbs"] = new List<(string Label, string? Url)>
        {
            ("Home", Url.Action("Index", "Home")),
            ("Playlists", Url.Action("Index", "Playlist")),
            ("Create", null)
        };

        return View(new PlaylistFormViewModel());
    }

    private async Task PopulateEditModelAsync(PlaylistFormViewModel model, RiffdleDbContext context, int playlistId)
    {
        var existingSongIds = await context.PlaylistSongs
            .AsNoTracking()
            .Where(item => item.PlaylistId == playlistId)
            .Select(item => item.SongId)
            .ToListAsync();

        model.AvailableSongs = await context.Songs
            .AsNoTracking()
            .Include(song => song.Album)
            .ThenInclude(album => album!.Band)
            .Where(song => !existingSongIds.Contains(song.Id))
            .OrderBy(song => song.Title)
            .ToListAsync();

        model.PlaylistSongs = await context.PlaylistSongs
            .AsNoTracking()
            .Where(item => item.PlaylistId == playlistId)
            .Include(item => item.Song)
            .ThenInclude(song => song!.Album)
            .ThenInclude(album => album!.Band)
            .OrderBy(item => item.AddedAt)
            .ToListAsync();
    }

    [Authorize(Roles = "Admin")]
    [HttpPost("playlists/create")]
    [HttpPost("~/Playlist/Create")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(PlaylistFormViewModel model)
    {
        if (!ModelState.IsValid)
        {
            ViewData["Breadcrumbs"] = new List<(string Label, string? Url)>
            {
                ("Home", Url.Action("Index", "Home")),
                ("Playlists", Url.Action("Index", "Playlist")),
                ("Create", null)
            };

            return View(model);
        }

        await using var context = await _dbContextFactory.CreateDbContextAsync();
        var playlist = new UserPlaylist
        {
            Name = model.Name.Trim(),
            OwnerUserName = model.OwnerUserName.Trim(),
            Description = model.Description.Trim(),
            CreatedAt = DateTime.UtcNow,
            IsPublic = model.IsPublic,
            Likes = 0
        };

        context.UserPlaylists.Add(playlist);
        await context.SaveChangesAsync();

        return RedirectToAction(nameof(Index));
    }

    [Authorize(Roles = "Admin")]
    [HttpGet("playlists/{id:int}/edit")]
    [HttpGet("~/Playlist/Edit/{id:int}")]
    public async Task<IActionResult> Edit(int id)
    {
        await using var context = await _dbContextFactory.CreateDbContextAsync();
        var playlist = await context.UserPlaylists.AsNoTracking().FirstOrDefaultAsync(item => item.Id == id);
        if (playlist is null)
        {
            return NotFound();
        }

        var model = new PlaylistFormViewModel
        {
            Id = playlist.Id,
            Name = playlist.Name,
            OwnerUserName = playlist.OwnerUserName,
            Description = playlist.Description,
            IsPublic = playlist.IsPublic,
            NewSongAddedAt = DateTime.UtcNow
        };

        await PopulateEditModelAsync(model, context, id);

        ViewData["Breadcrumbs"] = new List<(string Label, string? Url)>
        {
            ("Home", Url.Action("Index", "Home")),
            ("Playlists", Url.Action("Index", "Playlist")),
            (playlist.Name, Url.Action(nameof(Index), "Playlist")),
            ("Edit", null)
        };

        return View(model);
    }

    [Authorize(Roles = "Admin")]
    [HttpPost("playlists/{id:int}/edit")]
    [HttpPost("~/Playlist/Edit/{id:int}")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, PlaylistFormViewModel model)
    {
        if (id != model.Id)
        {
            return BadRequest();
        }

        if (!ModelState.IsValid)
        {
            await using var validationContext = await _dbContextFactory.CreateDbContextAsync();
            await PopulateEditModelAsync(model, validationContext, id);
            model.NewSongAddedAt = DateTime.UtcNow;

            ViewData["Breadcrumbs"] = new List<(string Label, string? Url)>
            {
                ("Home", Url.Action("Index", "Home")),
                ("Playlists", Url.Action("Index", "Playlist")),
                ("Edit", null)
            };

            return View(model);
        }

        await using var context = await _dbContextFactory.CreateDbContextAsync();
        var playlist = await context.UserPlaylists.FirstOrDefaultAsync(item => item.Id == id);
        if (playlist is null)
        {
            return NotFound();
        }

        playlist.Name = model.Name.Trim();
        playlist.OwnerUserName = model.OwnerUserName.Trim();
        playlist.Description = model.Description.Trim();
        playlist.IsPublic = model.IsPublic;

        await context.SaveChangesAsync();

        return RedirectToAction(nameof(Index));
    }

    [Authorize(Roles = "Admin")]
    [HttpPost("playlists/{id:int}/songs")]
    [HttpPost("~/Playlist/AddSong/{id:int}")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> AddSong(int id, int songId, DateTime? newSongAddedAt)
    {
        if (songId <= 0)
        {
            return RedirectToAction(nameof(Edit), new { id });
        }

        await using var context = await _dbContextFactory.CreateDbContextAsync();
        var playlistExists = await context.UserPlaylists.AnyAsync(item => item.Id == id);
        if (!playlistExists)
        {
            return NotFound();
        }

        var songExists = await context.Songs.AnyAsync(item => item.Id == songId);
        if (!songExists)
        {
            return NotFound();
        }

        var alreadyAdded = await context.PlaylistSongs.AnyAsync(item => item.PlaylistId == id && item.SongId == songId);
        if (!alreadyAdded)
        {
            context.PlaylistSongs.Add(new PlaylistSong
            {
                PlaylistId = id,
                SongId = songId,
                // Always set server time to prevent client manipulation
                AddedAt = DateTime.UtcNow
            });

            await context.SaveChangesAsync();
        }

        return RedirectToAction(nameof(Edit), new { id });
    }

    [Authorize(Roles = "Admin")]
    [HttpPost("playlists/{id:int}/songs/remove")]
    [HttpPost("~/Playlist/RemoveSong/{id:int}")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> RemoveSong(int id, int songId)
    {
        if (songId <= 0)
        {
            return RedirectToAction(nameof(Edit), new { id });
        }

        await using var context = await _dbContextFactory.CreateDbContextAsync();
        var relation = await context.PlaylistSongs.FirstOrDefaultAsync(item => item.PlaylistId == id && item.SongId == songId);
        if (relation is null)
        {
            return RedirectToAction(nameof(Edit), new { id });
        }

        context.PlaylistSongs.Remove(relation);
        await context.SaveChangesAsync();

        return RedirectToAction(nameof(Edit), new { id });
    }

    [AllowAnonymous]
    [HttpPost("playlists/{id:int}/like")]
    [HttpPost("~/Playlist/Like/{id:int}")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Like(int id)
    {
        var likedPlaylistIds = ReadLikedPlaylistIds();

        await using var context = await _dbContextFactory.CreateDbContextAsync();
        var playlist = await context.UserPlaylists.FirstOrDefaultAsync(item => item.Id == id);
        if (playlist is null)
        {
            return NotFound();
        }

        if (likedPlaylistIds.Contains(id))
        {
            likedPlaylistIds.Remove(id);
            if (playlist.Likes > 0)
            {
                playlist.Likes -= 1;
            }
        }
        else
        {
            likedPlaylistIds.Add(id);
            playlist.Likes += 1;
        }

        await context.SaveChangesAsync();
        PersistLikedPlaylistIds(likedPlaylistIds);

        return RedirectToAction(nameof(Index));
    }

    [Authorize(Roles = "Admin")]
    [HttpGet("playlists/{id:int}/delete")]
    [HttpGet("~/Playlist/Delete/{id:int}")]
    public IActionResult Delete(int id)
    {
        using var context = _dbContextFactory.CreateDbContext();
        var playlist = context.UserPlaylists
            .AsNoTracking()
            .Include(item => item.PlaylistSongs)
            .ThenInclude(item => item.Song)
            .FirstOrDefault(item => item.Id == id);

        if (playlist is null)
        {
            return NotFound();
        }

        ViewData["Breadcrumbs"] = new List<(string Label, string? Url)>
        {
            ("Home", Url.Action("Index", "Home")),
            ("Playlists", Url.Action("Index", "Playlist")),
            (playlist.Name, Url.Action(nameof(Index), "Playlist")),
            ("Delete", null)
        };

        return View(playlist);
    }

    [Authorize(Roles = "Admin")]
    [HttpPost("playlists/{id:int}/delete")]
    [HttpPost("~/Playlist/Delete/{id:int}")]
    [ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        await using var context = await _dbContextFactory.CreateDbContextAsync();
        var playlist = await context.UserPlaylists.FirstOrDefaultAsync(item => item.Id == id);
        if (playlist is null)
        {
            return NotFound();
        }

        context.UserPlaylists.Remove(playlist);
        await context.SaveChangesAsync();

        return RedirectToAction(nameof(Index));
    }
}