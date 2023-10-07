namespace TeacherReviews.Domain.DTO;

public class ReviewDto
{
    [Required]
    public string Id { get; set; } = default!;

    [Required]
    [Range(1, 5)]
    public int Rate { get; set; } = 1;

    public string? Text { get; set; } = string.Empty;

    [Required]
    public DateOnly CreateDate { get; set; }

    [Required]
    public string TeacherId { get; set; } = default!;
}