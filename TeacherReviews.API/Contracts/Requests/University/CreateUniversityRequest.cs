using System.ComponentModel.DataAnnotations;

namespace TeacherReviews.API.Contracts.Requests.University;

public class CreateUniversityRequest
{
    [Required]
    public string Name { get; set; } = default!;

    [Required]
    public string Abbreviation { get; set; } = default!;

    [Required]
    public string CityId { get; set; } = default!;
}