using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using TeacherReviews.API.Contracts.Repositories;
using TeacherReviews.Domain;
using TeacherReviews.Domain.Entities;

namespace TeacherReviews.API.Repositories;

public class CityRepository : ICityRepository
{
    private readonly ApplicationDbContext _context;

    public CityRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    private DbSet<City> CitiesSet => _context.Cities;

    public async Task<City> DeleteAsync(string id)
    {
        var city = await GetByIdAsync(id);
        CitiesSet.Remove(city);
        return city;
    }

    public Task<City> UpdateAsync(City item)
    {
        return Task.FromResult(CitiesSet.Update(item).Entity);
    }

    public async Task<City?> GetByIdAsync(string id)
    {
        var city = await CitiesSet.FindAsync(id);
        return city;
    }

    public async Task<IEnumerable<City>> GetAllAsync(Expression<Func<City, bool>>? filter = null)
    {
        if (filter is null)
        {
            return await CitiesSet.ToListAsync();
        }

        return await CitiesSet.Where(filter).ToListAsync();
    }

    public async Task<City> AddAsync(City item)
    {
        await CitiesSet.AddAsync(item);
        return item;
    }

    public async Task<bool> ExistsAsync(string id)
    {
        return await CitiesSet.AsNoTrackingWithIdentityResolution().AnyAsync(c => c.Id == id);
    }
}