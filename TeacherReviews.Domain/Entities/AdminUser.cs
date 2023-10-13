namespace TeacherReviews.Domain.Entities;

public class AdminUser
{
    [Required]
    [Key]
    public int Id { get; set; }

    [Required]
    public string Username { get; set; } = default!;

    [Required]
    public string Password { get; set; } = default!;
}