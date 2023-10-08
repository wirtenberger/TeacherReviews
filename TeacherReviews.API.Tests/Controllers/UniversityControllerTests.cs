using Bogus;
using System.Net;
using System.Text;
using System.Text.Json;
using TeacherReviews.API.Contracts.Requests.University;
using TeacherReviews.API.Mapping;
using TeacherReviews.API.Services;
using TeacherReviews.Domain.DTO;
using TeacherReviews.Domain.Entities;
using TeacherReviews.Domain.Exceptions;
using Xunit;

namespace TeacherReviews.API.Tests.Controllers;

public class UniversityControllerTests
{
    private readonly ApplicationFactory _applicationFactory;

    private readonly CityService _cityService;

    private readonly UniversityService _universityService;

    private readonly TeacherService _teacherService;

    private readonly Faker<City> _cityFaker = Fakers.CityFaker;

    private readonly Faker<University> _universityFaker = Fakers.UniversityFaker;

    private readonly Faker<Teacher> _teacherFaker = Fakers.TeacherFaker;


    public UniversityControllerTests()
    {
        _applicationFactory = new ApplicationFactory();
        var scope = _applicationFactory.Services.CreateScope();
        _cityService = scope.ServiceProvider.GetRequiredService<CityService>();
        _universityService = scope.ServiceProvider.GetRequiredService<UniversityService>();
        _teacherService = scope.ServiceProvider.GetRequiredService<TeacherService>();
    }

    [Fact]
    public async Task GetUniversityById_ReturnsUniversity_WhenUniversityExists()
    {
        var city = _cityFaker.Generate();
        await _cityService.CreateAsync(city);

        var university = _universityFaker.Generate();
        university.CityId = city.Id;
        await _universityService.CreateAsync(university);

        var httpClient = _applicationFactory.CreateClient();

        var response = await httpClient.GetAsync($"api/University/get?id={university.Id}");
        var responseString = await response.Content.ReadAsStringAsync();
        var universityDto = JsonSerializer.Deserialize<UniversityDto>(responseString, JsonDefaultOptions.DeserializeOptions);

        Assert.Equivalent(HttpStatusCode.OK, response.StatusCode);
        Assert.Equivalent(university.ToDto(), universityDto);
    }

    [Fact]
    public async Task GetUniversityById_ReturnsBadRequest_WhenUniversityNotExists()
    {
        var httpClient = _applicationFactory.CreateClient();

        var response = await httpClient.GetAsync($"api/University/get?id=NotExistingId");
        var responseString = await response.Content.ReadAsStringAsync();

        var exception = new EntityNotFoundException(typeof(University), "NotExistingId");

        Assert.Equivalent(HttpStatusCode.BadRequest, response.StatusCode);
        Assert.Equivalent(exception.Serialize(), responseString);
    }

    [Fact]
    public async Task CreateUniversity_ReturnsUniversity_WhenCityExistsAndUniversityWithSuchNameNotExists()
    {
        var city = _cityFaker.Generate();
        await _cityService.CreateAsync(city);

        var university = _universityFaker.Generate();
        university.CityId = city.Id;

        var httpClient = _applicationFactory.CreateClient();

        var response = await httpClient.PostAsync("api/University/create", new StringContent(
            JsonSerializer.Serialize(
                new CreateUniversityRequest { Name = university.Name, Abbreviation = university.Abbreviation, CityId = university.CityId },
                JsonDefaultOptions.SerializeOptions
            ), Encoding.UTF8, "application/json"
        ));

        var responseString = await response.Content.ReadAsStringAsync();
        var universityDto = JsonSerializer.Deserialize<UniversityDto>(responseString, JsonDefaultOptions.DeserializeOptions);

        university.Id = universityDto.Id;

        Assert.Equivalent(HttpStatusCode.OK, response.StatusCode);
        Assert.Equivalent(university.ToDto(), universityDto);
    }

    [Fact]
    public async Task CreateUniversity_ReturnsBadRequest_WhenCityNotExists()
    {
        var university = _universityFaker.Generate();
        university.CityId = "NotExistingId";

        var httpClient = _applicationFactory.CreateClient();

        var response = await httpClient.PostAsync("api/University/create", new StringContent(
            JsonSerializer.Serialize(
                new CreateUniversityRequest { Name = university.Name, Abbreviation = university.Abbreviation, CityId = university.CityId },
                JsonDefaultOptions.SerializeOptions
            ), Encoding.UTF8, "application/json"
        ));

        var responseString = await response.Content.ReadAsStringAsync();
        var exception = new EntityNotFoundException(typeof(City), university.CityId);

        Assert.Equivalent(HttpStatusCode.BadRequest, response.StatusCode);
        Assert.Equivalent(exception.Serialize(), responseString);
    }

    [Fact]
    public async Task CreateUniversity_ReturnsBadRequest_WhenUniversityWithSuchNameExists()
    {
        var city = _cityFaker.Generate();
        await _cityService.CreateAsync(city);

        var university = _universityFaker.Generate();
        university.CityId = city.Id;

        await _universityService.CreateAsync(university);

        var httpClient = _applicationFactory.CreateClient();

        var response = await httpClient.PostAsync("api/University/create", new StringContent(
            JsonSerializer.Serialize(
                new CreateUniversityRequest
                {
                    Name = university.Name, Abbreviation = university.Abbreviation, CityId = university.CityId,
                }, JsonDefaultOptions.SerializeOptions
            ), Encoding.UTF8, "application/json"
        ));

        var responseString = await response.Content.ReadAsStringAsync();

        var exception = new EntityExistsException(typeof(University), nameof(University.Name), university.Name);

        Assert.Equivalent(HttpStatusCode.BadRequest, response.StatusCode);
        Assert.Equivalent(exception.Serialize(), responseString);
    }

    [Fact]
    public async Task UpdateUniversity_ReturnsUniversity_WhenCityExistsAndUniversityWithSuchNameNotExists()
    {
        var city = _cityFaker.Generate();
        var city2 = _cityFaker.Generate();

        await _cityService.CreateAsync(city);
        await _cityService.CreateAsync(city2);

        var university = _universityFaker.Generate();
        university.CityId = city.Id;

        await _universityService.CreateAsync(university);

        var updateUniversity = _universityFaker.Generate();
        updateUniversity.CityId = city2.Id;

        var httpClient = _applicationFactory.CreateClient();

        var response = await httpClient.PutAsync("api/University/update", new StringContent(
            JsonSerializer.Serialize(
                new UpdateUniversityRequest
                {
                    Id = university.Id, Name = updateUniversity.Name, Abbreviation = updateUniversity.Abbreviation, CityId = updateUniversity.CityId,
                }, JsonDefaultOptions.SerializeOptions
            ), Encoding.UTF8, "application/json"
        ));

        var responseString = await response.Content.ReadAsStringAsync();
        var universityDto = JsonSerializer.Deserialize<UniversityDto>(responseString, JsonDefaultOptions.DeserializeOptions);

        updateUniversity.Id = universityDto.Id;

        Assert.Equivalent(HttpStatusCode.OK, response.StatusCode);
        Assert.Equivalent(updateUniversity.ToDto(), universityDto);
    }

    [Fact]
    public async Task UpdateUniversity_ReturnsBadRequest_WhenUniversityNotExists()
    {
        var university = _universityFaker.Generate();

        var httpClient = _applicationFactory.CreateClient();

        var response = await httpClient.PutAsync("api/University/update", new StringContent(
            JsonSerializer.Serialize(
                new UpdateUniversityRequest
                {
                    Id = "NotExistingId",
                    Name = university.Name,
                    Abbreviation = university.Abbreviation,
                    CityId = Guid.NewGuid().ToString(),
                }, JsonDefaultOptions.SerializeOptions
            ), Encoding.UTF8, "application/json"
        ));

        var responseString = await response.Content.ReadAsStringAsync();

        var exception = new EntityNotFoundException(typeof(University), "NotExistingId");

        Assert.Equivalent(HttpStatusCode.BadRequest, response.StatusCode);
        Assert.Equivalent(exception.Serialize(), responseString);
    }

    [Fact]
    public async Task UpdateUniversity_ReturnsBadRequest_WhenCityNotExists()
    {
        var city = _cityFaker.Generate();
        await _cityService.CreateAsync(city);

        var university = _universityFaker.Generate();
        university.CityId = city.Id;
        await _universityService.CreateAsync(university);

        var httpClient = _applicationFactory.CreateClient();

        var response = await httpClient.PutAsync("api/University/update", new StringContent(
            JsonSerializer.Serialize(
                new UpdateUniversityRequest
                {
                    Id = university.Id,
                    Name = university.Name,
                    Abbreviation = university.Abbreviation,
                    CityId = "NotExistingId",
                }, JsonDefaultOptions.SerializeOptions
            ), Encoding.UTF8, "application/json"
        ));

        var responseString = await response.Content.ReadAsStringAsync();

        var exception = new EntityNotFoundException(typeof(City), "NotExistingId");

        Assert.Equivalent(HttpStatusCode.BadRequest, response.StatusCode);
        Assert.Equivalent(exception.Serialize(), responseString);
    }

    [Fact]
    public async Task UpdateUniversity_ReturnsBadRequest_WhenUniversityWithSuchNameExists()
    {
        var city = _cityFaker.Generate();
        await _cityService.CreateAsync(city);

        var university = _universityFaker.Generate();
        university.CityId = city.Id;
        var university2 = _universityFaker.Generate();
        university2.CityId = city.Id;

        await _universityService.CreateAsync(university);
        await _universityService.CreateAsync(university2);

        var httpClient = _applicationFactory.CreateClient();

        var response = await httpClient.PutAsync("api/University/update", new StringContent(
            JsonSerializer.Serialize(
                new UpdateUniversityRequest
                {
                    Id = university.Id,
                    Name = university2.Name,
                    Abbreviation = university.Abbreviation,
                    CityId = university.CityId,
                }, JsonDefaultOptions.SerializeOptions
            ), Encoding.UTF8, "application/json"
        ));

        var responseString = await response.Content.ReadAsStringAsync();

        var exception = new EntityExistsException(typeof(University), nameof(University.Name), university2.Name);

        Assert.Equivalent(HttpStatusCode.BadRequest, response.StatusCode);
        Assert.Equivalent(exception.Serialize(), responseString);
    }

    [Fact]
    public async Task DeleteUniversity_ReturnsUniversity_WhenUniversityExists()
    {
        var city = _cityFaker.Generate();
        await _cityService.CreateAsync(city);

        var university = _universityFaker.Generate();
        university.CityId = city.Id;
        await _universityService.CreateAsync(university);

        var httpClient = _applicationFactory.CreateClient();

        var response = await httpClient.DeleteAsync($"api/University/delete?id={university.Id}");
        var responseString = await response.Content.ReadAsStringAsync();
        var universityDto = JsonSerializer.Deserialize<UniversityDto>(responseString, JsonDefaultOptions.DeserializeOptions);

        Assert.Equivalent(HttpStatusCode.OK, response.StatusCode);
        Assert.Equivalent(university.ToDto(), universityDto);
    }

    [Fact]
    public async Task DeleteUniversity_ReturnsBadRequest_WhenUniversityNotExists()
    {
        var httpClient = _applicationFactory.CreateClient();

        var response = await httpClient.DeleteAsync($"api/University/delete?id=NotExistingId");
        var responseString = await response.Content.ReadAsStringAsync();
        var exception = new EntityNotFoundException(typeof(University), "NotExistingId");

        Assert.Equivalent(HttpStatusCode.BadRequest, response.StatusCode);
        Assert.Equivalent(exception.Serialize(), responseString);
    }

    [Fact]
    public async Task GetUniversitysCity_ReturnsCity_WhenUniversityExist()
    {
        var city = _cityFaker.Generate();
        await _cityService.CreateAsync(city);

        var university = _universityFaker.Generate();
        university.CityId = city.Id;

        await _universityService.CreateAsync(university);

        var httpClient = _applicationFactory.CreateClient();

        var response = await httpClient.GetAsync($"api/University/getcity?id={university.Id}");
        var responseString = await response.Content.ReadAsStringAsync();
        var cityDto = JsonSerializer.Deserialize<CityDto>(responseString, JsonDefaultOptions.DeserializeOptions);

        Assert.Equivalent(HttpStatusCode.OK, response.StatusCode);
        Assert.Equivalent(city.ToDto(), cityDto);
    }

    [Fact]
    public async Task GetUniversitysCity_ReturnsBadRequest_WhenUniversityNotExist()
    {
        var httpClient = _applicationFactory.CreateClient();

        var response = await httpClient.GetAsync($"api/University/getcity?id=NotExistingId");
        var responseString = await response.Content.ReadAsStringAsync();
        var exception = new EntityNotFoundException(typeof(University), "NotExistingId");

        Assert.Equivalent(HttpStatusCode.BadRequest, response.StatusCode);
        Assert.Equivalent(exception.Serialize(), responseString);
    }

    [Fact]
    public async Task GetUniversitysTeachers_ReturnsListOfTeachers_WhenUniversityExist()
    {
        var city = _cityFaker.Generate();
        await _cityService.CreateAsync(city);

        var university = _universityFaker.Generate();
        university.CityId = city.Id;
        await _universityService.CreateAsync(university);

        var teachers = _teacherFaker.Generate(3);
        foreach (var teacher in teachers)
        {
            teacher.UniversityId = university.Id;
            await _teacherService.CreateAsync(teacher);
        }

        var httpClient = _applicationFactory.CreateClient();

        var response = await httpClient.GetAsync($"api/University/getteachers?id={university.Id}");
        var responseString = await response.Content.ReadAsStringAsync();
        var teacherDtos = JsonSerializer.Deserialize<List<TeacherDto>>(responseString, JsonDefaultOptions.DeserializeOptions);

        Assert.Equivalent(HttpStatusCode.OK, response.StatusCode);
        Assert.Equivalent(teachers.Select(t => t.ToDto()).ToList(), teacherDtos);
    }

    [Fact]
    public async Task GetUniversitysTeachers_ReturnsBadRequest_WhenUniversityNotExist()
    {
        var httpClient = _applicationFactory.CreateClient();

        var response = await httpClient.GetAsync($"api/University/getteachers?id=NotExistingId");
        var responseString = await response.Content.ReadAsStringAsync();
        var exception = new EntityNotFoundException(typeof(University), "NotExistingId");

        Assert.Equivalent(HttpStatusCode.BadRequest, response.StatusCode);
        Assert.Equivalent(exception.Serialize(), responseString);
    }
}