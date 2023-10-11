using System.Net;
using System.Text;
using System.Text.Json;
using Bogus;
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

    private Faker<City> CityFaker => Fakers.CityFaker;

    private readonly CityService _cityService;

    private Faker<Teacher> TeacherFaker => Fakers.TeacherFaker;

    private readonly TeacherService _teacherService;

    private Faker<University> UniversityFaker => Fakers.UniversityFaker;

    private readonly UniversityService _universityService;


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
        var university = UniversityFaker.Generate();
        await _cityService.CreateAsync(university.City);
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
        var id = "NotExistingId";
        var expectedException = new EntityNotFoundException(typeof(University), id).AsExceptionResponse();

        var httpClient = _applicationFactory.CreateClient();
        var response = await httpClient.GetAsync("api/University/get?id=NotExistingId");
        var responseString = await response.Content.ReadAsStringAsync();

        var exception = JsonSerializer.Deserialize<ExceptionResponse>(responseString, JsonDefaultOptions.DeserializeOptions);

        Assert.Equivalent(HttpStatusCode.BadRequest, response.StatusCode);
        Assert.Equivalent(expectedException, exception);
    }

    [Fact]
    public async Task CreateUniversity_ReturnsUniversity_WhenCityExistsAndUniversityWithSuchNameNotExists()
    {
        var university = UniversityFaker.Generate();
        await _cityService.CreateAsync(university.City);

        var httpClient = _applicationFactory.CreateClient();

        var response = await httpClient.PostAsync("api/University/create", new StringContent(
            JsonSerializer.Serialize(
                new CreateUniversityRequest { Name = university.Name, Abbreviation = university.Abbreviation, CityId = university.CityId },
                JsonDefaultOptions.SerializeOptions
            ), Encoding.UTF8, "application/json"
        ));

        var responseString = await response.Content.ReadAsStringAsync();
        var universityDto = JsonSerializer.Deserialize<UniversityDto>(responseString, JsonDefaultOptions.DeserializeOptions)!;

        university.Id = universityDto.Id;

        Assert.Equivalent(HttpStatusCode.OK, response.StatusCode);
        Assert.Equivalent(university.ToDto(), universityDto);
    }

    [Fact]
    public async Task CreateUniversity_ReturnsBadRequest_WhenCityNotExists()
    {
        var university = UniversityFaker.Generate();
        university.CityId = "NotExistingId";

        var expectedException = new EntityNotFoundException(typeof(City), university.CityId).AsExceptionResponse();

        var httpClient = _applicationFactory.CreateClient();

        var response = await httpClient.PostAsync("api/University/create", new StringContent(
            JsonSerializer.Serialize(
                new CreateUniversityRequest { Name = university.Name, Abbreviation = university.Abbreviation, CityId = university.CityId },
                JsonDefaultOptions.SerializeOptions
            ), Encoding.UTF8, "application/json"
        ));

        var responseString = await response.Content.ReadAsStringAsync();
        var exception = JsonSerializer.Deserialize<ExceptionResponse>(responseString, JsonDefaultOptions.DeserializeOptions);

        Assert.Equivalent(HttpStatusCode.BadRequest, response.StatusCode);
        Assert.Equivalent(expectedException, exception);
    }

    [Fact]
    public async Task CreateUniversity_ReturnsBadRequest_WhenUniversityWithSuchNameExists()
    {
        var university = UniversityFaker.Generate();
        await _cityService.CreateAsync(university.City);
        await _universityService.CreateAsync(university);

        var expectedException = new EntityExistsException(typeof(University), nameof(University.Name), university.Name).AsExceptionResponse();

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


        var exception = JsonSerializer.Deserialize<ExceptionResponse>(responseString, JsonDefaultOptions.DeserializeOptions);

        Assert.Equivalent(HttpStatusCode.BadRequest, response.StatusCode);
        Assert.Equivalent(expectedException, exception);
    }

    [Fact]
    public async Task UpdateUniversity_ReturnsUniversity_WhenCityExistsAndUniversityWithSuchNameNotExists()
    {
        var university = UniversityFaker.Generate();
        await _cityService.CreateAsync(university.City);
        await _universityService.CreateAsync(university);
        var updateUniversity = UniversityFaker.Generate();
        await _cityService.CreateAsync(updateUniversity.City);

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
        var universityDto = JsonSerializer.Deserialize<UniversityDto>(responseString, JsonDefaultOptions.DeserializeOptions)!;

        updateUniversity.Id = universityDto.Id;

        Assert.Equivalent(HttpStatusCode.OK, response.StatusCode);
        Assert.Equivalent(updateUniversity.ToDto(), universityDto);
    }

    [Fact]
    public async Task UpdateUniversity_ReturnsBadRequest_WhenUniversityNotExists()
    {
        var university = UniversityFaker.Generate();

        var notExistingId = "NotExistingId";
        var expectedException = new EntityNotFoundException(typeof(University), notExistingId).AsExceptionResponse();

        var httpClient = _applicationFactory.CreateClient();

        var response = await httpClient.PutAsync("api/University/update", new StringContent(
            JsonSerializer.Serialize(
                new UpdateUniversityRequest
                {
                    Id = notExistingId,
                    Name = university.Name,
                    Abbreviation = university.Abbreviation,
                    CityId = university.CityId,
                }, JsonDefaultOptions.SerializeOptions
            ), Encoding.UTF8, "application/json"
        ));

        var responseString = await response.Content.ReadAsStringAsync();
        var exception = JsonSerializer.Deserialize<ExceptionResponse>(responseString, JsonDefaultOptions.DeserializeOptions);

        Assert.Equivalent(HttpStatusCode.BadRequest, response.StatusCode);
        Assert.Equivalent(expectedException, exception);
    }

    [Fact]
    public async Task UpdateUniversity_ReturnsBadRequest_WhenCityNotExists()
    {
        var university = UniversityFaker.Generate();
        await _cityService.CreateAsync(university.City);
        await _universityService.CreateAsync(university);

        var notExistingId = "NotExistingId";
        var expectedException = new EntityNotFoundException(typeof(City), notExistingId).AsExceptionResponse();

        var httpClient = _applicationFactory.CreateClient();

        var response = await httpClient.PutAsync("api/University/update", new StringContent(
            JsonSerializer.Serialize(
                new UpdateUniversityRequest
                {
                    Id = university.Id,
                    Name = university.Name,
                    Abbreviation = university.Abbreviation,
                    CityId = notExistingId,
                }, JsonDefaultOptions.SerializeOptions
            ), Encoding.UTF8, "application/json"
        ));

        var responseString = await response.Content.ReadAsStringAsync();
        var exception = JsonSerializer.Deserialize<ExceptionResponse>(responseString, JsonDefaultOptions.DeserializeOptions);

        Assert.Equivalent(HttpStatusCode.BadRequest, response.StatusCode);
        Assert.Equivalent(expectedException, exception);
    }

    [Fact]
    public async Task UpdateUniversity_ReturnsBadRequest_WhenUniversityWithSuchNameExists()
    {
        var university = UniversityFaker.Generate();
        var university2 = UniversityFaker.Generate();

        (university2.City, university2.CityId) = (university.City, university.CityId);

        await _cityService.CreateAsync(university.City);

        await _universityService.CreateAsync(university);
        await _universityService.CreateAsync(university2);

        var expectedException = new EntityExistsException(typeof(University), nameof(University.Name), university2.Name).AsExceptionResponse();

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
        var exception = JsonSerializer.Deserialize<ExceptionResponse>(responseString, JsonDefaultOptions.DeserializeOptions);

        Assert.Equivalent(HttpStatusCode.BadRequest, response.StatusCode);
        Assert.Equivalent(expectedException, exception);
    }

    [Fact]
    public async Task DeleteUniversity_ReturnsUniversity_WhenUniversityExists()
    {
        var university = UniversityFaker.Generate();
        await _cityService.CreateAsync(university.City);
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
        var id = "NotExistingId";
        var expectedException = new EntityNotFoundException(typeof(University), id).AsExceptionResponse();

        var httpClient = _applicationFactory.CreateClient();
        var response = await httpClient.DeleteAsync($"api/University/delete?id={id}");
        var responseString = await response.Content.ReadAsStringAsync();
        var exception = JsonSerializer.Deserialize<ExceptionResponse>(responseString, JsonDefaultOptions.DeserializeOptions);

        Assert.Equivalent(HttpStatusCode.BadRequest, response.StatusCode);
        Assert.Equivalent(expectedException, exception);
    }

    [Fact]
    public async Task GetUniversitysCity_ReturnsCity_WhenUniversityExist()
    {
        var university = UniversityFaker.Generate();
        await _cityService.CreateAsync(university.City);
        await _universityService.CreateAsync(university);

        var httpClient = _applicationFactory.CreateClient();

        var response = await httpClient.GetAsync($"api/University/getcity?id={university.Id}");
        var responseString = await response.Content.ReadAsStringAsync();
        var cityDto = JsonSerializer.Deserialize<CityDto>(responseString, JsonDefaultOptions.DeserializeOptions);

        Assert.Equivalent(HttpStatusCode.OK, response.StatusCode);
        Assert.Equivalent(university.City.ToDto(), cityDto);
    }

    [Fact]
    public async Task GetUniversitysCity_ReturnsBadRequest_WhenUniversityNotExist()
    {
        var id = "NotExistingId";
        var expectedException = new EntityNotFoundException(typeof(University), id).AsExceptionResponse();

        var httpClient = _applicationFactory.CreateClient();

        var response = await httpClient.GetAsync($"api/University/getcity?id={id}");
        var responseString = await response.Content.ReadAsStringAsync();
        var exception = JsonSerializer.Deserialize<ExceptionResponse>(responseString, JsonDefaultOptions.DeserializeOptions);

        Assert.Equivalent(HttpStatusCode.BadRequest, response.StatusCode);
        Assert.Equivalent(expectedException, exception);
    }

    [Fact]
    public async Task GetUniversitysTeachers_ReturnsListOfTeachers_WhenUniversityExist()
    {
        var university = UniversityFaker.Generate();
        await _cityService.CreateAsync(university.City);
        await _universityService.CreateAsync(university);

        var teachers = TeacherFaker.Generate(3);
        foreach (var teacher in teachers)
        {
            (teacher.University, teacher.UniversityId) = (university, university.Id);
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
        var id = "NotExistingId";
        var expectedException = new EntityNotFoundException(typeof(University), id).AsExceptionResponse();

        var httpClient = _applicationFactory.CreateClient();

        var response = await httpClient.GetAsync($"api/University/getteachers?id={id}");
        var responseString = await response.Content.ReadAsStringAsync();
        var exception = JsonSerializer.Deserialize<ExceptionResponse>(responseString, JsonDefaultOptions.DeserializeOptions);

        Assert.Equivalent(HttpStatusCode.BadRequest, response.StatusCode);
        Assert.Equivalent(expectedException, exception);
    }
}