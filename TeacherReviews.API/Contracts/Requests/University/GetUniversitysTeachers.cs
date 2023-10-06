using System.ComponentModel.DataAnnotations;

namespace TeacherReviews.API.Contracts.Requests.University;

public class GetUniversitysTeachers
{
    [Required]
    public string Id { get; set; } = default!;
}