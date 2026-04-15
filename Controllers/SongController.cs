using Microsoft.AspNetCore.Mvc;
using Riffdle.Models.Mock;
using Riffdle.Models.ViewModels;

namespace Riffdle.Controllers;

public class SongController : Controller
{
    private readonly SongMockRepository _songRepository;

    public SongController(SongMockRepository songRepository)
    {
        _songRepository = songRepository;
    }

    public IActionResult Index()
    {
        ViewData["Breadcrumbs"] = new List<(string Label, string? Url)>
        {
            ("Home", Url.Action("Index", "Home")),
            ("Songs", null)
        };

        return View(new SongIndexViewModel
        {
            Songs = _songRepository.GetAll()
        });
    }

    public IActionResult Details(int id)
    {
        var song = _songRepository.GetById(id);
        if (song is null)
        {
            return NotFound();
        }

        ViewData["Breadcrumbs"] = new List<(string Label, string? Url)>
        {
            ("Home", Url.Action("Index", "Home")),
            ("Songs", Url.Action("Index", "Song")),
            (song.Title, null)
        };

        return View(new SongDetailsViewModel
        {
            Song = song
        });
    }
}
