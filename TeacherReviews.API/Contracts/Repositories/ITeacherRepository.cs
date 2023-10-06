using System.Linq.Expressions;
using TeacherReviews.Data.Entities;

namespace TeacherReviews.API.Contracts.Repositories;

public interface ITeacherRepository
{
    Task<Teacher?> GetByIdAsync(string id);
    Task<IEnumerable<Teacher>> GetAllAsync(Expression<Func<Teacher, bool>>? filter = null);
    Task<Teacher> AddAsync(Teacher item);
    Task<Teacher> DeleteAsync(string id);
    Task<Teacher> UpdateAsync(Teacher item);
    Task<bool> ExistsAsync(string id);
}