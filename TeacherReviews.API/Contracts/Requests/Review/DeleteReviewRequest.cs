using System.ComponentModel.DataAnnotations;

namespace TeacherReviews.API.Contracts.Requests.Review;

public class DeleteReviewRequest
{
    [Required]
    public string Id { get; set; }
}