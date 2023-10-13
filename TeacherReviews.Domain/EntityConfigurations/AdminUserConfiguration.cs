using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TeacherReviews.Domain.Entities;

namespace TeacherReviews.Domain.Configuration;

public class AdminUserConfiguration : IEntityTypeConfiguration<AdminUser>
{
    public void Configure(EntityTypeBuilder<AdminUser> builder)
    {
        builder.HasIndex(u => u.Username).IsUnique();
        builder.HasData(
            new AdminUser()
            {
                Id = 1,
                Username = "AbsolutelyUnpredictableId",
                Password = ""
            }
        );
    }
}