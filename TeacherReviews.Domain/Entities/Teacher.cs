namespace TeacherReviews.Data.Entities;

public class Teacher
{
    [Required]
    public string Id { get; set; }

    [Required]
    public string Name { get; set; } = default!;

    [Required]
    public string Surname { get; set; } = default!;

    public string? Patronymic { get; set; } = string.Empty;

    [Required]
    public string UniversityId { get; set; } = default!;

    [ForeignKey(nameof(UniversityId))]
    public virtual University University { get; set; } = default!;

    public virtual List<Review> Reviews { get; set; } = new();
}