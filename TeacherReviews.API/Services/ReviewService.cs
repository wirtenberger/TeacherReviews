using System.Linq.Expressions;
using TeacherReviews.API.Contracts.Repositories;
using TeacherReviews.Domain.Entities;
using TeacherReviews.Domain.Exceptions;

namespace TeacherReviews.API.Services;

public class ReviewService
{
    private readonly IReviewRepository _reviewRepository;
    private readonly ITeacherRepository _teacherRepository;
    private readonly UnitOfWork _unitOfWork;

    public ReviewService(IReviewRepository reviewRepository, ITeacherRepository teacherRepository,
                         UnitOfWork unitOfWork)
    {
        _reviewRepository = reviewRepository;
        _teacherRepository = teacherRepository;
        _unitOfWork = unitOfWork;
    }

    public Task<IEnumerable<Review>> GetAllAsync(Expression<Func<Review, bool>>? filter = null)
    {
        return _reviewRepository.GetAllAsync(filter);
    }

    public async Task<Review> CreateAsync(Review item)
    {
        if (await _teacherRepository.GetByIdAsync(item.TeacherId) is null)
        {
            throw new EntityNotFoundException(typeof(Teacher), item.TeacherId);
        }

        var review = await _reviewRepository.AddAsync(item);
        await _unitOfWork.SaveChangesAsync();
        return review;
    }

    public async Task<Review> DeleteAsync(string id)
    {
        if (await _reviewRepository.GetByIdAsync(id) is null)
        {
            throw new EntityNotFoundException(typeof(Review), id);
        }

        var review = await _reviewRepository.DeleteAsync(id);
        await _unitOfWork.SaveChangesAsync();
        return review;
    }
}