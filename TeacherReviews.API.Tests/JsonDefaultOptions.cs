using System.Text.Json;

namespace TeacherReviews.API.Tests;

internal static class JsonDefaultOptions
{
    public static JsonSerializerOptions DeserializeOptions = new()
    {
        PropertyNameCaseInsensitive = true,
    };

    public static JsonSerializerOptions SerializeOptions = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
    };
}