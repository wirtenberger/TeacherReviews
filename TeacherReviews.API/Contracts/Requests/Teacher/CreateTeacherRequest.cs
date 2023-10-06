using System.ComponentModel.DataAnnotations;

namespace TeacherReviews.API.Contracts.Requests.Teacher;

public class CreateTeacherRequest
{
    [Required]
    public string Name { get; set; } = default!;

    [Required]
    public string Surname { get; set; } = default!;

    public string? Patronymic { get; set; } = string.Empty;

    [Required]
    public string UniversityId { get; set; } = default!;
}