using System.Net;
using System.Net.Http.Json;
using Riffdle.Models.DTO;
using Xunit;

namespace Riffdle.Tests;

public class AlbumApiTests : IClassFixture<ApiWebApplicationFactory>
{
    private readonly ApiWebApplicationFactory _factory;
    private readonly HttpClient _client;

    public AlbumApiTests(ApiWebApplicationFactory factory)
    {
        _factory = factory;
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task GetAll_ReturnsSeededAlbums()
    {
        await _factory.ResetDatabaseAsync();

        var response = await _client.GetAsync("/api/album");
        var albums = await response.Content.ReadFromJsonAsync<List<AlbumDTO>>();

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.NotNull(albums);
        Assert.Contains(albums!, album => album.Title == "Ride the Lightning");
    }

    [Fact]
    public async Task GetById_ReturnsAlbum()
    {
        await _factory.ResetDatabaseAsync();
        var albumId = await ApiTestHelpers.GetAlbumIdAsync(_factory, "Ride the Lightning");

        var response = await _client.GetAsync($"/api/album/{albumId}");
        var album = await response.Content.ReadFromJsonAsync<AlbumDTO>();

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.NotNull(album);
        Assert.Equal(albumId, album!.Id);
        Assert.Equal("Ride the Lightning", album.Title);
    }

    [Fact]
    public async Task GetById_WhenAlbumDoesNotExist_ReturnsNotFound()
    {
        await _factory.ResetDatabaseAsync();

        var response = await _client.GetAsync("/api/album/999999");

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task Post_WithValidData_CreatesAlbum()
    {
        await _factory.ResetDatabaseAsync();
        var bandId = await ApiTestHelpers.GetBandIdAsync(_factory, "Metallica");
        var dto = new AlbumDTO
        {
            Title = "Test Album",
            ReleaseYear = 2020,
            BandId = bandId
        };

        var response = await ApiTestHelpers.SendJsonAsync(_client, HttpMethod.Post, "/api/album", dto);
        var created = await ApiTestHelpers.ReadJsonAsync<AlbumDTO>(response);

        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        Assert.True(created.Id > 0);
        Assert.Equal(dto.Title, created.Title);

        var getResponse = await _client.GetAsync($"/api/album/{created.Id}");
        Assert.Equal(HttpStatusCode.OK, getResponse.StatusCode);
    }

    [Fact]
    public async Task Post_WithInvalidData_ReturnsBadRequest()
    {
        await _factory.ResetDatabaseAsync();
        var dto = new AlbumDTO { Title = string.Empty, ReleaseYear = 1800, BandId = 0 };

        var response = await ApiTestHelpers.SendJsonAsync(_client, HttpMethod.Post, "/api/album", dto);

        await ApiTestHelpers.AssertBadRequestAsync(response);
    }

    [Fact]
    public async Task Put_WithValidData_UpdatesAlbum()
    {
        await _factory.ResetDatabaseAsync();
        var bandId = await ApiTestHelpers.GetBandIdAsync(_factory, "Metallica");
        var create = new AlbumDTO
        {
            Title = "Update Album",
            ReleaseYear = 2021,
            BandId = bandId
        };

        var createResponse = await ApiTestHelpers.SendJsonAsync(_client, HttpMethod.Post, "/api/album", create);
        var created = await ApiTestHelpers.ReadJsonAsync<AlbumDTO>(createResponse);

        created.Title = "Updated Album";
        created.ReleaseYear = 2022;

        var updateResponse = await ApiTestHelpers.SendJsonAsync(_client, HttpMethod.Put, $"/api/album/{created.Id}", created);
        var updated = await ApiTestHelpers.ReadJsonAsync<AlbumDTO>(updateResponse);

        Assert.Equal(HttpStatusCode.OK, updateResponse.StatusCode);
        Assert.Equal("Updated Album", updated.Title);

        var getResponse = await _client.GetAsync($"/api/album/{created.Id}");
        var fetched = await getResponse.Content.ReadFromJsonAsync<AlbumDTO>();
        Assert.Equal("Updated Album", fetched!.Title);
    }

    [Fact]
    public async Task Delete_RemovesAlbum()
    {
        await _factory.ResetDatabaseAsync();
        var bandId = await ApiTestHelpers.GetBandIdAsync(_factory, "Metallica");
        var create = new AlbumDTO
        {
            Title = "Delete Album",
            ReleaseYear = 2023,
            BandId = bandId
        };

        var createResponse = await ApiTestHelpers.SendJsonAsync(_client, HttpMethod.Post, "/api/album", create);
        var created = await ApiTestHelpers.ReadJsonAsync<AlbumDTO>(createResponse);

        var deleteResponse = await _client.DeleteAsync($"/api/album/{created.Id}");

        Assert.Equal(HttpStatusCode.NoContent, deleteResponse.StatusCode);
        var getResponse = await _client.GetAsync($"/api/album/{created.Id}");
        Assert.Equal(HttpStatusCode.NotFound, getResponse.StatusCode);
    }
}
