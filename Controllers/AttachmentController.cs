using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;
using Riffdle.Data;
using Riffdle.Models.Domain;

namespace Riffdle.Controllers;

public class AttachmentController : Controller
{
    private readonly RiffdleDbContext _db;
    private readonly IWebHostEnvironment _env;

    public AttachmentController(RiffdleDbContext db, IWebHostEnvironment env)
    {
        _db = db;
        _env = env;
    }

    [HttpPost]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Upload(int songId, IFormFile file)
    {
        if (file == null || file.Length == 0) return BadRequest("No file provided");

        var song = await _db.Songs.FindAsync(songId);
        if (song == null) return NotFound();

        var quizRound = await _db.QuizRounds.FirstOrDefaultAsync(round => round.SongId == songId);
        if (quizRound == null)
        {
            quizRound = new QuizRound { SongId = songId };
            _db.QuizRounds.Add(quizRound);
            await _db.SaveChangesAsync();
        }

        var uploadsRoot = Path.Combine(_env.WebRootPath ?? "wwwroot", "uploads", "songs", songId.ToString());
        Directory.CreateDirectory(uploadsRoot);

        var storedFileName = $"{Guid.NewGuid()}{Path.GetExtension(file.FileName)}";
        var filePath = Path.Combine(uploadsRoot, storedFileName);

        await using (var stream = System.IO.File.Create(filePath))
        {
            await file.CopyToAsync(stream);
        }

        var url = $"/uploads/songs/{songId}/{storedFileName}";

        var attachment = new Attachment
        {
            SongId = songId,
            QuizRoundId = quizRound.Id,
            FileName = storedFileName,
            OriginalName = file.FileName,
            ContentType = file.ContentType ?? string.Empty,
            Size = file.Length,
            Url = url
        };

        _db.Attachments.Add(attachment);
        await _db.SaveChangesAsync();

        return Json(new { success = true, attachmentId = attachment.Id, url = url, name = attachment.OriginalName });
    }

    [HttpGet]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> GetAttachments(int songId)
    {
        var list = await _db.Attachments.Where(a => a.SongId == songId).OrderByDescending(a => a.CreatedAt).ToListAsync();
        return PartialView("_AttachmentList", list);
    }

    [HttpPost]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> DeleteAttachment(int id)
    {
        var att = await _db.Attachments.FindAsync(id);
        if (att == null) return NotFound();

        var physical = Path.Combine(_env.WebRootPath ?? "wwwroot", att.Url.TrimStart('/').Replace('/', Path.DirectorySeparatorChar));
        try
        {
            if (System.IO.File.Exists(physical)) System.IO.File.Delete(physical);
        }
        catch { }

        _db.Attachments.Remove(att);
        await _db.SaveChangesAsync();

        return Json(new { success = true });
    }
}
