using System.Linq.Expressions;
using TeacherReviews.API.Contracts.Repositories;
using TeacherReviews.Domain.Entities;
using TeacherReviews.Domain.Exceptions;

namespace TeacherReviews.API.Services;

public class TeacherService
{
    private readonly IReviewRepository _reviewRepository;
    private readonly ITeacherRepository _teacherRepository;
    private readonly UnitOfWork _unitOfWork;
    private readonly IUniversityRepository _universityRepository;

    public TeacherService(ITeacherRepository teacherRepository, IUniversityRepository universityRepository,
                          IReviewRepository reviewRepository, UnitOfWork unitOfWork)
    {
        _teacherRepository = teacherRepository;
        _universityRepository = universityRepository;
        _reviewRepository = reviewRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Teacher> GetByIdAsync(string id)
    {
        var teacher = await _teacherRepository.GetByIdAsync(id);
        if (teacher is null)
        {
            throw new EntityNotFoundException(typeof(Teacher), id);
        }

        return teacher;
    }

    public Task<IEnumerable<Teacher>> GetAllAsync(Expression<Func<Teacher, bool>>? filter = null)
    {
        return _teacherRepository.GetAllAsync(filter);
    }

    public async Task<Teacher> CreateAsync(Teacher item)
    {
        if (await _universityRepository.GetByIdAsync(item.UniversityId) is null)
        {
            throw new EntityNotFoundException(typeof(University), item.UniversityId);
        }

        var university = await _teacherRepository.AddAsync(item);
        await _unitOfWork.SaveChangesAsync();
        return university;
    }

    public async Task<Teacher> DeleteAsync(string id)
    {
        if (await _teacherRepository.GetByIdAsync(id) is null)
        {
            throw new EntityNotFoundException(typeof(Teacher), id);
        }

        var teacher = await _teacherRepository.DeleteAsync(id);
        await _unitOfWork.SaveChangesAsync();
        return teacher;
    }

    public async Task<Teacher> UpdateAsync(Teacher item)
    {
        if (await _teacherRepository.GetByIdAsync(item.Id) is null)
        {
            throw new EntityNotFoundException(typeof(Teacher), item.Id);
        }

        if (await _universityRepository.GetByIdAsync(item.UniversityId) is null)
        {
            throw new EntityNotFoundException(typeof(University), item.UniversityId);
        }

        var teacher = await _teacherRepository.UpdateAsync(item);
        await _unitOfWork.SaveChangesAsync();
        return teacher;
    }

    public async Task<University> GetTeachersUniversity(string id)
    {
        return (await GetByIdAsync(id)).University;
    }

    public Task<IEnumerable<Review>> GetTeachersReviews(string id)
    {
        return _reviewRepository.GetAllAsync(r => r.TeacherId == id);
    }
}