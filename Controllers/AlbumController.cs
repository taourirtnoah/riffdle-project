using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Riffdle.Data;
using Riffdle.Models.Domain;
using Riffdle.Models.Mock;
using Riffdle.Models.ViewModels;

namespace Riffdle.Controllers;

public class AlbumController : Controller
{
    private readonly AlbumMockRepository _albumRepository;
    private readonly BandMockRepository _bandRepository;
    private readonly IDbContextFactory<RiffdleDbContext> _dbContextFactory;

    public AlbumController(
        AlbumMockRepository albumRepository,
        BandMockRepository bandRepository,
        IDbContextFactory<RiffdleDbContext> dbContextFactory)
    {
        _albumRepository = albumRepository;
        _bandRepository = bandRepository;
        _dbContextFactory = dbContextFactory;
    }

    [HttpGet("albums")]
    [HttpGet("~/Album/Index")]
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

    [HttpGet("albums/{id:int}")]
    [HttpGet("~/Album/Details/{id:int}")]
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

    [HttpGet("albums/create")]
    [HttpGet("~/Album/Create")]
    public IActionResult Create()
    {
        ViewData["Breadcrumbs"] = new List<(string Label, string? Url)>
        {
            ("Home", Url.Action("Index", "Home")),
            ("Albums", Url.Action("Index", "Album")),
            ("Create", null)
        };

        return View(new AlbumFormViewModel());
    }

    [HttpPost("albums/create")]
    [HttpPost("~/Album/Create")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(AlbumFormViewModel model)
    {
        if (!ModelState.IsValid)
        {
            ViewData["Breadcrumbs"] = new List<(string Label, string? Url)>
            {
                ("Home", Url.Action("Index", "Home")),
                ("Albums", Url.Action("Index", "Album")),
                ("Create", null)
            };

            return View(model);
        }

        await using var context = await _dbContextFactory.CreateDbContextAsync();

        var album = new Album
        {
            Title = model.Title.Trim(),
            ReleaseYear = model.ReleaseYear,
            BandId = model.BandId
        };

        context.Albums.Add(album);
        await context.SaveChangesAsync();

        return RedirectToAction(nameof(Details), new { id = album.Id });
    }

    [HttpGet("albums/{id:int}/edit")]
    [HttpGet("~/Album/Edit/{id:int}")]
    public IActionResult Edit(int id)
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
            (album.Title, Url.Action(nameof(Details), "Album", new { id = album.Id })),
            ("Edit", null)
        };

        return View(new AlbumFormViewModel
        {
            Id = album.Id,
            Title = album.Title,
            ReleaseYear = album.ReleaseYear,
            BandId = album.BandId,
            BandName = album.Band?.Name
        });
    }

    [HttpPost("albums/{id:int}/edit")]
    [HttpPost("~/Album/Edit/{id:int}")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, AlbumFormViewModel model)
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
                ("Albums", Url.Action("Index", "Album")),
                ("Edit", null)
            };

            return View(model);
        }

        await using var context = await _dbContextFactory.CreateDbContextAsync();
        var album = await context.Albums.FirstOrDefaultAsync(item => item.Id == id);
        if (album is null)
        {
            return NotFound();
        }

        album.Title = model.Title.Trim();
        album.ReleaseYear = model.ReleaseYear;
        album.BandId = model.BandId;

        await context.SaveChangesAsync();

        return RedirectToAction(nameof(Details), new { id = album.Id });
    }

    [HttpGet("albums/{id:int}/delete")]
    [HttpGet("~/Album/Delete/{id:int}")]
    public IActionResult Delete(int id)
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
            (album.Title, Url.Action(nameof(Details), "Album", new { id = album.Id })),
            ("Delete", null)
        };

        return View(album);
    }

    [HttpPost("albums/{id:int}/delete")]
    [HttpPost("~/Album/Delete/{id:int}")]
    [ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        await using var context = await _dbContextFactory.CreateDbContextAsync();
        var album = await context.Albums.FirstOrDefaultAsync(item => item.Id == id);
        if (album is null)
        {
            return NotFound();
        }

        context.Albums.Remove(album);
        await context.SaveChangesAsync();

        return RedirectToAction(nameof(Index));
    }
}
