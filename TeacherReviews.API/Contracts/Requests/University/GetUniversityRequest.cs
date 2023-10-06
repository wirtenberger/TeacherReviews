using System.ComponentModel.DataAnnotations;

namespace TeacherReviews.API.Contracts.Requests.University;

public class GetUniversityRequest
{
    [Required]
    public string Id { get; set; } = default!;
}