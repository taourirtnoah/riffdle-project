using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Riffdle.Data;
using Riffdle.Models.Domain;
using Riffdle.Models.Mock;

namespace Riffdle.Controllers;

public class SearchController : Controller
{
    private readonly AlbumMockRepository _albumRepository;
    private readonly BandMockRepository _bandRepository;
    private readonly GenreMockRepository _genreRepository;
    private readonly SongMockRepository _songRepository;
    private readonly IDbContextFactory<RiffdleDbContext> _dbContextFactory;

    public SearchController(
        AlbumMockRepository albumRepository,
        BandMockRepository bandRepository,
        GenreMockRepository genreRepository,
        SongMockRepository songRepository,
        IDbContextFactory<RiffdleDbContext> dbContextFactory)
    {
        _albumRepository = albumRepository;
        _bandRepository = bandRepository;
        _genreRepository = genreRepository;
        _songRepository = songRepository;
        _dbContextFactory = dbContextFactory;
    }

    [HttpGet("search/albums")]
    public IActionResult Albums(string? q)
    {
        var query = q?.Trim() ?? string.Empty;
        var results = _albumRepository
            .GetAll()
            .Where(item => string.IsNullOrWhiteSpace(query) || item.Title.Contains(query, StringComparison.OrdinalIgnoreCase))
            .OrderBy(item => item.Title)
            .Take(10)
            .Select(item => new { id = item.Id, text = item.Title })
            .ToList();

        return Json(results);
    }

    [HttpGet("search/bands")]
    public IActionResult Bands(string? q)
    {
        var query = q?.Trim() ?? string.Empty;
        var results = _bandRepository
            .GetAll()
            .Where(item => string.IsNullOrWhiteSpace(query) || item.Name.Contains(query, StringComparison.OrdinalIgnoreCase))
            .OrderBy(item => item.Name)
            .Take(10)
            .Select(item => new { id = item.Id, text = item.Name })
            .ToList();

        return Json(results);
    }

    [HttpGet("search/genres")]
    public IActionResult Genres(string? q)
    {
        var query = q?.Trim() ?? string.Empty;
        var results = _genreRepository
            .GetAll()
            .Where(item => string.IsNullOrWhiteSpace(query) || item.Name.Contains(query, StringComparison.OrdinalIgnoreCase))
            .OrderBy(item => item.Name)
            .Take(10)
            .Select(item => new { id = item.Id, text = item.Name })
            .ToList();

        return Json(results);
    }

    [HttpGet("search/songs")]
    public IActionResult Songs(string? q)
    {
        var query = q?.Trim() ?? string.Empty;
        var results = _songRepository
            .GetAll()
            .Where(item =>
                string.IsNullOrWhiteSpace(query) ||
                item.Title.Contains(query, StringComparison.OrdinalIgnoreCase) ||
                (item.Album?.Title ?? string.Empty).Contains(query, StringComparison.OrdinalIgnoreCase) ||
                (item.Album?.Band?.Name ?? string.Empty).Contains(query, StringComparison.OrdinalIgnoreCase) ||
                item.OpeningLyric.Contains(query, StringComparison.OrdinalIgnoreCase))
            .OrderBy(item => item.Title)
            .Take(10)
            .Select(item => new { id = item.Id, text = item.Title })
            .ToList();

        return Json(results);
    }

    [HttpGet("search/playlists")]
    public async Task<IActionResult> Playlists(string? q)
    {
        var query = q?.Trim() ?? string.Empty;

        await using var context = await _dbContextFactory.CreateDbContextAsync();
        var playlists = await context.UserPlaylists
            .AsNoTracking()
            .Include(item => item.PlaylistSongs)
            .ThenInclude(item => item.Song)
            .ThenInclude(song => song!.Album)
            .ThenInclude(album => album!.Band)
            .ToListAsync();

        var results = playlists
            .Where(playlist =>
                string.IsNullOrWhiteSpace(query) ||
                playlist.Name.Contains(query, StringComparison.OrdinalIgnoreCase) ||
                playlist.OwnerUserName.Contains(query, StringComparison.OrdinalIgnoreCase) ||
                (playlist.Description ?? string.Empty).Contains(query, StringComparison.OrdinalIgnoreCase) ||
                (playlist.PlaylistSongs ?? Array.Empty<PlaylistSong>()).Any(item =>
                    (item.Song?.Title ?? string.Empty).Contains(query, StringComparison.OrdinalIgnoreCase) ||
                    (item.Song?.Album?.Title ?? string.Empty).Contains(query, StringComparison.OrdinalIgnoreCase) ||
                    (item.Song?.Album?.Band?.Name ?? string.Empty).Contains(query, StringComparison.OrdinalIgnoreCase)))
            .OrderByDescending(item => item.IsPublic)
            .ThenByDescending(item => item.Likes)
            .ThenBy(item => item.Name)
            .Take(10)
            .Select(item => new { id = item.Id, text = item.Name })
            .ToList();

        return Json(results);
    }
}
