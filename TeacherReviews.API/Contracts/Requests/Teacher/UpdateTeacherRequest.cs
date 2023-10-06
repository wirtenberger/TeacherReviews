using System.ComponentModel.DataAnnotations;

namespace TeacherReviews.API.Contracts.Requests.Teacher;

public class UpdateTeacherRequest
{
    [Required]
    public string Id { get; set; } = default!;

    public string? Name { get; set; } = string.Empty;

    public string? Surname { get; set; } = string.Empty;

    public string? Patronymic { get; set; } = string.Empty;
    public string UniversityId { get; set; } = string.Empty;
}