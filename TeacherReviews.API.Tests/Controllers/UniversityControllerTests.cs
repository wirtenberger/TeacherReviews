using System.Net;
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

    private readonly CityService _cityService;

    private readonly TeacherService _teacherService;

    private readonly UniversityService _universityService;

    private Faker<Teacher> TeacherFaker => Fakers.TeacherFaker;

    private Faker<University> UniversityFaker => Fakers.UniversityFaker;

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
        var universityDto = await response.Content.ReadFromJsonAsync<UniversityDto>();

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
        var exception = await response.Content.ReadFromJsonAsync<ExceptionResponse>();

        Assert.Equivalent(HttpStatusCode.BadRequest, response.StatusCode);
        Assert.Equivalent(expectedException, exception);
    }

    [Fact]
    public async Task CreateUniversity_ReturnsUniversity_WhenCityExistsAndUniversityWithSuchNameNotExists()
    {
        var university = UniversityFaker.Generate();
        await _cityService.CreateAsync(university.City);

        var httpClient = _applicationFactory.CreateClient();

        var response = await httpClient.PostAsJsonAsync("api/University/create",
            new CreateUniversityRequest
            {
                Name = university.Name,
                Abbreviation = university.Abbreviation,
                CityId = university.CityId,
            }
        );
        var universityDto = await response.Content.ReadFromJsonAsync<UniversityDto>();
        university.Id = universityDto!.Id;

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

        var response = await httpClient.PostAsJsonAsync("api/University/create",
            new CreateUniversityRequest
            {
                Name = university.Name,
                Abbreviation = university.Abbreviation,
                CityId = university.CityId,
            }
        );
        var exception = await response.Content.ReadFromJsonAsync<ExceptionResponse>();

        Assert.Equivalent(HttpStatusCode.BadRequest, response.StatusCode);
        Assert.Equivalent(expectedException, exception);
    }

    [Fact]
    public async Task CreateUniversity_ReturnsBadRequest_WhenUniversityWithSuchNameExists()
    {
        var university = UniversityFaker.Generate();
        await _cityService.CreateAsync(university.City);
        await _universityService.CreateAsync(university);

        var expectedException = new EntityExistsException(typeof(University), nameof(University.Name), university.Name)
            .AsExceptionResponse();

        var httpClient = _applicationFactory.CreateClient();
        var response = await httpClient.PostAsJsonAsync("api/University/create",
            new CreateUniversityRequest
            {
                Name = university.Name,
                Abbreviation = university.Abbreviation,
                CityId = university.CityId,
            }
        );

        var exception = await response.Content.ReadFromJsonAsync<ExceptionResponse>();

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
        var response = await httpClient.PutAsJsonAsync("api/University/update",
            new UpdateUniversityRequest
            {
                Id = university.Id,
                Name = updateUniversity.Name,
                Abbreviation = updateUniversity.Abbreviation,
                CityId = updateUniversity.CityId,
            }
        );
        var universityDto = await response.Content.ReadFromJsonAsync<UniversityDto>();
        updateUniversity.Id = universityDto!.Id;

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

        var response = await httpClient.PutAsJsonAsync("api/University/update",
            new UpdateUniversityRequest
            {
                Id = notExistingId,
                Name = university.Name,
                Abbreviation = university.Abbreviation,
                CityId = university.CityId,
            }
        );
        var exception = await response.Content.ReadFromJsonAsync<ExceptionResponse>();

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
        var response = await httpClient.PutAsJsonAsync("api/University/update",
            new UpdateUniversityRequest
            {
                Id = university.Id,
                Name = university.Name,
                Abbreviation = university.Abbreviation,
                CityId = notExistingId,
            });
        var exception = await response.Content.ReadFromJsonAsync<ExceptionResponse>();

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

        var expectedException = new EntityExistsException(typeof(University), nameof(University.Name), university2.Name)
            .AsExceptionResponse();

        var httpClient = _applicationFactory.CreateClient();
        var response = await httpClient.PutAsJsonAsync("api/University/update",
            new UpdateUniversityRequest
            {
                Id = university.Id,
                Name = university2.Name,
                Abbreviation = university.Abbreviation,
                CityId = university.CityId,
            });

        var exception = await response.Content.ReadFromJsonAsync<ExceptionResponse>();

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
        var universityDto = await response.Content.ReadFromJsonAsync<UniversityDto>();

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
        var exception = await response.Content.ReadFromJsonAsync<ExceptionResponse>();

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
        var cityDto = await response.Content.ReadFromJsonAsync<CityDto>();

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
        var exception = await response.Content.ReadFromJsonAsync<ExceptionResponse>();

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
        var teacherDtos = await response.Content.ReadFromJsonAsync<List<TeacherDto>>();

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
        var exception = await response.Content.ReadFromJsonAsync<ExceptionResponse>();

        Assert.Equivalent(HttpStatusCode.BadRequest, response.StatusCode);
        Assert.Equivalent(expectedException, exception);
    }
}