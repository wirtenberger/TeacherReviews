using System.Linq.Expressions;
using TeacherReviews.Domain.Entities;

namespace TeacherReviews.API.Contracts.Repositories;

public interface IAdminUserRepository
{
    Task<List<AdminUser>> GetAllAsync(Expression<Func<AdminUser, bool>>? filter);
    Task<AdminUser?> GetByIdAsync(int id);
    Task<AdminUser?> GetByUsernameAsync(string username);
    Task<AdminUser> CreateAsync(AdminUser user);
    Task<AdminUser> UpdateAsync(AdminUser adminUser);
    Task<AdminUser> DeleteAsync(int id);
}