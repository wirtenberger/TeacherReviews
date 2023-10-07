using System.Net;
using System.Text;
using System.Text.Json;
using TeacherReviews.API.Mapping;
using TeacherReviews.Domain;
using TeacherReviews.Domain.DTO;
using TeacherReviews.Domain.Entities;
using TeacherReviews.Domain.Exceptions;
using Xunit;

namespace TeacherReviews.API.Tests.Controllers;

public class CityControllerTests
{
    private readonly ApplicationFactory _applicationFactory;

    private readonly ApplicationDbContext _dbContext;

    public CityControllerTests()
    {
        _applicationFactory = new ApplicationFactory();
        _dbContext = _applicationFactory.Services.CreateScope().ServiceProvider.GetRequiredService<ApplicationDbContext>();
    }

    [Fact]
    public async Task GetCityById_ReturnsCity_WhenExists()
    {
        var id = Guid.NewGuid().ToString();
        var city = new City
        {
            Id = id,
            Name = "CityName",
        };

        await _dbContext.Cities.AddAsync(city);
        await _dbContext.SaveChangesAsync();

        var httpClient = _applicationFactory.CreateClient();

        var response = await httpClient.GetAsync($"api/City/get?id={id}");
        var responseString = await response.Content.ReadAsStringAsync();
        var cityDto = JsonSerializer.Deserialize<CityDto>(responseString, JsonDeserializeOptions.Default);

        Assert.Equivalent(HttpStatusCode.OK, response.StatusCode);
        Assert.Equivalent(city.ToDto(), cityDto);
    }

    [Fact]
    public async Task GetCityById_ReturnsBadRequest_WhenNotExists()
    {
        var httpClient = _applicationFactory.CreateClient();

        var response = await httpClient.GetAsync("api/City/get?id=NotExistingId");
        var responseString = await response.Content.ReadAsStringAsync();

        var exception = new EntityNotFoundException(typeof(City), "NotExistingId");

        Assert.Equivalent(HttpStatusCode.BadRequest, response.StatusCode);
        Assert.Equivalent(exception.Serialize(), responseString);
    }

    [Fact]
    public async Task Create_ReturnsCity_WhenCityWithSuchNameNotExists()
    {
        var httpClient = _applicationFactory.CreateClient();

        var response = await httpClient.PostAsync("api/City/create", new StringContent(
            JsonSerializer.Serialize(
                new { name = "cityName" }
            ), Encoding.UTF8, "application/json")
        );

        var responseString = await response.Content.ReadAsStringAsync();
        var cityDto = JsonSerializer.Deserialize<CityDto>(responseString, JsonDeserializeOptions.Default);

        Assert.Equivalent(HttpStatusCode.OK, response.StatusCode);
        Assert.Equivalent("cityName", cityDto.Name);
    }

    [Fact]
    public async Task Create_ReturnsBadRequest_WhenCityWithSuchNameExists()
    {
        var city = new City
        {
            Id = Guid.NewGuid().ToString(),
            Name = "cityName",
        };

        await _dbContext.Cities.AddAsync(city);
        await _dbContext.SaveChangesAsync();

        var httpClient = _applicationFactory.CreateClient();

        var response = await httpClient.PostAsync("api/City/create", new StringContent(
            JsonSerializer.Serialize(
                new { name = "cityName" }
            ), Encoding.UTF8, "application/json")
        );

        var responseString = await response.Content.ReadAsStringAsync();

        var exception = new EntityExistsException(typeof(City), nameof(City.Name), city.Name);

        Assert.Equivalent(HttpStatusCode.BadRequest, response.StatusCode);
        Assert.Equivalent(exception.Serialize(), responseString);
    }

    [Fact]
    public async Task Update_ReturnCity_WhenCityWithSuchNameNotExists()
    {
        var id = Guid.NewGuid().ToString();
        var city = new City
        {
            Id = id,
            Name = "CityName",
        };

        await _dbContext.Cities.AddAsync(city);
        await _dbContext.SaveChangesAsync();

        var httpClient = _applicationFactory.CreateClient();

        var response = await httpClient.PutAsync("api/City/update", new StringContent(
            JsonSerializer.Serialize(
                new { id, name = "newCityName" }
            ), Encoding.UTF8, "application/json")
        );

        var responseString = await response.Content.ReadAsStringAsync();
        var cityDto = JsonSerializer.Deserialize<CityDto>(responseString, JsonDeserializeOptions.Default);

        Assert.Equivalent(HttpStatusCode.OK, response.StatusCode);
        Assert.Equivalent("newCityName", cityDto.Name);
    }

    [Fact]
    public async Task Update_ReturnBadRequest_WhenCityWithSuchNameExists()
    {
        var id = Guid.NewGuid().ToString();
        var city = new City
        {
            Id = id,
            Name = "CityName",
        };

        var existingCity = new City
        {
            Id = Guid.NewGuid().ToString(),
            Name = "ExistingCityName",
        };

        await _dbContext.Cities.AddAsync(city);
        await _dbContext.Cities.AddAsync(existingCity);
        await _dbContext.SaveChangesAsync();

        var httpClient = _applicationFactory.CreateClient();

        var response = await httpClient.PutAsync("api/City/update", new StringContent(
            JsonSerializer.Serialize(
                new { id, name = "ExistingCityName" }
            ), Encoding.UTF8, "application/json")
        );

        var responseString = await response.Content.ReadAsStringAsync();

        var exception = new EntityExistsException(typeof(City), nameof(City.Name), "ExistingCityName");

        Assert.Equivalent(HttpStatusCode.BadRequest, response.StatusCode);
        Assert.Equivalent(exception.Serialize(), responseString);
    }

    [Fact]
    public async Task Delete_ReturnsCity_WhenCityExists()
    {
        var id = Guid.NewGuid().ToString();
        var city = new City
        {
            Id = id,
            Name = "CityName",
        };

        await _dbContext.Cities.AddAsync(city);
        await _dbContext.SaveChangesAsync();

        var httpClient = _applicationFactory.CreateClient();

        var response = await httpClient.DeleteAsync($"api/City/delete?id={id}");
        var responseString = await response.Content.ReadAsStringAsync();
        var cityDto = JsonSerializer.Deserialize<CityDto>(responseString, JsonDeserializeOptions.Default);

        Assert.Equivalent(HttpStatusCode.OK, response.StatusCode);
        Assert.Equivalent(city.ToDto(), cityDto);
    }

    [Fact]
    public async Task Delete_ReturnsBadRequest_WhenCityNotExists()
    {
        var httpClient = _applicationFactory.CreateClient();

        var response = await httpClient.DeleteAsync($"api/City/delete?id=NotExistingId");
        var responseString = await response.Content.ReadAsStringAsync();

        var exception = new EntityNotFoundException(typeof(City), "NotExistingId");
        Assert.Equivalent(HttpStatusCode.BadRequest, response.StatusCode);
        Assert.Equivalent(exception.Serialize(), responseString);
    }
}