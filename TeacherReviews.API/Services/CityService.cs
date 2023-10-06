using System.Linq.Expressions;
using TeacherReviews.API.Contracts.Repositories;
using TeacherReviews.API.Repositories;
using TeacherReviews.Data;
using TeacherReviews.Data.Entities;
using TeacherReviews.Data.Exceptions;

namespace TeacherReviews.API.Services;

public class CityService
{
    private readonly ICityRepository _cityRepository;
    private readonly UnitOfWork _unitOfWork;
    private readonly IUniversityRepository _universityRepository;

    public CityService(ApplicationDbContext context, UnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
        _cityRepository = new CityRepository(context);
        _universityRepository = new UniversityRepository(context);
    }

    public async Task<City> GetByIdAsync(string id)
    {
        var city = await _cityRepository.GetByIdAsync(id);
        if (city is null)
        {
            throw new EntityNotFoundException(typeof(City), id);
        }

        return city;
    }

    public Task<IEnumerable<City>> GetAllAsync(Expression<Func<City, bool>>? filter = null)
    {
        return _cityRepository.GetAllAsync(filter);
    }

    public async Task<City> CreateAsync(City item)
    {
        if ((await _cityRepository.GetAllAsync(c => c.Name == item.Name)).Any())
        {
            throw new EntityExistsException(typeof(City), nameof(City.Name), item.Name);
        }

        var city = await _cityRepository.AddAsync(item);

        await _unitOfWork.SaveChangesAsync();

        return city;
    }

    public async Task<City> DeleteAsync(string id)
    {
        if (!await _cityRepository.ExistsAsync(id))
        {
            throw new EntityNotFoundException(typeof(City), id);
        }

        var city = await _cityRepository.DeleteAsync(id);

        await _unitOfWork.SaveChangesAsync();

        return city;
    }

    public async Task<City> UpdateAsync(City item)
    {
        if ((await _cityRepository.GetAllAsync(c => c.Name == item.Name)).Any())
        {
            throw new EntityExistsException(typeof(City), nameof(City.Name), item.Name);
        }

        var city = await _cityRepository.UpdateAsync(item);

        await _unitOfWork.SaveChangesAsync();

        return city;
    }

    public Task<IEnumerable<University>> GetCitysUniversitiesAsync(string id)
    {
        return _universityRepository.GetAllAsync(u => u.CityId == id);
    }
}