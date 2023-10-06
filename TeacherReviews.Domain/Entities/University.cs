namespace TeacherReviews.Data.Entities;

public class University
{
    [Required]
    public string Id { get; set; }

    [Required]
    public string Name { get; set; } = default!;

    [Required]
    public string Abbreviation { get; set; } = default!;

    [Required]
    public string CityId { get; set; } = default!;

    [ForeignKey(nameof(CityId))]
    public virtual City City { get; set; } = default!;

    public virtual List<Teacher> Teachers { get; set; } = new();
}