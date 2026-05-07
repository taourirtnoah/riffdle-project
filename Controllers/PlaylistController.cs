using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Riffdle.Data;
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
}