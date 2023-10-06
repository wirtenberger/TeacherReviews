namespace TeacherReviews.Data.DTO;

public class CityDto
{
    [Required]
    public string Id { get; set; } = default!;

    [Required]
    public string Name { get; set; } = default!;
}