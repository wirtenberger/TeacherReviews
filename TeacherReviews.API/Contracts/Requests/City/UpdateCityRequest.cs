using System.ComponentModel.DataAnnotations;

namespace TeacherReviews.API.Contracts.Requests.City;

public class UpdateCityRequest
{
    [Required]
    public string Id { get; set; } = default!;

    [Required]
    public string Name { get; set; } = string.Empty;
}