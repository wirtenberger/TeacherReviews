using System.ComponentModel.DataAnnotations;

namespace TeacherReviews.API.Contracts.Requests.City;

public class GetCityRequest
{
    [Required]
    public string Id { get; set; } = default!;
}