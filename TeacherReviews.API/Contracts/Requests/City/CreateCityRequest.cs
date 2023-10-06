using System.ComponentModel.DataAnnotations;

namespace TeacherReviews.API.Contracts.Requests.City;

public class CreateCityRequest
{
    [Required]
    public string Name { get; set; } = default!;
}