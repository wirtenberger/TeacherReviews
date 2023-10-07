using TeacherReviews.API.Contracts.Requests.University;
using TeacherReviews.Domain.DTO;
using TeacherReviews.Domain.Entities;

namespace TeacherReviews.API.Mapping;

public static class UniversityMapper
{
    public static University ToUniversity(this CreateUniversityRequest createUniversityRequest)
    {
        return new University
        {
            Id = Guid.NewGuid().ToString(),
            Name = createUniversityRequest.Name,
            Abbreviation = createUniversityRequest.Abbreviation,
            CityId = createUniversityRequest.CityId,
        };
    }

    public static University ToUniversity(this UpdateUniversityRequest updateUniversityRequest)
    {
        return new University
        {
            Id = updateUniversityRequest.Id,
            Name = updateUniversityRequest.Name!,
            Abbreviation = updateUniversityRequest.Abbreviation!,
            CityId = updateUniversityRequest.CityId!,
        };
    }

    public static UniversityDto ToDto(this University university)
    {
        return new UniversityDto
        {
            Id = university.Id,
            Name = university.Name,
            Abbreviation = university.Abbreviation,
            CityId = university.CityId,
        };
    }
}