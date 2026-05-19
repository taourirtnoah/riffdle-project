using Microsoft.AspNetCore.Mvc;
using Riffdle.Models.Mock;

namespace Riffdle.Controllers;

public class SearchController : Controller
{
    private readonly AlbumMockRepository _albumRepository;
    private readonly BandMockRepository _bandRepository;
    private readonly GenreMockRepository _genreRepository;

    public SearchController(
        AlbumMockRepository albumRepository,
        BandMockRepository bandRepository,
        GenreMockRepository genreRepository)
    {
        _albumRepository = albumRepository;
        _bandRepository = bandRepository;
        _genreRepository = genreRepository;
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
}
