using Microsoft.EntityFrameworkCore;
using TeacherReviews.Domain.Entities;

namespace TeacherReviews.Domain;

public class UsersDbContext : DbContext
{
    public DbSet<AdminUser> AdminUsers { get; set; } = default!;

    public UsersDbContext(DbContextOptions<UsersDbContext> options) : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.ApplyConfigurationsFromAssembly(typeof(UsersDbContext).Assembly);
    }
}