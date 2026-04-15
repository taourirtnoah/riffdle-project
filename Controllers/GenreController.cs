using Microsoft.AspNetCore.Mvc;
using Riffdle.Models.Mock;
using Riffdle.Models.ViewModels;

namespace Riffdle.Controllers;

public class GenreController : Controller
{
    private readonly GenreMockRepository _genreRepository;
    private readonly BandMockRepository _bandRepository;

    public GenreController(GenreMockRepository genreRepository, BandMockRepository bandRepository)
    {
        _genreRepository = genreRepository;
        _bandRepository = bandRepository;
    }

    public IActionResult Index()
    {
        ViewData["Breadcrumbs"] = new List<(string Label, string? Url)>
        {
            ("Home", Url.Action("Index", "Home")),
            ("Genres", null)
        };

        return View(new GenreIndexViewModel
        {
            Genres = _genreRepository.GetAll()
        });
    }

    public IActionResult Details(int id)
    {
        var genre = _genreRepository.GetById(id);
        if (genre is null)
        {
            return NotFound();
        }

        var bandsInGenre = _bandRepository
            .GetAll()
            .Where(band => band.Genre.Id == genre.Id)
            .OrderBy(band => band.Name)
            .ToList();

        ViewData["Breadcrumbs"] = new List<(string Label, string? Url)>
        {
            ("Home", Url.Action("Index", "Home")),
            ("Genres", Url.Action("Index", "Genre")),
            (genre.Name, null)
        };

        return View(new GenreDetailsViewModel
        {
            Genre = genre,
            Bands = bandsInGenre
        });
    }
}
