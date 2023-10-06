using System.ComponentModel.DataAnnotations;

namespace TeacherReviews.API.Contracts.Requests.City;

public class GetCityUniversitiesRequest
{
    [Required]
    public string Id { get; set; } = default!;
}