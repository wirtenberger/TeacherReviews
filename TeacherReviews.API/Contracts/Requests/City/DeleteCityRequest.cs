using System.ComponentModel.DataAnnotations;

namespace TeacherReviews.API.Contracts.Requests.City;

public class DeleteCityRequest
{
    [Required]
    public string Id { get; set; } = default!;
}