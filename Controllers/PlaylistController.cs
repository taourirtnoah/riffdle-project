using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Riffdle.Data;
using Riffdle.Models.Domain;
using Riffdle.Models.ViewModels;

namespace Riffdle.Controllers;

public class PlaylistController : Controller
{
    private readonly IDbContextFactory<RiffdleDbContext> _dbContextFactory;

    public PlaylistController(IDbContextFactory<RiffdleDbContext> dbContextFactory)
    {
        _dbContextFactory = dbContextFactory;
    }

    [HttpGet("playlists")]
    [HttpGet("~/Playlist/Index")]
    public IActionResult Index()
    {
        using var context = _dbContextFactory.CreateDbContext();

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
            Playlists = playlists
        });
    }

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
            CreatedAt = model.CreatedAt,
            IsPublic = model.IsPublic,
            Likes = model.Likes
        };

        context.UserPlaylists.Add(playlist);
        await context.SaveChangesAsync();

        return RedirectToAction(nameof(Index));
    }

    [HttpGet("playlists/{id:int}/edit")]
    [HttpGet("~/Playlist/Edit/{id:int}")]
    public IActionResult Edit(int id)
    {
        using var context = _dbContextFactory.CreateDbContext();
        var playlist = context.UserPlaylists.AsNoTracking().FirstOrDefault(item => item.Id == id);
        if (playlist is null)
        {
            return NotFound();
        }

        ViewData["Breadcrumbs"] = new List<(string Label, string? Url)>
        {
            ("Home", Url.Action("Index", "Home")),
            ("Playlists", Url.Action("Index", "Playlist")),
            (playlist.Name, Url.Action(nameof(Index), "Playlist")),
            ("Edit", null)
        };

        return View(new PlaylistFormViewModel
        {
            Id = playlist.Id,
            Name = playlist.Name,
            OwnerUserName = playlist.OwnerUserName,
            Description = playlist.Description,
            CreatedAt = playlist.CreatedAt,
            IsPublic = playlist.IsPublic,
            Likes = playlist.Likes
        });
    }

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
        playlist.CreatedAt = model.CreatedAt;
        playlist.IsPublic = model.IsPublic;
        playlist.Likes = model.Likes;

        await context.SaveChangesAsync();

        return RedirectToAction(nameof(Index));
    }

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