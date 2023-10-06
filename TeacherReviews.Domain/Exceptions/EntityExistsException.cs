using Microsoft.AspNetCore.Http;

namespace TeacherReviews.Data.Exceptions;

public class EntityExistsException : BaseApiException
{
    public new const int DefaultCode = StatusCodes.Status400BadRequest;

    public EntityExistsException(Type entityType, string field, string value) : base(DefaultCode,
        $"{entityType.Name} with {field} = {value} already exists")
    {
    }
}