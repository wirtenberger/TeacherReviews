using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using TeacherReviews.API.Contracts.Repositories;
using TeacherReviews.Domain;
using TeacherReviews.Domain.Entities;

namespace TeacherReviews.API.Repositories;

public class AdminUserRepository : IAdminUserRepository
{
    private readonly UsersDbContext _usersDbContext;

    private DbSet<AdminUser> UsersSet => _usersDbContext.AdminUsers;


    public AdminUserRepository(UsersDbContext usersDbContext)
    {
        _usersDbContext = usersDbContext;
    }

    public Task<List<AdminUser>> GetAllAsync(Expression<Func<AdminUser, bool>>? filter)
    {
        if (filter == null)
        {
            return UsersSet.ToListAsync();
        }

        return UsersSet.Where(filter).ToListAsync();
    }

    public async Task<AdminUser?> GetByIdAsync(int id)
    {
        return await UsersSet.FindAsync(id);
    }

    public Task<AdminUser?> GetByUsernameAsync(string username)
    {
        return UsersSet.SingleOrDefaultAsync(u => u.Username == username);
    }

    public async Task<AdminUser> CreateAsync(AdminUser user)
    {
        return (await UsersSet.AddAsync(user)).Entity;
    }

    public Task<AdminUser> UpdateAsync(AdminUser adminUser)
    {
        return Task.FromResult(UsersSet.Update(adminUser).Entity);
    }

    public async Task<AdminUser> DeleteAsync(int id)
    {
        var user = (await GetByIdAsync(id))!;
        return UsersSet.Remove(user).Entity;
    }
}