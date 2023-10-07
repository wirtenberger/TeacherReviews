using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using TeacherReviews.API.Contracts.Repositories;
using TeacherReviews.Domain;
using TeacherReviews.Domain.Entities;

namespace TeacherReviews.API.Repositories;

public class UniversityRepository : IUniversityRepository
{
    private readonly ApplicationDbContext _context;

    public UniversityRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    private DbSet<University> UniversitiesSet => _context.Universities;

    public async Task<University> DeleteAsync(string id)
    {
        var city = await GetByIdAsync(id);
        UniversitiesSet.Remove(city);
        return city;
    }

    public Task<University> UpdateAsync(University item)
    {
        return Task.FromResult(UniversitiesSet.Update(item).Entity);
    }

    public async Task<University?> GetByIdAsync(string id)
    {
        var city = await UniversitiesSet.FindAsync(id);
        return city;
    }

    public async Task<IEnumerable<University>> GetAllAsync(Expression<Func<University, bool>>? filter = null)
    {
        if (filter is null)
        {
            return await UniversitiesSet.ToListAsync();
        }

        return await UniversitiesSet.Where(filter).ToListAsync();
    }

    public async Task<University> AddAsync(University item)
    {
        await UniversitiesSet.AddAsync(item);
        return item;
    }

    public async Task<bool> ExistsAsync(string id)
    {
        return await UniversitiesSet.AsNoTrackingWithIdentityResolution().AnyAsync(u => u.Id == id);
    }
}