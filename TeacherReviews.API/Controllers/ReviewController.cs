using Microsoft.AspNetCore.Mvc;
using TeacherReviews.API.Contracts.Requests.Review;
using TeacherReviews.API.Mapping;
using TeacherReviews.API.Services;

namespace TeacherReviews.API.Controllers;

[Route("api/[controller]")]
[ApiController]
public class ReviewController : ControllerBase
{
    private readonly ReviewService _reviewService;

    public ReviewController(ReviewService reviewService)
    {
        _reviewService = reviewService;
    }

    [HttpGet("getall")]
    public async Task<IActionResult> GetAll()
    {
        return Ok(
            (await _reviewService.GetAllAsync()).Select(r => r.ToDto())
        );
    }

    [HttpPost("create")]
    public async Task<IActionResult> Create([FromBody] CreateReviewRequest createTeacherRequest)
    {
        return Ok(
            (await _reviewService.AddAsync(createTeacherRequest.ToReview())).ToDto()
        );
    }

    [HttpDelete("delete")]
    public async Task<IActionResult> Delete([FromQuery] DeleteReviewRequest deleteReviewRequest)
    {
        return Ok(
            (await _reviewService.DeleteAsync(deleteReviewRequest.Id)).ToDto()
        );
    }
}