namespace TeacherReviews.Data.DTO;

public class TeacherDto
{
    [Required]
    public string Id { get; set; } = default!;

    [Required]
    public string Name { get; set; } = default!;

    [Required]
    public string Surname { get; set; } = default!;

    public string? Patronymic { get; set; } = string.Empty;

    [Required]
    public string UniversityId { get; set; } = default!;
}