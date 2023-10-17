using System.Net;
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

    private Faker<City> CityFaker => Fakers.CityFaker;

    private Faker<University> UniversityFaker => Fakers.UniversityFaker;

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
        var city = CityFaker.Generate();

        await _cityService.CreateAsync(city);

        var httpClient = _applicationFactory.CreateClient();
        var response = await httpClient.GetAsync($"api/City/get?id={city.Id}");
        var cityDto = await response.Content.ReadFromJsonAsync<CityDto>();

        Assert.Equivalent(HttpStatusCode.OK, response.StatusCode);
        Assert.Equivalent(city.ToDto(), cityDto);
    }

    [Fact]
    public async Task GetCityById_ReturnsBadRequest_WhenNotExists()
    {
        var id = "NotExistingId";
        var expectedException = new EntityNotFoundException(typeof(City), id).AsExceptionResponse();

        var httpClient = _applicationFactory.CreateClient();

        var response = await httpClient.GetAsync($"api/City/get?id={id}");
        var exception = await response.Content.ReadFromJsonAsync<ExceptionResponse>();

        Assert.Equivalent(HttpStatusCode.BadRequest, response.StatusCode);
        Assert.Equivalent(expectedException, exception);
    }

    [Fact]
    public async Task Create_ReturnsCity_WhenCityWithSuchNameNotExists()
    {
        var httpClient = _applicationFactory.CreateClient();

        var city = CityFaker.Generate();

        var response = await httpClient.PostAsJsonAsync("api/City/create",
            new CreateCityRequest
            {
                Name = city.Name,
            }
        );
        var cityDto = await response.Content.ReadFromJsonAsync<CityDto>();

        Assert.Equivalent(HttpStatusCode.OK, response.StatusCode);
        Assert.Equivalent(city.Name, cityDto!.Name);
    }

    [Fact]
    public async Task Create_ReturnsBadRequest_WhenCityWithSuchNameExists()
    {
        var city = CityFaker.Generate();
        await _cityService.CreateAsync(city);
        var expectedException =
            new EntityExistsException(typeof(City), nameof(City.Name), city.Name).AsExceptionResponse();

        var httpClient = _applicationFactory.CreateClient();
        var response = await httpClient.PostAsJsonAsync("api/City/create",
            new CreateCityRequest
            {
                Name = city.Name,
            }
        );
        var exception = await response.Content.ReadFromJsonAsync<ExceptionResponse>();

        Assert.Equivalent(HttpStatusCode.BadRequest, response.StatusCode);
        Assert.Equivalent(expectedException, exception);
    }

    [Fact]
    public async Task Update_ReturnCity_WhenCityWithSuchNameNotExists()
    {
        var city = CityFaker.Generate();
        var newCityName = Fakers.Faker.Address.City();
        await _cityService.CreateAsync(city);

        var httpClient = _applicationFactory.CreateClient();
        var response = await httpClient.PutAsJsonAsync("api/City/update",
            new UpdateCityRequest
            {
                Id = city.Id,
                Name = newCityName,
            }
        );
        var cityDto = await response.Content.ReadFromJsonAsync<CityDto>();

        Assert.Equivalent(HttpStatusCode.OK, response.StatusCode);
        Assert.Equivalent(newCityName, cityDto!.Name);
    }

    [Fact]
    public async Task Update_ReturnBadRequest_WhenCityWithSuchNameExists()
    {
        var city = CityFaker.Generate();
        var existingCity = CityFaker.Generate();
        await _cityService.CreateAsync(city);
        await _cityService.CreateAsync(existingCity);
        var expectedException = new EntityExistsException(typeof(City), nameof(City.Name), existingCity.Name)
            .AsExceptionResponse();

        var httpClient = _applicationFactory.CreateClient();
        var response = await httpClient.PutAsJsonAsync("api/City/update",
            new UpdateCityRequest
            {
                Id = city.Id,
                Name = existingCity.Name,
            }
        );
        var exception = await response.Content.ReadFromJsonAsync<ExceptionResponse>();

        Assert.Equivalent(HttpStatusCode.BadRequest, response.StatusCode);
        Assert.Equivalent(expectedException, exception);
    }

    [Fact]
    public async Task Update_ReturnsBadRequest_WhenCityNotExists()
    {
        var id = "NotExistingId";
        var expectedException = new EntityNotFoundException(typeof(City), id).AsExceptionResponse();

        var httpClient = _applicationFactory.CreateClient();
        var response = await httpClient.PutAsJsonAsync("api/City/update",
            new UpdateCityRequest
            {
                Id = id,
                Name = "Name",
            }
        );
        var exception = await response.Content.ReadFromJsonAsync<ExceptionResponse>();

        Assert.Equivalent(HttpStatusCode.BadRequest, response.StatusCode);
        Assert.Equivalent(expectedException, exception);
    }

    [Fact]
    public async Task Delete_ReturnsCity_WhenCityExists()
    {
        var city = CityFaker.Generate();
        await _cityService.CreateAsync(city);

        var httpClient = _applicationFactory.CreateClient();

        var response = await httpClient.DeleteAsync($"api/City/delete?id={city.Id}");
        var cityDto = await response.Content.ReadFromJsonAsync<CityDto>();

        Assert.Equivalent(HttpStatusCode.OK, response.StatusCode);
        Assert.Equivalent(city.ToDto(), cityDto);
    }

    [Fact]
    public async Task Delete_ReturnsBadRequest_WhenCityNotExists()
    {
        var id = "NotExistingId";
        var expectedException = new EntityNotFoundException(typeof(City), id).AsExceptionResponse();

        var httpClient = _applicationFactory.CreateClient();

        var response = await httpClient.DeleteAsync($"api/City/delete?id={id}");
        var exception = await response.Content.ReadFromJsonAsync<ExceptionResponse>();

        Assert.Equivalent(HttpStatusCode.BadRequest, response.StatusCode);
        Assert.Equivalent(expectedException, exception);
    }

    [Fact]
    public async Task GetUniversities_ReturnsListOfUniversities_WhenCityExists()
    {
        var city = CityFaker.Generate();
        await _cityService.CreateAsync(city);

        var list = UniversityFaker.Generate(3);
        foreach (var university in list)
        {
            university.City = city;
            university.CityId = city.Id;
            await _universityService.CreateAsync(university);
        }

        var httpClient = _applicationFactory.CreateClient();
        var response = await httpClient.GetAsync($"api/City/universities?id={city.Id}");
        var universitiesDtoList = await response.Content.ReadFromJsonAsync<List<UniversityDto>>();

        Assert.Equivalent(HttpStatusCode.OK, response.StatusCode);
        Assert.Equivalent(list.Select(u => u.ToDto()).ToList(), universitiesDtoList);
    }

    [Fact]
    public async Task GetUniversities_ReturnsListOfUniversities_WhenCityNotExists()
    {
        var id = "NotExistingId";
        var expectedException = new EntityNotFoundException(typeof(City), id).AsExceptionResponse();

        var httpClient = _applicationFactory.CreateClient();

        var response = await httpClient.GetAsync($"api/City/universities?id={id}");
        var exception = await response.Content.ReadFromJsonAsync<ExceptionResponse>();

        Assert.Equivalent(HttpStatusCode.BadRequest, response.StatusCode);
        Assert.Equivalent(expectedException, exception);
    }
}