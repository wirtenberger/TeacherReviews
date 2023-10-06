using Microsoft.AspNetCore.Http;

namespace TeacherReviews.Data.Exceptions;

public static class ExceptionHandler
{
    public static async Task Handle(HttpContext context, Exception exception)
    {
        var baseApiException = exception as BaseApiException ?? new BaseApiException(exception);

        context.Response.ContentType = "application/json";
        context.Response.StatusCode = baseApiException.StatusCode;

        await context.Response.WriteAsync(baseApiException.Serialize());
    }
}