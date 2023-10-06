using TeacherReviews.Data;

namespace TeacherReviews.API.Middlewares;

public class RequestTransactionMiddleware
{
    private readonly RequestDelegate _next;


    public RequestTransactionMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext context, ApplicationDbContext dbContext)
    {
        var transaction = await dbContext.Database.BeginTransactionAsync();
        try
        {
            await _next(context);
            await transaction.CommitAsync();
        }
        catch
        {
            await transaction.RollbackAsync();
            throw;
        }
    }
}