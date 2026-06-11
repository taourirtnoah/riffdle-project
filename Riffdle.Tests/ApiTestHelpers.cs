using System.Net;
using System.Net.Http.Json;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Riffdle.Data;
using Riffdle.Models.Domain;
using Riffdle.Models.DTO;

namespace Riffdle.Tests;

public static class ApiTestHelpers
{
    public static async Task<int> GetGenreIdAsync(ApiWebApplicationFactory factory, string name)
    {
        using var scope = factory.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<RiffdleDbContext>();
        return await db.Genres.Where(g => g.Name == name).Select(g => g.Id).FirstAsync();
    }

    public static async Task<int> GetBandIdAsync(ApiWebApplicationFactory factory, string name)
    {
        using var scope = factory.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<RiffdleDbContext>();
        return await db.Bands.Where(b => b.Name == name).Select(b => b.Id).FirstAsync();
    }

    public static async Task<int> GetAlbumIdAsync(ApiWebApplicationFactory factory, string title)
    {
        using var scope = factory.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<RiffdleDbContext>();
        return await db.Albums.Where(a => a.Title == title).Select(a => a.Id).FirstAsync();
    }

    public static async Task SeedGenreAsync(ApiWebApplicationFactory factory, string name, string description = "Test genre")
    {
        using var scope = factory.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<RiffdleDbContext>();
        db.Genres.Add(new Genre { Name = name, Description = description });
        await db.SaveChangesAsync();
    }

    public static async Task SeedBandAsync(ApiWebApplicationFactory factory, string name, int genreId)
    {
        using var scope = factory.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<RiffdleDbContext>();
        db.Bands.Add(new Band
        {
            Name = name,
            FormedYear = 2000,
            Country = "Croatia",
            Description = "Seed band",
            GenreId = genreId
        });
        await db.SaveChangesAsync();
    }

    public static async Task SeedAlbumAsync(ApiWebApplicationFactory factory, string title, int bandId)
    {
        using var scope = factory.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<RiffdleDbContext>();
        db.Albums.Add(new Album
        {
            Title = title,
            ReleaseYear = 2024,
            BandId = bandId
        });
        await db.SaveChangesAsync();
    }

    public static async Task SeedSongAsync(ApiWebApplicationFactory factory, string title, int albumId)
    {
        using var scope = factory.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<RiffdleDbContext>();
        db.Songs.Add(new Song
        {
            Title = title,
            DurationSeconds = 180,
            AlbumId = albumId,
            OpeningLyric = "seed lyric",
            IsDailyQuizSong = false,
            AudioSnippetUrl = "https://example.com/audio.mp3",
            AlbumCoverUrl = "https://example.com/cover.jpg"
        });
        await db.SaveChangesAsync();
    }

    public static async Task<T> ReadJsonAsync<T>(HttpResponseMessage response)
    {
        response.EnsureSuccessStatusCode();
        var payload = await response.Content.ReadFromJsonAsync<T>();
        return payload ?? throw new InvalidOperationException("Response payload was empty.");
    }

    public static async Task<HttpResponseMessage> SendJsonAsync<T>(HttpClient client, HttpMethod method, string url, T body)
    {
        var request = new HttpRequestMessage(method, url)
        {
            Content = JsonContent.Create(body)
        };
        return await client.SendAsync(request);
    }

    public static async Task AssertBadRequestAsync(HttpResponseMessage response)
    {
        if (response.StatusCode != HttpStatusCode.BadRequest)
        {
            var body = await response.Content.ReadAsStringAsync();
            throw new InvalidOperationException($"Expected 400 BadRequest but got {(int)response.StatusCode}: {body}");
        }
    }
}
