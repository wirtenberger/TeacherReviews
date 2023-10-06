using System.Linq.Expressions;
using TeacherReviews.Data.Entities;

namespace TeacherReviews.API.Contracts.Repositories;

public interface ICityRepository
{
    Task<City?> GetByIdAsync(string id);
    Task<IEnumerable<City>> GetAllAsync(Expression<Func<City, bool>>? filter = null);
    Task<City> AddAsync(City item);
    Task<City> DeleteAsync(string id);
    Task<City> UpdateAsync(City item);
    Task<bool> ExistsAsync(string id);
}