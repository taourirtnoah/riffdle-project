using Microsoft.AspNetCore.Mvc;
using Riffdle.Models.Mock;
using Riffdle.Models.ViewModels;

namespace Riffdle.Controllers;

public class BandController : Controller
{
    private readonly BandMockRepository _bandRepository;

    public BandController(BandMockRepository bandRepository)
    {
        _bandRepository = bandRepository;
    }

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
}
