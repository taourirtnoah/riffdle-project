using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Riffdle.Data;
using Riffdle.Models.Domain;
using Riffdle.Models.Mock;
using Riffdle.Models.ViewModels;

namespace Riffdle.Controllers;

public class GenreController : Controller
{
    private readonly GenreMockRepository _genreRepository;
    private readonly BandMockRepository _bandRepository;
    private readonly IDbContextFactory<RiffdleDbContext> _dbContextFactory;

    public GenreController(
        GenreMockRepository genreRepository,
        BandMockRepository bandRepository,
        IDbContextFactory<RiffdleDbContext> dbContextFactory)
    {
        _genreRepository = genreRepository;
        _bandRepository = bandRepository;
        _dbContextFactory = dbContextFactory;
    }

    [HttpGet("genres")]
    [HttpGet("~/Genre/Index")]
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

    [HttpGet("genres/{id:int}")]
    [HttpGet("~/Genre/Details/{id:int}")]
    public IActionResult Details(int id)
    {
        var genre = _genreRepository.GetById(id);
        if (genre is null)
        {
            return NotFound();
        }

        var bandsInGenre = _bandRepository
            .GetAll()
            .Where(band => band.Genre is not null && band.Genre.Id == genre.Id)
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

    [HttpGet("genres/create")]
    [HttpGet("~/Genre/Create")]
    public IActionResult Create()
    {
        ViewData["Breadcrumbs"] = new List<(string Label, string? Url)>
        {
            ("Home", Url.Action("Index", "Home")),
            ("Genres", Url.Action("Index", "Genre")),
            ("Create", null)
        };

        return View(new GenreFormViewModel());
    }

    [HttpPost("genres/create")]
    [HttpPost("~/Genre/Create")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(GenreFormViewModel model)
    {
        if (!ModelState.IsValid)
        {
            ViewData["Breadcrumbs"] = new List<(string Label, string? Url)>
            {
                ("Home", Url.Action("Index", "Home")),
                ("Genres", Url.Action("Index", "Genre")),
                ("Create", null)
            };

            return View(model);
        }

        await using var context = await _dbContextFactory.CreateDbContextAsync();
        var genre = new Genre
        {
            Name = model.Name.Trim(),
            Description = model.Description.Trim()
        };

        context.Genres.Add(genre);
        await context.SaveChangesAsync();

        return RedirectToAction(nameof(Details), new { id = genre.Id });
    }

    [HttpGet("genres/{id:int}/edit")]
    [HttpGet("~/Genre/Edit/{id:int}")]
    public IActionResult Edit(int id)
    {
        var genre = _genreRepository.GetById(id);
        if (genre is null)
        {
            return NotFound();
        }

        ViewData["Breadcrumbs"] = new List<(string Label, string? Url)>
        {
            ("Home", Url.Action("Index", "Home")),
            ("Genres", Url.Action("Index", "Genre")),
            (genre.Name, Url.Action(nameof(Details), "Genre", new { id = genre.Id })),
            ("Edit", null)
        };

        return View(new GenreFormViewModel
        {
            Id = genre.Id,
            Name = genre.Name,
            Description = genre.Description
        });
    }

    [HttpPost("genres/{id:int}/edit")]
    [HttpPost("~/Genre/Edit/{id:int}")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, GenreFormViewModel model)
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
                ("Genres", Url.Action("Index", "Genre")),
                ("Edit", null)
            };

            return View(model);
        }

        await using var context = await _dbContextFactory.CreateDbContextAsync();
        var genre = await context.Genres.FirstOrDefaultAsync(item => item.Id == id);
        if (genre is null)
        {
            return NotFound();
        }

        genre.Name = model.Name.Trim();
        genre.Description = model.Description.Trim();

        await context.SaveChangesAsync();

        return RedirectToAction(nameof(Details), new { id = genre.Id });
    }

    [HttpGet("genres/{id:int}/delete")]
    [HttpGet("~/Genre/Delete/{id:int}")]
    public IActionResult Delete(int id)
    {
        var genre = _genreRepository.GetById(id);
        if (genre is null)
        {
            return NotFound();
        }

        ViewData["Breadcrumbs"] = new List<(string Label, string? Url)>
        {
            ("Home", Url.Action("Index", "Home")),
            ("Genres", Url.Action("Index", "Genre")),
            (genre.Name, Url.Action(nameof(Details), "Genre", new { id = genre.Id })),
            ("Delete", null)
        };

        return View(genre);
    }

    [HttpPost("genres/{id:int}/delete")]
    [HttpPost("~/Genre/Delete/{id:int}")]
    [ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        await using var context = await _dbContextFactory.CreateDbContextAsync();
        var genre = await context.Genres.FirstOrDefaultAsync(item => item.Id == id);
        if (genre is null)
        {
            return NotFound();
        }

        context.Genres.Remove(genre);
        await context.SaveChangesAsync();

        return RedirectToAction(nameof(Index));
    }
}
