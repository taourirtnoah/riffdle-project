using System.Net;
using System.Net.Http.Json;
using Riffdle.Models.DTO;
using Xunit;

namespace Riffdle.Tests;

public class GenreApiTests : IClassFixture<ApiWebApplicationFactory>
{
    private readonly ApiWebApplicationFactory _factory;
    private readonly HttpClient _client;

    public GenreApiTests(ApiWebApplicationFactory factory)
    {
        _factory = factory;
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task GetAll_ReturnsSeededGenres()
    {
        await _factory.ResetDatabaseAsync();

        var response = await _client.GetAsync("/api/genre");
        var genres = await response.Content.ReadFromJsonAsync<List<GenreDTO>>();

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.NotNull(genres);
        Assert.Contains(genres!, genre => genre.Name == "Thrash Metal");
    }

    [Fact]
    public async Task GetById_ReturnsGenre()
    {
        await _factory.ResetDatabaseAsync();
        var genreId = await ApiTestHelpers.GetGenreIdAsync(_factory, "Thrash Metal");

        var response = await _client.GetAsync($"/api/genre/{genreId}");
        var genre = await response.Content.ReadFromJsonAsync<GenreDTO>();

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.NotNull(genre);
        Assert.Equal(genreId, genre!.Id);
        Assert.Equal("Thrash Metal", genre.Name);
    }

    [Fact]
    public async Task GetById_WhenGenreDoesNotExist_ReturnsNotFound()
    {
        await _factory.ResetDatabaseAsync();

        var response = await _client.GetAsync("/api/genre/999999");

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task Post_WithValidData_CreatesGenre()
    {
        await _factory.ResetDatabaseAsync();
        var dto = new GenreDTO { Name = "Blackened Death", Description = "Test genre" };

        var response = await ApiTestHelpers.SendJsonAsync(_client, HttpMethod.Post, "/api/genre", dto);
        var created = await ApiTestHelpers.ReadJsonAsync<GenreDTO>(response);

        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        Assert.True(created.Id > 0);
        Assert.Equal(dto.Name, created.Name);

        var getResponse = await _client.GetAsync($"/api/genre/{created.Id}");
        Assert.Equal(HttpStatusCode.OK, getResponse.StatusCode);
    }

    [Fact]
    public async Task Post_WithInvalidData_ReturnsBadRequest()
    {
        await _factory.ResetDatabaseAsync();
        var dto = new GenreDTO { Name = string.Empty };

        var response = await ApiTestHelpers.SendJsonAsync(_client, HttpMethod.Post, "/api/genre", dto);

        await ApiTestHelpers.AssertBadRequestAsync(response);
    }

    [Fact]
    public async Task Put_WithValidData_UpdatesGenre()
    {
        await _factory.ResetDatabaseAsync();
        var dto = new GenreDTO { Name = "Progressive Doom", Description = "Original" };
        var createResponse = await ApiTestHelpers.SendJsonAsync(_client, HttpMethod.Post, "/api/genre", dto);
        var created = await ApiTestHelpers.ReadJsonAsync<GenreDTO>(createResponse);

        created.Name = "Progressive Doom Metal";
        created.Description = "Updated";

        var updateResponse = await ApiTestHelpers.SendJsonAsync(_client, HttpMethod.Put, $"/api/genre/{created.Id}", created);
        var updated = await ApiTestHelpers.ReadJsonAsync<GenreDTO>(updateResponse);

        Assert.Equal(HttpStatusCode.OK, updateResponse.StatusCode);
        Assert.Equal("Progressive Doom Metal", updated.Name);

        var getResponse = await _client.GetAsync($"/api/genre/{created.Id}");
        var fetched = await getResponse.Content.ReadFromJsonAsync<GenreDTO>();
        Assert.Equal("Progressive Doom Metal", fetched!.Name);
    }

    [Fact]
    public async Task Delete_RemovesGenre()
    {
        await _factory.ResetDatabaseAsync();
        var dto = new GenreDTO { Name = "Speed Metal", Description = "Delete me" };
        var createResponse = await ApiTestHelpers.SendJsonAsync(_client, HttpMethod.Post, "/api/genre", dto);
        var created = await ApiTestHelpers.ReadJsonAsync<GenreDTO>(createResponse);

        var deleteResponse = await _client.DeleteAsync($"/api/genre/{created.Id}");

        Assert.Equal(HttpStatusCode.NoContent, deleteResponse.StatusCode);
        var getResponse = await _client.GetAsync($"/api/genre/{created.Id}");
        Assert.Equal(HttpStatusCode.NotFound, getResponse.StatusCode);
    }
}
