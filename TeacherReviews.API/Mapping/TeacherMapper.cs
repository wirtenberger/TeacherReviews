using TeacherReviews.API.Contracts.Requests.Teacher;
using TeacherReviews.Data.DTO;
using TeacherReviews.Data.Entities;

namespace TeacherReviews.API.Mapping;

public static class TeacherMapper
{
    public static TeacherDto ToDto(this Teacher teacher)
    {
        return new TeacherDto
        {
            Id = teacher.Id,
            Name = teacher.Name,
            Surname = teacher.Surname,
            Patronymic = teacher.Patronymic,
            UniversityId = teacher.UniversityId
        };
    }

    public static Teacher ToTeacher(this CreateTeacherRequest createTeacherRequest)
    {
        return new Teacher
        {
            Id = Guid.NewGuid().ToString(),
            Name = createTeacherRequest.Name,
            Surname = createTeacherRequest.Surname,
            Patronymic = createTeacherRequest.Patronymic,
            UniversityId = createTeacherRequest.UniversityId
        };
    }

    public static Teacher ToTeacher(this UpdateTeacherRequest updateTeacherRequest)
    {
        return new Teacher
        {
            Id = updateTeacherRequest.Id,
            Name = updateTeacherRequest.Name!,
            Surname = updateTeacherRequest.Surname!,
            Patronymic = updateTeacherRequest.Patronymic,
            UniversityId = updateTeacherRequest.UniversityId
        };
    }
}