using System.ComponentModel.DataAnnotations;

namespace TeacherReviews.API.Contracts.Requests.University;

public class DeleteUniversityRequest
{
    [Required]
    public string Id { get; set; } = default!;
}