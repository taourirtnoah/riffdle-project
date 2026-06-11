using System.Net;
using System.Net.Http.Json;
using Riffdle.Models.DTO;
using Xunit;

namespace Riffdle.Tests;

public class BandApiTests : IClassFixture<ApiWebApplicationFactory>
{
    private readonly ApiWebApplicationFactory _factory;
    private readonly HttpClient _client;

    public BandApiTests(ApiWebApplicationFactory factory)
    {
        _factory = factory;
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task GetAll_ReturnsSeededBands()
    {
        await _factory.ResetDatabaseAsync();

        var response = await _client.GetAsync("/api/band");
        var bands = await response.Content.ReadFromJsonAsync<List<BandDTO>>();

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.NotNull(bands);
        Assert.Contains(bands!, band => band.Name == "Metallica");
    }

    [Fact]
    public async Task GetById_ReturnsBand()
    {
        await _factory.ResetDatabaseAsync();
        var bandId = await ApiTestHelpers.GetBandIdAsync(_factory, "Metallica");

        var response = await _client.GetAsync($"/api/band/{bandId}");
        var band = await response.Content.ReadFromJsonAsync<BandDTO>();

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.NotNull(band);
        Assert.Equal(bandId, band!.Id);
        Assert.Equal("Metallica", band.Name);
    }

    [Fact]
    public async Task GetById_WhenBandDoesNotExist_ReturnsNotFound()
    {
        await _factory.ResetDatabaseAsync();

        var response = await _client.GetAsync("/api/band/999999");

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task Post_WithValidData_CreatesBand()
    {
        await _factory.ResetDatabaseAsync();
        var genreId = await ApiTestHelpers.GetGenreIdAsync(_factory, "Thrash Metal");
        var dto = new BandDTO
        {
            Name = "Test Band",
            FormedYear = 1999,
            Country = "Croatia",
            Description = "Test description",
            GenreId = genreId
        };

        var response = await ApiTestHelpers.SendJsonAsync(_client, HttpMethod.Post, "/api/band", dto);
        var created = await ApiTestHelpers.ReadJsonAsync<BandDTO>(response);

        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        Assert.True(created.Id > 0);
        Assert.Equal(dto.Name, created.Name);

        var getResponse = await _client.GetAsync($"/api/band/{created.Id}");
        Assert.Equal(HttpStatusCode.OK, getResponse.StatusCode);
    }

    [Fact]
    public async Task Post_WithInvalidData_ReturnsBadRequest()
    {
        await _factory.ResetDatabaseAsync();
        var dto = new BandDTO { Name = string.Empty, FormedYear = 1700, Country = string.Empty, Description = string.Empty, GenreId = 0 };

        var response = await ApiTestHelpers.SendJsonAsync(_client, HttpMethod.Post, "/api/band", dto);

        await ApiTestHelpers.AssertBadRequestAsync(response);
    }

    [Fact]
    public async Task Put_WithValidData_UpdatesBand()
    {
        await _factory.ResetDatabaseAsync();
        var genreId = await ApiTestHelpers.GetGenreIdAsync(_factory, "Thrash Metal");
        var create = new BandDTO
        {
            Name = "Update Band",
            FormedYear = 2001,
            Country = "Croatia",
            Description = "Original",
            GenreId = genreId
        };

        var createResponse = await ApiTestHelpers.SendJsonAsync(_client, HttpMethod.Post, "/api/band", create);
        var created = await ApiTestHelpers.ReadJsonAsync<BandDTO>(createResponse);

        created.Name = "Updated Band";
        created.Description = "Updated description";

        var updateResponse = await ApiTestHelpers.SendJsonAsync(_client, HttpMethod.Put, $"/api/band/{created.Id}", created);
        var updated = await ApiTestHelpers.ReadJsonAsync<BandDTO>(updateResponse);

        Assert.Equal(HttpStatusCode.OK, updateResponse.StatusCode);
        Assert.Equal("Updated Band", updated.Name);

        var getResponse = await _client.GetAsync($"/api/band/{created.Id}");
        var fetched = await getResponse.Content.ReadFromJsonAsync<BandDTO>();
        Assert.Equal("Updated Band", fetched!.Name);
    }

    [Fact]
    public async Task Delete_RemovesBand()
    {
        await _factory.ResetDatabaseAsync();
        var genreId = await ApiTestHelpers.GetGenreIdAsync(_factory, "Thrash Metal");
        var create = new BandDTO
        {
            Name = "Delete Band",
            FormedYear = 2005,
            Country = "Croatia",
            Description = "Delete me",
            GenreId = genreId
        };

        var createResponse = await ApiTestHelpers.SendJsonAsync(_client, HttpMethod.Post, "/api/band", create);
        var created = await ApiTestHelpers.ReadJsonAsync<BandDTO>(createResponse);

        var deleteResponse = await _client.DeleteAsync($"/api/band/{created.Id}");

        Assert.Equal(HttpStatusCode.NoContent, deleteResponse.StatusCode);
        var getResponse = await _client.GetAsync($"/api/band/{created.Id}");
        Assert.Equal(HttpStatusCode.NotFound, getResponse.StatusCode);
    }
}
