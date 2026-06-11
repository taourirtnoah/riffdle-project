using System.Net;
using System.Net.Http.Json;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Riffdle.Models.Domain;
using Riffdle.Models.DTO;
using Xunit;

namespace Riffdle.Tests;

public class ApiCoverageTests : IClassFixture<ApiWebApplicationFactory>
{
    private readonly ApiWebApplicationFactory _factory;
    private readonly HttpClient _client;

    public ApiCoverageTests(ApiWebApplicationFactory factory)
    {
        _factory = factory;
        _client = factory.CreateClient();
    }

    [Theory]
    [InlineData("/api/genre", """{"id":999999,"name":"Missing","description":"Missing"}""")]
    [InlineData("/api/band", """{"id":999999,"name":"Missing","formedYear":2000,"country":"Croatia","description":"Missing","genreId":1}""")]
    [InlineData("/api/album", """{"id":999999,"title":"Missing","releaseYear":2000,"bandId":1}""")]
    [InlineData("/api/song", """{"id":999999,"title":"Missing","durationSeconds":180,"albumId":1,"openingLyric":"Missing","audioSnippetUrl":"https://example.com/a.mp3","albumCoverUrl":"https://example.com/a.jpg"}""")]
    public async Task ExistingCrudApis_ReturnNotFound_ForMissingPutAndDelete(string endpoint, string putJson)
    {
        await _factory.ResetDatabaseAsync();

        var putResponse = await _client.SendAsync(new HttpRequestMessage(HttpMethod.Put, $"{endpoint}/999999")
        {
            Content = new StringContent(putJson, System.Text.Encoding.UTF8, "application/json")
        });
        var deleteResponse = await _client.DeleteAsync($"{endpoint}/999999");

        Assert.Equal(HttpStatusCode.NotFound, putResponse.StatusCode);
        Assert.Equal(HttpStatusCode.NotFound, deleteResponse.StatusCode);
    }

    [Fact]
    public async Task ProtectedApiMutation_RequiresAuthenticatedAdmin()
    {
        await _factory.ResetDatabaseAsync();

        var anonymousClient = _factory.CreateClient();
        anonymousClient.DefaultRequestHeaders.Add("X-Test-Auth", "false");

        var forbiddenClient = _factory.CreateClient();
        forbiddenClient.DefaultRequestHeaders.Add("X-Test-Role", "User");

        var dto = new GenreDTO { Name = "Unauthorized Genre", Description = "Should not save" };

        var anonymousResponse = await anonymousClient.PostAsJsonAsync("/api/genre", dto);
        var forbiddenResponse = await forbiddenClient.PostAsJsonAsync("/api/genre", dto);

        Assert.Equal(HttpStatusCode.Unauthorized, anonymousResponse.StatusCode);
        Assert.Equal(HttpStatusCode.Forbidden, forbiddenResponse.StatusCode);
    }

    [Fact]
    public async Task ExistingCrudApis_FilterWithQuery()
    {
        await _factory.ResetDatabaseAsync();

        var response = await _client.GetAsync("/api/song?q=Puppets");
        var songs = await response.Content.ReadFromJsonAsync<List<SongDTO>>();

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.NotNull(songs);
        Assert.Contains(songs!, song => song.Title == "Master of Puppets");
        Assert.DoesNotContain(songs!, song => song.Title == "Battery");
    }

    [Fact]
    public async Task PlaylistApi_CoversCrudValidationAndMissingIds()
    {
        await _factory.ResetDatabaseAsync();

        var invalid = await _client.PostAsJsonAsync("/api/playlist", new UserPlaylistDTO());
        Assert.Equal(HttpStatusCode.BadRequest, invalid.StatusCode);

        var create = new UserPlaylistDTO
        {
            Name = "API Playlist",
            OwnerUserName = "api-user",
            Description = "Created through API",
            IsPublic = true
        };

        var createResponse = await _client.PostAsJsonAsync("/api/playlist", create);
        var created = await ApiTestHelpers.ReadJsonAsync<UserPlaylistDTO>(createResponse);
        Assert.Equal(HttpStatusCode.Created, createResponse.StatusCode);

        var getResponse = await _client.GetAsync($"/api/playlist/{created.Id}");
        Assert.Equal(HttpStatusCode.OK, getResponse.StatusCode);

        created.Description = "Updated through API";
        var updateResponse = await _client.PutAsJsonAsync($"/api/playlist/{created.Id}", created);
        Assert.Equal(HttpStatusCode.OK, updateResponse.StatusCode);

        var missingPut = await _client.PutAsJsonAsync("/api/playlist/999999", new UserPlaylistDTO
        {
            Id = 999999,
            Name = "Missing",
            OwnerUserName = "missing",
            Description = "Missing"
        });
        Assert.Equal(HttpStatusCode.NotFound, missingPut.StatusCode);

        var deleteResponse = await _client.DeleteAsync($"/api/playlist/{created.Id}");
        Assert.Equal(HttpStatusCode.NoContent, deleteResponse.StatusCode);

        var missingDelete = await _client.DeleteAsync("/api/playlist/999999");
        Assert.Equal(HttpStatusCode.NotFound, missingDelete.StatusCode);
    }

    [Fact]
    public async Task QuizRoundAndHintApis_CoverCrudValidationAndMissingIds()
    {
        await _factory.ResetDatabaseAsync();
        var songId = await GetSongIdAsync("Master of Puppets");

        var invalidRound = await _client.PostAsJsonAsync("/api/quiz-round", new QuizRoundDTO());
        Assert.Equal(HttpStatusCode.BadRequest, invalidRound.StatusCode);

        var roundResponse = await _client.PostAsJsonAsync("/api/quiz-round", new QuizRoundDTO { SongId = songId });
        var round = await ApiTestHelpers.ReadJsonAsync<QuizRoundDTO>(roundResponse);
        Assert.Equal(HttpStatusCode.Created, roundResponse.StatusCode);

        var invalidHint = await _client.PostAsJsonAsync("/api/hint", new HintDTO());
        Assert.Equal(HttpStatusCode.BadRequest, invalidHint.StatusCode);

        var hintResponse = await _client.PostAsJsonAsync("/api/hint", new HintDTO
        {
            QuizRoundId = round.Id,
            Type = HintType.BandName,
            Order = 1,
            Content = "Band initials"
        });
        var hint = await ApiTestHelpers.ReadJsonAsync<HintDTO>(hintResponse);
        Assert.Equal(HttpStatusCode.Created, hintResponse.StatusCode);

        hint.Content = "Updated hint";
        var hintUpdate = await _client.PutAsJsonAsync($"/api/hint/{hint.Id}", hint);
        Assert.Equal(HttpStatusCode.OK, hintUpdate.StatusCode);

        var roundUpdate = await _client.PutAsJsonAsync($"/api/quiz-round/{round.Id}", round);
        Assert.Equal(HttpStatusCode.OK, roundUpdate.StatusCode);

        var missingRoundPut = await _client.PutAsJsonAsync("/api/quiz-round/999999", new QuizRoundDTO { Id = 999999, SongId = songId });
        var missingHintPut = await _client.PutAsJsonAsync("/api/hint/999999", new HintDTO { Id = 999999, QuizRoundId = round.Id, Type = HintType.BandName, Content = "Missing" });
        Assert.Equal(HttpStatusCode.NotFound, missingRoundPut.StatusCode);
        Assert.Equal(HttpStatusCode.NotFound, missingHintPut.StatusCode);

        Assert.Equal(HttpStatusCode.NoContent, (await _client.DeleteAsync($"/api/hint/{hint.Id}")).StatusCode);
        Assert.Equal(HttpStatusCode.NoContent, (await _client.DeleteAsync($"/api/quiz-round/{round.Id}")).StatusCode);
        Assert.Equal(HttpStatusCode.NotFound, (await _client.DeleteAsync("/api/hint/999999")).StatusCode);
        Assert.Equal(HttpStatusCode.NotFound, (await _client.DeleteAsync("/api/quiz-round/999999")).StatusCode);
    }

    [Fact]
    public async Task AttachmentApi_CoversCrudValidationAndMissingIds()
    {
        await _factory.ResetDatabaseAsync();
        var songId = await GetSongIdAsync("Master of Puppets");

        var invalid = await _client.PostAsJsonAsync("/api/attachment", new AttachmentDTO());
        Assert.Equal(HttpStatusCode.BadRequest, invalid.StatusCode);

        var createResponse = await _client.PostAsJsonAsync("/api/attachment", new AttachmentDTO
        {
            SongId = songId,
            FileName = "stored.mp3",
            OriginalName = "original.mp3",
            ContentType = "audio/mpeg",
            Size = 12345,
            Url = "/uploads/songs/1/stored.mp3"
        });
        var created = await ApiTestHelpers.ReadJsonAsync<AttachmentDTO>(createResponse);
        Assert.Equal(HttpStatusCode.Created, createResponse.StatusCode);

        created.OriginalName = "renamed.mp3";
        Assert.Equal(HttpStatusCode.OK, (await _client.PutAsJsonAsync($"/api/attachment/{created.Id}", created)).StatusCode);

        var missingPut = await _client.PutAsJsonAsync("/api/attachment/999999", new AttachmentDTO
        {
            Id = 999999,
            SongId = songId,
            FileName = "missing.mp3",
            OriginalName = "missing.mp3",
            ContentType = "audio/mpeg",
            Url = "/uploads/missing.mp3"
        });
        Assert.Equal(HttpStatusCode.NotFound, missingPut.StatusCode);

        Assert.Equal(HttpStatusCode.NoContent, (await _client.DeleteAsync($"/api/attachment/{created.Id}")).StatusCode);
        Assert.Equal(HttpStatusCode.NotFound, (await _client.DeleteAsync("/api/attachment/999999")).StatusCode);
    }

    [Fact]
    public async Task PlaylistSongApi_CoversCrudValidationAndMissingIds()
    {
        await _factory.ResetDatabaseAsync();
        var songId = await GetSongIdAsync("Master of Puppets");
        var playlist = await CreatePlaylistAsync();

        var invalid = await _client.PostAsJsonAsync("/api/playlist-song", new PlaylistSongDTO());
        Assert.Equal(HttpStatusCode.BadRequest, invalid.StatusCode);

        var createResponse = await _client.PostAsJsonAsync("/api/playlist-song", new PlaylistSongDTO
        {
            PlaylistId = playlist.Id,
            SongId = songId
        });
        var created = await ApiTestHelpers.ReadJsonAsync<PlaylistSongDTO>(createResponse);
        Assert.Equal(HttpStatusCode.Created, createResponse.StatusCode);

        created.AddedAt = DateTime.UtcNow.AddMinutes(-5);
        Assert.Equal(HttpStatusCode.OK, (await _client.PutAsJsonAsync($"/api/playlist-song/{playlist.Id}/{songId}", created)).StatusCode);

        var missingPut = await _client.PutAsJsonAsync("/api/playlist-song/999999/999999", new PlaylistSongDTO
        {
            PlaylistId = 999999,
            SongId = 999999,
            AddedAt = DateTime.UtcNow
        });
        Assert.Equal(HttpStatusCode.NotFound, missingPut.StatusCode);

        Assert.Equal(HttpStatusCode.NoContent, (await _client.DeleteAsync($"/api/playlist-song/{playlist.Id}/{songId}")).StatusCode);
        Assert.Equal(HttpStatusCode.NotFound, (await _client.DeleteAsync("/api/playlist-song/999999/999999")).StatusCode);
    }

    private async Task<int> GetSongIdAsync(string title)
    {
        using var scope = _factory.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<Riffdle.Data.RiffdleDbContext>();
        return await db.Songs.Where(song => song.Title == title).Select(song => song.Id).FirstAsync();
    }

    private async Task<UserPlaylistDTO> CreatePlaylistAsync()
    {
        var response = await _client.PostAsJsonAsync("/api/playlist", new UserPlaylistDTO
        {
            Name = $"Playlist {Guid.NewGuid():N}",
            OwnerUserName = "api-user",
            Description = "Playlist for relation tests",
            IsPublic = true
        });

        return await ApiTestHelpers.ReadJsonAsync<UserPlaylistDTO>(response);
    }
}
