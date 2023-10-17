using System.Linq.Expressions;
using TeacherReviews.Domain.Entities;

namespace TeacherReviews.API.Contracts.Repositories;

public interface IReviewRepository
{
    Task<Review?> GetByIdAsync(string id);
    Task<IEnumerable<Review>> GetAllAsync(Expression<Func<Review, bool>>? filter = null);
    Task<Review> AddAsync(Review item);
    Task<Review> DeleteAsync(string id);
    Task<Review> UpdateAsync(Review item);
}