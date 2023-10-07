using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using TeacherReviews.API.Contracts.Repositories;
using TeacherReviews.Domain;
using TeacherReviews.Domain.Entities;

namespace TeacherReviews.API.Repositories;

public class ReviewRepository : IReviewRepository
{
    private readonly ApplicationDbContext _context;

    public ReviewRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    private DbSet<Review> ReviewsSet => _context.Reviews;

    public async Task<Review?> GetByIdAsync(string id)
    {
        return await ReviewsSet.FindAsync(id);
    }

    public async Task<IEnumerable<Review>> GetAllAsync(Expression<Func<Review, bool>>? filter = null)
    {
        if (filter is null)
        {
            return await ReviewsSet.ToListAsync();
        }

        return await ReviewsSet.Where(filter).ToListAsync();
    }

    public async Task<Review> AddAsync(Review item)
    {
        await ReviewsSet.AddAsync(item);
        return item;
    }

    public async Task<Review> DeleteAsync(string id)
    {
        var review = await GetByIdAsync(id);
        return ReviewsSet.Remove(review).Entity;
    }

    public Task<Review> UpdateAsync(Review item)
    {
        return Task.FromResult(ReviewsSet.Update(item).Entity);
    }

    public async Task<bool> ExistsAsync(string id)
    {
        return await ReviewsSet.AsNoTrackingWithIdentityResolution().AnyAsync(r => r.Id == id);
    }
}