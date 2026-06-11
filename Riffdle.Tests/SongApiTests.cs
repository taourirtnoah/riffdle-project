using System.Net;
using System.Net.Http.Json;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Riffdle.Data;
using Riffdle.Models.DTO;
using Xunit;

namespace Riffdle.Tests;

public class SongApiTests : IClassFixture<ApiWebApplicationFactory>
{
    private readonly ApiWebApplicationFactory _factory;
    private readonly HttpClient _client;

    public SongApiTests(ApiWebApplicationFactory factory)
    {
        _factory = factory;
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task GetAll_ReturnsSeededSongs()
    {
        await _factory.ResetDatabaseAsync();

        var response = await _client.GetAsync("/api/song");
        var songs = await response.Content.ReadFromJsonAsync<List<SongDTO>>();

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.NotNull(songs);
        Assert.Contains(songs!, song => song.Title == "Master of Puppets");
    }

    [Fact]
    public async Task GetById_ReturnsSong()
    {
        await _factory.ResetDatabaseAsync();
        var songId = await GetSongIdAsync("Master of Puppets");

        var response = await _client.GetAsync($"/api/song/{songId}");
        var song = await response.Content.ReadFromJsonAsync<SongDTO>();

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.NotNull(song);
        Assert.Equal(songId, song!.Id);
        Assert.Equal("Master of Puppets", song.Title);
    }

    [Fact]
    public async Task GetById_WhenSongDoesNotExist_ReturnsNotFound()
    {
        await _factory.ResetDatabaseAsync();

        var response = await _client.GetAsync("/api/song/999999");

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task Post_WithValidData_CreatesSong()
    {
        await _factory.ResetDatabaseAsync();
        var albumId = await ApiTestHelpers.GetAlbumIdAsync(_factory, "Ride the Lightning");
        var dto = new SongDTO
        {
            Title = "Test Song",
            DurationSeconds = 250,
            AlbumId = albumId,
            OpeningLyric = "Test opening lyric",
            IsDailyQuizSong = false,
            AudioSnippetUrl = "https://example.com/audio.mp3",
            AlbumCoverUrl = "https://example.com/cover.jpg"
        };

        var response = await ApiTestHelpers.SendJsonAsync(_client, HttpMethod.Post, "/api/song", dto);
        var created = await ApiTestHelpers.ReadJsonAsync<SongDTO>(response);

        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        Assert.True(created.Id > 0);
        Assert.Equal(dto.Title, created.Title);

        var getResponse = await _client.GetAsync($"/api/song/{created.Id}");
        Assert.Equal(HttpStatusCode.OK, getResponse.StatusCode);
    }

    [Fact]
    public async Task Post_WithInvalidData_ReturnsBadRequest()
    {
        await _factory.ResetDatabaseAsync();
        var dto = new SongDTO { Title = string.Empty, DurationSeconds = 0, AlbumId = 0, OpeningLyric = string.Empty, AudioSnippetUrl = "", AlbumCoverUrl = "" };

        var response = await ApiTestHelpers.SendJsonAsync(_client, HttpMethod.Post, "/api/song", dto);

        await ApiTestHelpers.AssertBadRequestAsync(response);
    }

    [Fact]
    public async Task Put_WithValidData_UpdatesSong()
    {
        await _factory.ResetDatabaseAsync();
        var albumId = await ApiTestHelpers.GetAlbumIdAsync(_factory, "Ride the Lightning");
        var create = new SongDTO
        {
            Title = "Update Song",
            DurationSeconds = 260,
            AlbumId = albumId,
            OpeningLyric = "Original lyric",
            IsDailyQuizSong = false,
            AudioSnippetUrl = "https://example.com/original.mp3",
            AlbumCoverUrl = "https://example.com/original.jpg"
        };

        var createResponse = await ApiTestHelpers.SendJsonAsync(_client, HttpMethod.Post, "/api/song", create);
        var created = await ApiTestHelpers.ReadJsonAsync<SongDTO>(createResponse);

        created.Title = "Updated Song";
        created.DurationSeconds = 300;
        created.OpeningLyric = "Updated lyric";

        var updateResponse = await ApiTestHelpers.SendJsonAsync(_client, HttpMethod.Put, $"/api/song/{created.Id}", created);
        var updated = await ApiTestHelpers.ReadJsonAsync<SongDTO>(updateResponse);

        Assert.Equal(HttpStatusCode.OK, updateResponse.StatusCode);
        Assert.Equal("Updated Song", updated.Title);

        var getResponse = await _client.GetAsync($"/api/song/{created.Id}");
        var fetched = await getResponse.Content.ReadFromJsonAsync<SongDTO>();
        Assert.Equal("Updated Song", fetched!.Title);
    }

    [Fact]
    public async Task Delete_RemovesSong()
    {
        await _factory.ResetDatabaseAsync();
        var albumId = await ApiTestHelpers.GetAlbumIdAsync(_factory, "Ride the Lightning");
        var create = new SongDTO
        {
            Title = "Delete Song",
            DurationSeconds = 200,
            AlbumId = albumId,
            OpeningLyric = "Delete lyric",
            IsDailyQuizSong = false,
            AudioSnippetUrl = "https://example.com/delete.mp3",
            AlbumCoverUrl = "https://example.com/delete.jpg"
        };

        var createResponse = await ApiTestHelpers.SendJsonAsync(_client, HttpMethod.Post, "/api/song", create);
        var created = await ApiTestHelpers.ReadJsonAsync<SongDTO>(createResponse);

        var deleteResponse = await _client.DeleteAsync($"/api/song/{created.Id}");

        Assert.Equal(HttpStatusCode.NoContent, deleteResponse.StatusCode);
        var getResponse = await _client.GetAsync($"/api/song/{created.Id}");
        Assert.Equal(HttpStatusCode.NotFound, getResponse.StatusCode);
    }

    private async Task<int> GetSongIdAsync(string title)
    {
        using var scope = _factory.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<RiffdleDbContext>();
        return await db.Songs.Where(song => song.Title == title).Select(song => song.Id).FirstAsync();
    }
}
