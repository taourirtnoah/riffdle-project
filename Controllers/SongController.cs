using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Riffdle.Data;
using Riffdle.Models.Domain;
using Riffdle.Models.Mock;
using Riffdle.Models.ViewModels;

namespace Riffdle.Controllers;

public class SongController : Controller
{
    private readonly SongMockRepository _songRepository;
    private readonly IDbContextFactory<RiffdleDbContext> _dbContextFactory;

    public SongController(SongMockRepository songRepository, IDbContextFactory<RiffdleDbContext> dbContextFactory)
    {
        _songRepository = songRepository;
        _dbContextFactory = dbContextFactory;
    }

    [AllowAnonymous]
    [HttpGet("songs")]
    [HttpGet("~/Song/Index")]
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

    [AllowAnonymous]
    [HttpGet("songs/{id:int}")]
    [HttpGet("~/Song/Details/{id:int}")]
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

    [Authorize(Roles = "Admin")]
    [HttpGet("songs/create")]
    [HttpGet("~/Song/Create")]
    public IActionResult Create()
    {
        ViewData["Breadcrumbs"] = new List<(string Label, string? Url)>
        {
            ("Home", Url.Action("Index", "Home")),
            ("Songs", Url.Action("Index", "Song")),
            ("Create", null)
        };

        return View(new SongFormViewModel());
    }

    [Authorize(Roles = "Admin")]
    [HttpPost("songs/create")]
    [HttpPost("~/Song/Create")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(SongFormViewModel model)
    {
        if (!ModelState.IsValid)
        {
            ViewData["Breadcrumbs"] = new List<(string Label, string? Url)>
            {
                ("Home", Url.Action("Index", "Home")),
                ("Songs", Url.Action("Index", "Song")),
                ("Create", null)
            };

            return View(model);
        }

        await using var context = await _dbContextFactory.CreateDbContextAsync();
        var song = new Song
        {
            Title = model.Title.Trim(),
            DurationSeconds = model.DurationSeconds,
            AlbumId = model.AlbumId,
            OpeningLyric = model.OpeningLyric.Trim(),
            IsDailyQuizSong = model.IsDailyQuizSong,
            AlbumCoverUrl = model.AlbumCoverUrl.Trim()
        };

        context.Songs.Add(song);
        await context.SaveChangesAsync();

        return RedirectToAction(nameof(Details), new { id = song.Id });
    }

    [Authorize(Roles = "Admin")]
    [HttpGet("songs/{id:int}/edit")]
    [HttpGet("~/Song/Edit/{id:int}")]
    public IActionResult Edit(int id)
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
            (song.Title, Url.Action(nameof(Details), "Song", new { id = song.Id })),
            ("Edit", null)
        };

        return View(new SongFormViewModel
        {
            Id = song.Id,
            Title = song.Title,
            DurationSeconds = song.DurationSeconds,
            AlbumId = song.AlbumId,
            AlbumTitle = song.Album?.Title,
            OpeningLyric = song.OpeningLyric,
            IsDailyQuizSong = song.IsDailyQuizSong,
            AlbumCoverUrl = song.AlbumCoverUrl
        });
    }

    [Authorize(Roles = "Admin")]
    [HttpPost("songs/{id:int}/edit")]
    [HttpPost("~/Song/Edit/{id:int}")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, SongFormViewModel model)
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
                ("Songs", Url.Action("Index", "Song")),
                ("Edit", null)
            };

            return View(model);
        }

        await using var context = await _dbContextFactory.CreateDbContextAsync();
        var song = await context.Songs.FirstOrDefaultAsync(item => item.Id == id);
        if (song is null)
        {
            return NotFound();
        }

        song.Title = model.Title.Trim();
        song.DurationSeconds = model.DurationSeconds;
        song.AlbumId = model.AlbumId;
        song.OpeningLyric = model.OpeningLyric.Trim();
        song.IsDailyQuizSong = model.IsDailyQuizSong;
        song.AlbumCoverUrl = model.AlbumCoverUrl.Trim();

        await context.SaveChangesAsync();

        return RedirectToAction(nameof(Details), new { id = song.Id });
    }

    [Authorize(Roles = "Admin")]
    [HttpGet("songs/{id:int}/delete")]
    [HttpGet("~/Song/Delete/{id:int}")]
    public IActionResult Delete(int id)
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
            (song.Title, Url.Action(nameof(Details), "Song", new { id = song.Id })),
            ("Delete", null)
        };

        return View(song);
    }

    [Authorize(Roles = "Admin")]
    [HttpPost("songs/{id:int}/delete")]
    [HttpPost("~/Song/Delete/{id:int}")]
    [ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        await using var context = await _dbContextFactory.CreateDbContextAsync();
        var song = await context.Songs.FirstOrDefaultAsync(item => item.Id == id);
        if (song is null)
        {
            return NotFound();
        }

        context.Songs.Remove(song);
        await context.SaveChangesAsync();

        return RedirectToAction(nameof(Index));
    }
}
