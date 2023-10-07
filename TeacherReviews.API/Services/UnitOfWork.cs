using TeacherReviews.Domain;

namespace TeacherReviews.API.Services;

public class UnitOfWork
{
    private readonly ApplicationDbContext _context;

    public UnitOfWork(ApplicationDbContext context)
    {
        _context = context;
    }

    public Task SaveChangesAsync()
    {
        return _context.SaveChangesAsync();
    }
}