using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using TeacherReviews.API.Contracts.Repositories;
using TeacherReviews.Domain;
using TeacherReviews.Domain.Entities;

namespace TeacherReviews.API.Repositories;

public class TeacherRepository : ITeacherRepository
{
    private readonly ApplicationDbContext _context;

    private DbSet<Teacher> TeachersSet => _context.Teachers;

    public TeacherRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Teacher> DeleteAsync(string id)
    {
        var teacher = (await GetByIdAsync(id))!;
        TeachersSet.Remove(teacher!);
        return teacher;
    }

    public Task<Teacher> UpdateAsync(Teacher item)
    {
        return Task.FromResult(TeachersSet.Update(item).Entity);
    }

    public async Task<Teacher?> GetByIdAsync(string id)
    {
        var city = await TeachersSet.FindAsync(id);
        return city;
    }

    public async Task<IEnumerable<Teacher>> GetAllAsync(Expression<Func<Teacher, bool>>? filter = null)
    {
        if (filter is null)
        {
            return await TeachersSet.ToListAsync();
        }

        return await TeachersSet.Where(filter).ToListAsync();
    }

    public async Task<Teacher> AddAsync(Teacher item)
    {
        await TeachersSet.AddAsync(item);
        return item;
    }
}