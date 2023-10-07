namespace TeacherReviews.Domain.DTO;

public class UniversityDto
{
    [Required]
    public string Id { get; set; } = default!;

    [Required]
    public string Name { get; set; } = default!;

    [Required]
    public string Abbreviation { get; set; } = default!;

    [Required]
    public string CityId { get; set; } = default!;
}