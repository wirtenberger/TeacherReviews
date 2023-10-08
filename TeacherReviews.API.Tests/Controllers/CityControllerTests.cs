using System.Collections.Generic;
using System.Net;
using System.Text;
using System.Text.Json;
using Bogus;
using TeacherReviews.API.Contracts.Requests.City;
using TeacherReviews.API.Mapping;
using TeacherReviews.API.Services;
using TeacherReviews.Domain.DTO;
using TeacherReviews.Domain.Entities;
using TeacherReviews.Domain.Exceptions;
using Xunit;

namespace TeacherReviews.API.Tests.Controllers;

public class CityControllerTests
{
    private readonly ApplicationFactory _applicationFactory;

    private readonly CityService _cityService;

    private readonly UniversityService _universityService;

    private readonly Faker<City> _cityFaker = Fakers.CityFaker;

    private readonly Faker<University> _universityFaker = Fakers.UniversityFaker;

    public CityControllerTests()
    {
        _applicationFactory = new ApplicationFactory();
        var scope = _applicationFactory.Services.CreateScope();
        _cityService = scope.ServiceProvider.GetRequiredService<CityService>();
        _universityService = scope.ServiceProvider.GetRequiredService<UniversityService>();
    }

    [Fact]
    public async Task GetCityById_ReturnsCity_WhenExists()
    {
        var city = _cityFaker.Generate();

        await _cityService.CreateAsync(city);

        var httpClient = _applicationFactory.CreateClient();

        var response = await httpClient.GetAsync($"api/City/get?id={city.Id}");
        var responseString = await response.Content.ReadAsStringAsync();
        var cityDto = JsonSerializer.Deserialize<CityDto>(responseString, JsonDefaultOptions.DeserializeOptions);

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

        var city = _cityFaker.Generate();

        var response = await httpClient.PostAsync("api/City/create", new StringContent(
            JsonSerializer.Serialize(
                new CreateCityRequest { Name = city.Name }, JsonDefaultOptions.SerializeOptions
            ), Encoding.UTF8, "application/json")
        );

        var responseString = await response.Content.ReadAsStringAsync();
        var cityDto = JsonSerializer.Deserialize<CityDto>(responseString, JsonDefaultOptions.DeserializeOptions);

        Assert.Equivalent(HttpStatusCode.OK, response.StatusCode);
        Assert.Equivalent(city.Name, cityDto.Name);
    }

    [Fact]
    public async Task Create_ReturnsBadRequest_WhenCityWithSuchNameExists()
    {
        var city = _cityFaker.Generate();

        await _cityService.CreateAsync(city);

        var httpClient = _applicationFactory.CreateClient();

        var response = await httpClient.PostAsync("api/City/create", new StringContent(
            JsonSerializer.Serialize(
                new CreateCityRequest { Name = city.Name }, JsonDefaultOptions.SerializeOptions
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
        var city = _cityFaker.Generate();
        var newCityName = Fakers.Faker.Address.City();

        await _cityService.CreateAsync(city);

        var httpClient = _applicationFactory.CreateClient();


        var response = await httpClient.PutAsync("api/City/update", new StringContent(
            JsonSerializer.Serialize(
                new UpdateCityRequest { Id = city.Id, Name = newCityName }
            ), Encoding.UTF8, "application/json")
        );

        var responseString = await response.Content.ReadAsStringAsync();
        var cityDto = JsonSerializer.Deserialize<CityDto>(responseString, JsonDefaultOptions.DeserializeOptions);

        Assert.Equivalent(HttpStatusCode.OK, response.StatusCode);
        Assert.Equivalent(newCityName, cityDto.Name);
    }

    [Fact]
    public async Task Update_ReturnBadRequest_WhenCityWithSuchNameExists()
    {
        var city = _cityFaker.Generate();

        var existingCity = _cityFaker.Generate();

        await _cityService.CreateAsync(city);
        await _cityService.CreateAsync(existingCity);

        var httpClient = _applicationFactory.CreateClient();

        var response = await httpClient.PutAsync("api/City/update", new StringContent(
            JsonSerializer.Serialize(
                new UpdateCityRequest { Id = city.Id, Name = existingCity.Name }
            ), Encoding.UTF8, "application/json")
        );

        var responseString = await response.Content.ReadAsStringAsync();

        var exception = new EntityExistsException(typeof(City), nameof(City.Name), existingCity.Name);

        Assert.Equivalent(HttpStatusCode.BadRequest, response.StatusCode);
        Assert.Equivalent(exception.Serialize(), responseString);
    }

    [Fact]
    public async Task Update_ReturnsBadRequest_WhenCityNotExists()
    {
        var httpClient = _applicationFactory.CreateClient();

        var response = await httpClient.PutAsync("api/City/update", new StringContent(
            JsonSerializer.Serialize(
                new UpdateCityRequest { Id = "NotExistingId", Name = "Name" }
            ), Encoding.UTF8, "application/json")
        );

        var responseString = await response.Content.ReadAsStringAsync();

        var exception = new EntityNotFoundException(typeof(City), "NotExistingId");

        Assert.Equivalent(HttpStatusCode.BadRequest, response.StatusCode);
        Assert.Equivalent(exception.Serialize(), responseString);
    }

    [Fact]
    public async Task Delete_ReturnsCity_WhenCityExists()
    {
        var city = _cityFaker.Generate();
        await _cityService.CreateAsync(city);

        var httpClient = _applicationFactory.CreateClient();

        var response = await httpClient.DeleteAsync($"api/City/delete?id={city.Id}");
        var responseString = await response.Content.ReadAsStringAsync();
        var cityDto = JsonSerializer.Deserialize<CityDto>(responseString, JsonDefaultOptions.DeserializeOptions);

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

    [Fact]
    public async Task GetUniversities_ReturnsListOfUniversities_WhenCityExists()
    {
        var city = _cityFaker.Generate();
        await _cityService.CreateAsync(city);

        var list = new List<UniversityDto>();

        for (var i = 0; i < 3; i++)
        {
            var university = _universityFaker.Generate();
            university.CityId = city.Id;
            list.Add(university.ToDto());
            await _universityService.CreateAsync(university);
        }

        var httpClient = _applicationFactory.CreateClient();

        var response = await httpClient.GetAsync($"api/City/universities?id={city.Id}");
        var responseString = await response.Content.ReadAsStringAsync();
        var universitiesDtoList = JsonSerializer.Deserialize<List<UniversityDto>>(responseString, JsonDefaultOptions.DeserializeOptions)!;

        Assert.Equivalent(HttpStatusCode.OK, response.StatusCode);
        Assert.Equivalent(list, universitiesDtoList);
    }

    [Fact]
    public async Task GetUniversities_ReturnsListOfUniversities_WhenCityNotExists()
    {
        var httpClient = _applicationFactory.CreateClient();

        var response = await httpClient.GetAsync($"api/City/universities?id=NotExistingId");
        var responseString = await response.Content.ReadAsStringAsync();

        var exception = new EntityNotFoundException(typeof(City), "NotExistingId");
        Assert.Equivalent(HttpStatusCode.BadRequest, response.StatusCode);
        Assert.Equivalent(exception.Serialize(), responseString);
    }
}