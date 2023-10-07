using TeacherReviews.API.Contracts.Requests.City;
using TeacherReviews.Domain.DTO;
using TeacherReviews.Domain.Entities;

namespace TeacherReviews.API.Mapping;

public static class CityMapper
{
    public static City ToCity(this CreateCityRequest createCityRequest)
    {
        return new City
        {
            Id = Guid.NewGuid().ToString(),
            Name = createCityRequest.Name,
        };
    }

    public static City ToCity(this UpdateCityRequest updateCityRequest)
    {
        return new City
        {
            Id = updateCityRequest.Id,
            Name = updateCityRequest.Name!,
        };
    }

    public static CityDto ToDto(this City city)
    {
        return new CityDto
        {
            Id = city.Id,
            Name = city.Name,
        };
    }
}