using System.Linq.Expressions;
using TeacherReviews.Domain.Entities;

namespace TeacherReviews.API.Contracts.Repositories;

public interface IUniversityRepository
{
    Task<University?> GetByIdAsync(string id);
    Task<IEnumerable<University>> GetAllAsync(Expression<Func<University, bool>>? filter = null);
    Task<University> AddAsync(University item);
    Task<University> DeleteAsync(string id);
    Task<University> UpdateAsync(University item);
    Task<bool> ExistsAsync(string id);
}