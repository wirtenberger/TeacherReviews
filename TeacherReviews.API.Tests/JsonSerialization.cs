using System.Text.Json;

namespace TeacherReviews.API.Tests;

internal static class JsonDeserializeOptions
{
    public static JsonSerializerOptions Default = new()
    {
        PropertyNameCaseInsensitive = true,
    };
}