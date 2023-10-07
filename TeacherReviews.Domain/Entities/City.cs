namespace TeacherReviews.Domain.Entities;

public class City
{
    [Required]
    public string Id { get; set; }

    [Required]
    public string Name { get; set; } = default!;

    public virtual List<University> Universities { get; set; } = new();
}