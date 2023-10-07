using TeacherReviews.API.Contracts.Requests.Review;
using TeacherReviews.Domain.DTO;
using TeacherReviews.Domain.Entities;

namespace TeacherReviews.API.Mapping;

public static class ReviewMapper
{
    public static Review ToReview(this CreateReviewRequest createReviewRequest)
    {
        return new Review
        {
            Id = Guid.NewGuid().ToString(),
            Rate = createReviewRequest.Rate,
            Text = createReviewRequest.Text,
            CreateDate = DateOnly.FromDateTime(DateTime.Now),
            TeacherId = createReviewRequest.TeacherId,
        };
    }

    public static ReviewDto ToDto(this Review review)
    {
        return new ReviewDto
        {
            Id = review.Id,
            Rate = review.Rate,
            Text = review.Text,
            CreateDate = review.CreateDate,
            TeacherId = review.TeacherId,
        };
    }
}