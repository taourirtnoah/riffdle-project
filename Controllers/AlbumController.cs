using Microsoft.AspNetCore.Mvc;
using Riffdle.Models.Mock;
using Riffdle.Models.ViewModels;

namespace Riffdle.Controllers;

public class AlbumController : Controller
{
    private readonly AlbumMockRepository _albumRepository;

    public AlbumController(AlbumMockRepository albumRepository)
    {
        _albumRepository = albumRepository;
    }

    public IActionResult Index()
    {
        ViewData["Breadcrumbs"] = new List<(string Label, string? Url)>
        {
            ("Home", Url.Action("Index", "Home")),
            ("Albums", null)
        };

        return View(new AlbumIndexViewModel
        {
            Albums = _albumRepository.GetAll()
        });
    }

    public IActionResult Details(int id)
    {
        var album = _albumRepository.GetById(id);
        if (album is null)
        {
            return NotFound();
        }

        ViewData["Breadcrumbs"] = new List<(string Label, string? Url)>
        {
            ("Home", Url.Action("Index", "Home")),
            ("Albums", Url.Action("Index", "Album")),
            (album.Title, null)
        };

        return View(new AlbumDetailsViewModel
        {
            Album = album
        });
    }
}
