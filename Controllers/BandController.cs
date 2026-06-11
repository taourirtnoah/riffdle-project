using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Riffdle.Data;
using Riffdle.Models.Domain;
using Riffdle.Models.Mock;
using Riffdle.Models.ViewModels;

namespace Riffdle.Controllers;

public class BandController : Controller
{
    private readonly BandMockRepository _bandRepository;
    private readonly IDbContextFactory<RiffdleDbContext> _dbContextFactory;

    public BandController(BandMockRepository bandRepository, IDbContextFactory<RiffdleDbContext> dbContextFactory)
    {
        _bandRepository = bandRepository;
        _dbContextFactory = dbContextFactory;
    }

    [AllowAnonymous]
    [HttpGet("bands")]
    [HttpGet("~/Band/Index")]
    public IActionResult Index()
    {
        ViewData["Breadcrumbs"] = new List<(string Label, string? Url)>
        {
            ("Home", Url.Action("Index", "Home")),
            ("Bands", null)
        };

        return View(new BandIndexViewModel
        {
            Bands = _bandRepository.GetAll()
        });
    }

    [AllowAnonymous]
    [HttpGet("bands/{id:int}")]
    [HttpGet("~/Band/Details/{id:int}")]
    public IActionResult Details(int id)
    {
        var band = _bandRepository.GetById(id);
        if (band is null)
        {
            return NotFound();
        }

        ViewData["Breadcrumbs"] = new List<(string Label, string? Url)>
        {
            ("Home", Url.Action("Index", "Home")),
            ("Bands", Url.Action("Index", "Band")),
            (band.Name, null)
        };

        return View(new BandDetailsViewModel
        {
            Band = band
        });
    }

    [Authorize(Roles = "Admin")]
    [HttpGet("bands/create")]
    [HttpGet("~/Band/Create")]
    public IActionResult Create()
    {
        ViewData["Breadcrumbs"] = new List<(string Label, string? Url)>
        {
            ("Home", Url.Action("Index", "Home")),
            ("Bands", Url.Action("Index", "Band")),
            ("Create", null)
        };

        return View(new BandFormViewModel());
    }

    [Authorize(Roles = "Admin")]
    [HttpPost("bands/create")]
    [HttpPost("~/Band/Create")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(BandFormViewModel model)
    {
        if (!ModelState.IsValid)
        {
            ViewData["Breadcrumbs"] = new List<(string Label, string? Url)>
            {
                ("Home", Url.Action("Index", "Home")),
                ("Bands", Url.Action("Index", "Band")),
                ("Create", null)
            };

            return View(model);
        }

        await using var context = await _dbContextFactory.CreateDbContextAsync();
        var band = new Band
        {
            Name = model.Name.Trim(),
            FormedYear = model.FormedYear,
            Country = model.Country.Trim(),
            Description = model.Description.Trim(),
            GenreId = model.GenreId
        };

        context.Bands.Add(band);
        await context.SaveChangesAsync();

        return RedirectToAction(nameof(Details), new { id = band.Id });
    }

    [Authorize(Roles = "Admin")]
    [HttpGet("bands/{id:int}/edit")]
    [HttpGet("~/Band/Edit/{id:int}")]
    public IActionResult Edit(int id)
    {
        var band = _bandRepository.GetById(id);
        if (band is null)
        {
            return NotFound();
        }

        ViewData["Breadcrumbs"] = new List<(string Label, string? Url)>
        {
            ("Home", Url.Action("Index", "Home")),
            ("Bands", Url.Action("Index", "Band")),
            (band.Name, Url.Action(nameof(Details), "Band", new { id = band.Id })),
            ("Edit", null)
        };

        return View(new BandFormViewModel
        {
            Id = band.Id,
            Name = band.Name,
            FormedYear = band.FormedYear,
            Country = band.Country,
            Description = band.Description,
            GenreId = band.GenreId,
            GenreName = band.Genre?.Name
        });
    }

    [Authorize(Roles = "Admin")]
    [HttpPost("bands/{id:int}/edit")]
    [HttpPost("~/Band/Edit/{id:int}")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, BandFormViewModel model)
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
                ("Bands", Url.Action("Index", "Band")),
                ("Edit", null)
            };

            return View(model);
        }

        await using var context = await _dbContextFactory.CreateDbContextAsync();
        var band = await context.Bands.FirstOrDefaultAsync(item => item.Id == id);
        if (band is null)
        {
            return NotFound();
        }

        band.Name = model.Name.Trim();
        band.FormedYear = model.FormedYear;
        band.Country = model.Country.Trim();
        band.Description = model.Description.Trim();
        band.GenreId = model.GenreId;

        await context.SaveChangesAsync();

        return RedirectToAction(nameof(Details), new { id = band.Id });
    }

    [Authorize(Roles = "Admin")]
    [HttpGet("bands/{id:int}/delete")]
    [HttpGet("~/Band/Delete/{id:int}")]
    public IActionResult Delete(int id)
    {
        var band = _bandRepository.GetById(id);
        if (band is null)
        {
            return NotFound();
        }

        ViewData["Breadcrumbs"] = new List<(string Label, string? Url)>
        {
            ("Home", Url.Action("Index", "Home")),
            ("Bands", Url.Action("Index", "Band")),
            (band.Name, Url.Action(nameof(Details), "Band", new { id = band.Id })),
            ("Delete", null)
        };

        return View(band);
    }

    [Authorize(Roles = "Admin")]
    [HttpPost("bands/{id:int}/delete")]
    [HttpPost("~/Band/Delete/{id:int}")]
    [ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        await using var context = await _dbContextFactory.CreateDbContextAsync();
        var band = await context.Bands.FirstOrDefaultAsync(item => item.Id == id);
        if (band is null)
        {
            return NotFound();
        }

        context.Bands.Remove(band);
        await context.SaveChangesAsync();

        return RedirectToAction(nameof(Index));
    }
}
