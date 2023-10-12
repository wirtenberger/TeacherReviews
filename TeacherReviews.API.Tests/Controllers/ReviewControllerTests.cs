using System.Net;
using Bogus;
using TeacherReviews.API.Contracts.Requests.Review;
using TeacherReviews.API.Mapping;
using TeacherReviews.API.Services;
using TeacherReviews.Domain.DTO;
using TeacherReviews.Domain.Entities;
using TeacherReviews.Domain.Exceptions;
using Xunit;

namespace TeacherReviews.API.Tests.Controllers;

public class ReviewControllerTests
{
    private readonly ApplicationFactory _applicationFactory;
    
    private readonly CityService _cityService;
    
    private readonly UniversityService _universityService;
    
    private readonly TeacherService _teacherService;
    
    private readonly ReviewService _reviewService;

    private Faker<Review> ReviewFaker => Fakers.ReviewFaker;

    public ReviewControllerTests()
    {
        _applicationFactory = new ApplicationFactory();
        var scope = _applicationFactory.Services.CreateScope();
        _cityService = scope.ServiceProvider.GetRequiredService<CityService>();
        _universityService = scope.ServiceProvider.GetRequiredService<UniversityService>();
        _teacherService = scope.ServiceProvider.GetRequiredService<TeacherService>();
        _reviewService = scope.ServiceProvider.GetRequiredService<ReviewService>();
    }

    [Fact]
    public async Task Create_ReturnsReview_WhenTeacherExists()
    {
        var review = ReviewFaker.Generate();
        await _cityService.CreateAsync(review.Teacher.University.City);
        await _universityService.CreateAsync(review.Teacher.University);
        await _teacherService.CreateAsync(review.Teacher);

        var httpClient = _applicationFactory.CreateClient();
        var response = await httpClient.PostAsJsonAsync($"api/Review/create", new CreateReviewRequest
        {
            TeacherId = review.TeacherId,
            Rate = review.Rate,
            Text = review.Text
        });
        var reviewDto = await response.Content.ReadFromJsonAsync<ReviewDto>();

        review.Id = reviewDto.Id;
        
        Assert.Equivalent(HttpStatusCode.OK, response.StatusCode);
        Assert.Equivalent(review.ToDto(), reviewDto);
    }
    
    [Fact]
    public async Task Create_ReturnsReview_WhenTeacherNotExists()
    {
        var review = ReviewFaker.Generate();
        var expectedException = new EntityNotFoundException(typeof(Teacher), review.TeacherId).AsExceptionResponse();

        var httpClient = _applicationFactory.CreateClient();
        var response = await httpClient.PostAsJsonAsync($"api/Review/create", new CreateReviewRequest
        {
            TeacherId = review.TeacherId,
            Rate = review.Rate,
            Text = review.Text
        });
        var exception = await response.Content.ReadFromJsonAsync<ExceptionResponse>();
        
        Assert.Equivalent(HttpStatusCode.BadRequest, response.StatusCode);
        Assert.Equivalent(expectedException, exception);
    }

    [Fact]
    public async Task Delete_ReturnsReview_WhenExist()
    {
        var review = ReviewFaker.Generate();
        await _cityService.CreateAsync(review.Teacher.University.City);
        await _universityService.CreateAsync(review.Teacher.University);
        await _teacherService.CreateAsync(review.Teacher);
        await _reviewService.CreateAsync(review);

        var httpClient = _applicationFactory.CreateClient();
        var response = await httpClient.DeleteAsync($"api/Review/delete?id={review.Id}");
        var reviewDto = await response.Content.ReadFromJsonAsync<ReviewDto>();
        
        Assert.Equivalent(HttpStatusCode.OK, response.StatusCode);
        Assert.Equivalent(review.ToDto(), reviewDto);
    }
    
    [Fact]
    public async Task Delete_ReturnsReview_WhenNotExist()
    {
        var id = "NotExistingId";
        var expectedException = new EntityNotFoundException(typeof(Review),id).AsExceptionResponse();

        var httpClient = _applicationFactory.CreateClient();
        var response = await httpClient.DeleteAsync($"api/Review/delete?id={id}");
        var exception = await response.Content.ReadFromJsonAsync<ExceptionResponse>();
        
        Assert.Equivalent(HttpStatusCode.BadRequest, response.StatusCode);
        Assert.Equivalent(expectedException, exception);
    }
}