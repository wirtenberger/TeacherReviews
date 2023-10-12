using System.Net;
using Bogus;
using TeacherReviews.API.Contracts.Requests.Teacher;
using TeacherReviews.API.Mapping;
using TeacherReviews.API.Services;
using TeacherReviews.Domain.DTO;
using TeacherReviews.Domain.Entities;
using TeacherReviews.Domain.Exceptions;
using Xunit;

namespace TeacherReviews.API.Tests.Controllers;

public class TeacherControllerTests
{
    private readonly ApplicationFactory _applicationFactory;

    private readonly CityService _cityService;

    private readonly ReviewService _reviewService;

    private readonly TeacherService _teacherService;

    private readonly UniversityService _universityService;

    private Faker<Teacher> TeacherFaker => Fakers.TeacherFaker;

    private Faker<Review> ReviewFaker => Fakers.ReviewFaker;

    public TeacherControllerTests()
    {
        _applicationFactory = new ApplicationFactory();
        var scope = _applicationFactory.Services.CreateScope();
        _cityService = scope.ServiceProvider.GetRequiredService<CityService>();
        _universityService = scope.ServiceProvider.GetRequiredService<UniversityService>();
        _teacherService = scope.ServiceProvider.GetRequiredService<TeacherService>();
        _reviewService = scope.ServiceProvider.GetRequiredService<ReviewService>();
    }

    [Fact]
    public async Task GetById_ReturnsTeacher_WhenExists()
    {
        var teacher = TeacherFaker.Generate();
        await _cityService.CreateAsync(teacher.University.City);
        await _universityService.CreateAsync(teacher.University);
        await _teacherService.CreateAsync(teacher);

        var httpClient = _applicationFactory.CreateClient();
        var response = await httpClient.GetAsync($"api/Teacher/get?id={teacher.Id}");
        var teacherDto = await response.Content.ReadFromJsonAsync<TeacherDto>();

        Assert.Equivalent(HttpStatusCode.OK, response.StatusCode);
        Assert.Equivalent(teacher.ToDto(), teacherDto);
    }

    [Fact]
    public async Task GetById_ReturnsBadRequest_WhenNotExist()
    {
        var id = "NotExistingId";
        var expectedException = new EntityNotFoundException(typeof(Teacher), id).AsExceptionResponse();

        var httpClient = _applicationFactory.CreateClient();
        var response = await httpClient.GetAsync($"api/Teacher/get?id={id}");
        var exception = await response.Content.ReadFromJsonAsync<ExceptionResponse>();

        Assert.Equivalent(HttpStatusCode.BadRequest, response.StatusCode);
        Assert.Equivalent(expectedException, exception);
    }

    [Fact]
    public async Task Create_ReturnsTeacher_WhenUniversityExists()
    {
        var teacher = TeacherFaker.Generate();
        await _cityService.CreateAsync(teacher.University.City);
        await _universityService.CreateAsync(teacher.University);

        var httpClient = _applicationFactory.CreateClient();
        var response = await httpClient.PostAsJsonAsync("api/Teacher/create",
            new CreateTeacherRequest
            {
                Name = teacher.Name,
                Surname = teacher.Surname,
                Patronymic = teacher.Patronymic,
                UniversityId = teacher.UniversityId
            });
        var teacherDto = await response.Content.ReadFromJsonAsync<TeacherDto>();
        teacher.Id = teacherDto!.Id;

        Assert.Equivalent(HttpStatusCode.OK, response.StatusCode);
        Assert.Equivalent(teacher.ToDto(), teacherDto);
    }

    [Fact]
    public async Task Create_ReturnsBadRequest_WhenUniversityNotExists()
    {
        var teacher = TeacherFaker.Generate();
        var id = "NotExistingId";
        var expectedException = new EntityNotFoundException(typeof(University), id).AsExceptionResponse();

        var httpClient = _applicationFactory.CreateClient();
        var response = await httpClient.PostAsJsonAsync("api/Teacher/create",
            new CreateTeacherRequest
            {
                Name = teacher.Name,
                Surname = teacher.Surname,
                Patronymic = teacher.Patronymic,
                UniversityId = id
            });
        var exception = await response.Content.ReadFromJsonAsync<ExceptionResponse>();

        Assert.Equivalent(HttpStatusCode.BadRequest, response.StatusCode);
        Assert.Equivalent(expectedException, exception);
    }

    [Fact]
    public async Task Update_ReturnsTeacher_WhenTeacherAndUniversityExists()
    {
        var teacher = TeacherFaker.Generate();

        await _cityService.CreateAsync(teacher.University.City);
        await _universityService.CreateAsync(teacher.University);
        await _teacherService.CreateAsync(teacher);

        var updateTeacher = TeacherFaker.Generate();
        await _cityService.CreateAsync(updateTeacher.University.City);
        await _universityService.CreateAsync(updateTeacher.University);

        updateTeacher.Id = teacher.Id;

        var httpClient = _applicationFactory.CreateClient();
        var response = await httpClient.PutAsJsonAsync("api/Teacher/update",
            new UpdateTeacherRequest
            {
                Id = updateTeacher.Id,
                Name = updateTeacher.Name,
                Surname = updateTeacher.Surname,
                Patronymic = updateTeacher.Patronymic,
                UniversityId = updateTeacher.UniversityId
            });
        var teacherDto = await response.Content.ReadFromJsonAsync<TeacherDto>();

        Assert.Equivalent(HttpStatusCode.OK, response.StatusCode);
        Assert.Equivalent(updateTeacher.ToDto(), teacherDto);
    }

    [Fact]
    public async Task Update_ReturnsBadRequest_WhenNotExists()
    {
        var id = "NotExistingId";
        var teacher = TeacherFaker.Generate();

        var expectedException = new EntityNotFoundException(typeof(Teacher), id).AsExceptionResponse();

        var httpClient = _applicationFactory.CreateClient();
        var response = await httpClient.PutAsJsonAsync("api/Teacher/update",
            new UpdateTeacherRequest
            {
                Id = id,
                Name = teacher.Name,
                Surname = teacher.Surname,
                Patronymic = teacher.Patronymic,
                UniversityId = teacher.UniversityId
            });
        var exception = await response.Content.ReadFromJsonAsync<ExceptionResponse>();

        Assert.Equivalent(HttpStatusCode.BadRequest, response.StatusCode);
        Assert.Equivalent(expectedException, exception);
    }

    [Fact]
    public async Task Update_ReturnsBadRequest_WhenUniversityNotExists()
    {
        var id = "NotExistingId";
        var teacher = TeacherFaker.Generate();
        await _cityService.CreateAsync(teacher.University.City);
        await _universityService.CreateAsync(teacher.University);
        await _teacherService.CreateAsync(teacher);

        var expectedException = new EntityNotFoundException(typeof(University), id).AsExceptionResponse();

        var httpClient = _applicationFactory.CreateClient();
        var response = await httpClient.PutAsJsonAsync("api/Teacher/update",
            new UpdateTeacherRequest
            {
                Id = teacher.Id,
                Name = teacher.Name,
                Surname = teacher.Surname,
                Patronymic = teacher.Patronymic,
                UniversityId = id
            });

        var exception = await response.Content.ReadFromJsonAsync<ExceptionResponse>();

        Assert.Equivalent(HttpStatusCode.BadRequest, response.StatusCode);
        Assert.Equivalent(expectedException, exception);
    }

    [Fact]
    public async Task Delete_ReturnsTeacher_WhenExists()
    {
        var teacher = TeacherFaker.Generate();
        await _cityService.CreateAsync(teacher.University.City);
        await _universityService.CreateAsync(teacher.University);
        await _teacherService.CreateAsync(teacher);

        var httpClient = _applicationFactory.CreateClient();
        var response = await httpClient.DeleteAsync($"api/Teacher/delete?id={teacher.Id}");
        var teacherDto = await response.Content.ReadFromJsonAsync<TeacherDto>();

        Assert.Equivalent(HttpStatusCode.OK, response.StatusCode);
        Assert.Equivalent(teacher.ToDto(), teacherDto);
    }

    [Fact]
    public async Task Delete_ReturnsBadRequest_WhenNotExists()
    {
        var id = "NotExistingId";
        var expectedException = new EntityNotFoundException(typeof(Teacher), id).AsExceptionResponse();

        var httpClient = _applicationFactory.CreateClient();
        var response = await httpClient.DeleteAsync($"api/Teacher/delete?id={id}");
        var exception = await response.Content.ReadFromJsonAsync<ExceptionResponse>();

        Assert.Equivalent(HttpStatusCode.BadRequest, response.StatusCode);
        Assert.Equivalent(expectedException, exception);
    }

    [Fact]
    public async Task GetReviews_ReturnsListOfReviews_WhenExists()
    {
        var teacher = TeacherFaker.Generate();
        await _cityService.CreateAsync(teacher.University.City);
        await _universityService.CreateAsync(teacher.University);
        await _teacherService.CreateAsync(teacher);

        var reviews = ReviewFaker.Generate(3);

        foreach (var review in reviews)
        {
            (review.Teacher, review.TeacherId) = (teacher, teacher.Id);
            await _reviewService.CreateAsync(review);
        }

        var httpClient = _applicationFactory.CreateClient();
        var response = await httpClient.GetAsync($"api/Teacher/getreviews?id={teacher.Id}");
        var reviewDtos = await response.Content.ReadFromJsonAsync<List<ReviewDto>>();

        Assert.Equivalent(HttpStatusCode.OK, response.StatusCode);
        Assert.Equivalent(reviews.Select(r => r.ToDto()), reviewDtos);
    }

    [Fact]
    public async Task GetReviews_ReturnsListOfReviews_WhenNotExists()
    {
        var id = "NotExistingId";
        var expectedException = new EntityNotFoundException(typeof(Teacher), id).AsExceptionResponse();

        var httpClient = _applicationFactory.CreateClient();
        var response = await httpClient.GetAsync($"api/Teacher/getreviews?id={id}");
        var exception = await response.Content.ReadFromJsonAsync<ExceptionResponse>();

        Assert.Equivalent(HttpStatusCode.BadRequest, response.StatusCode);
        Assert.Equivalent(expectedException, exception);
    }
}