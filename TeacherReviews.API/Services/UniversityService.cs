using System.Linq.Expressions;
using TeacherReviews.API.Contracts.Repositories;
using TeacherReviews.Domain.Entities;
using TeacherReviews.Domain.Exceptions;

namespace TeacherReviews.API.Services;

public class UniversityService
{
    private readonly ICityRepository _cityRepository;
    private readonly UnitOfWork _unitOfWork;
    private readonly IUniversityRepository _universityRepository;

    public UniversityService(IUniversityRepository universityRepository,
                             ICityRepository cityRepository,
                             UnitOfWork unitOfWork)
    {
        _universityRepository = universityRepository;
        _cityRepository = cityRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<University> GetByIdAsync(string id)
    {
        var university = await _universityRepository.GetByIdAsync(id);
        if (university is null)
        {
            throw new EntityNotFoundException(typeof(University), id);
        }

        return university;
    }

    public Task<IEnumerable<University>> GetAllAsync(Expression<Func<University, bool>>? filter = null)
    {
        return _universityRepository.GetAllAsync(filter);
    }

    public async Task<University> CreateAsync(University item)
    {
        if (await _cityRepository.GetByIdAsync(item.CityId) is null)
        {
            throw new EntityNotFoundException(typeof(City), item.CityId);
        }

        Expression<Func<University, bool>> filter = u => u.Name == item.Name && u.CityId == item.CityId;

        if (
            (await _universityRepository.GetAllAsync(filter)).Any()
        )
        {
            throw new EntityExistsException(typeof(University), nameof(University.Name), item.Name);
        }

        var university = await _universityRepository.AddAsync(item);
        await _unitOfWork.SaveChangesAsync();
        return university;
    }

    public async Task<University> DeleteAsync(string id)
    {
        if (await _universityRepository.GetByIdAsync(id) is null)
        {
            throw new EntityNotFoundException(typeof(University), id);
        }

        var university = await _universityRepository.DeleteAsync(id);
        await _unitOfWork.SaveChangesAsync();
        return university;
    }

    public async Task<University> UpdateAsync(University item)
    {
        if (await _universityRepository.GetByIdAsync(item.Id) is null)
        {
            throw new EntityNotFoundException(typeof(University), item.Id);
        }

        if (await _cityRepository.GetByIdAsync(item.CityId) is null)
        {
            throw new EntityNotFoundException(typeof(City), item.CityId);
        }

        Expression<Func<University, bool>> filter = u =>
            u.Name == item.Name && u.CityId == item.CityId && u.Id != item.Id;

        if (
            (await _universityRepository.GetAllAsync(filter)).Any()
        )
        {
            throw new EntityExistsException(typeof(University), nameof(University.Name), item.Name);
        }

        var university = await _universityRepository.UpdateAsync(item);
        await _unitOfWork.SaveChangesAsync();
        return university;
    }
}