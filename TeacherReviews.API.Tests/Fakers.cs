using Bogus;
using TeacherReviews.Domain.Entities;

namespace TeacherReviews.API.Tests;

public static class Fakers
{
    public static Faker Faker => new();

    public static Faker<City> CityFaker => new Faker<City>()
                                           .RuleFor(x => x.Id, _ => Guid.NewGuid().ToString())
                                           .RuleFor(x => x.Name, faker => faker.Address.City());

    public static Faker<University> UniversityFaker => new Faker<University>()
                                                       .RuleFor(u => u.Id, _ => Guid.NewGuid().ToString())
                                                       .RuleFor(u => u.Name, faker => faker.Address.City() + "University")
                                                       .RuleFor(u => u.Abbreviation, faker => faker.Random.String2(3, 4));

    public static Faker<Teacher> TeacherFaker => new Faker<Teacher>()
                                                 .RuleFor(t => t.Id, _ => Guid.NewGuid().ToString())
                                                 .RuleFor(t => t.Name, faker => faker.Name.FirstName())
                                                 .RuleFor(t => t.Surname, faker => faker.Name.LastName())
                                                 .RuleFor(t => t.Patronymic, faker => faker.Name.Random.String2(4, 8));
}