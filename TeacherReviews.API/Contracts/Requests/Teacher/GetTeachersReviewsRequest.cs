using System.ComponentModel.DataAnnotations;

namespace TeacherReviews.API.Contracts.Requests.Teacher;

public class GetTeachersReviewsRequest
{
    [Required]
    public string Id { get; set; } = default!;
}