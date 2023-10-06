using System.ComponentModel.DataAnnotations;

namespace TeacherReviews.API.Contracts.Requests.Review;

public class CreateReviewRequest
{
    [Required]
    [Range(1, 5)]
    public int Rate { get; set; } = default!;

    public string? Text { get; set; } = string.Empty;

    [Required]
    public string TeacherId { get; set; } = default!;
}