using System.ComponentModel.DataAnnotations;

namespace TeacherReviews.API.Contracts.Requests.Teacher;

public class GetTeacherRequest
{
    [Required]
    public string Id { get; set; } = default!;
}