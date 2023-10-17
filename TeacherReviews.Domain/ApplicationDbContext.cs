using Microsoft.EntityFrameworkCore;
using TeacherReviews.Domain.Entities;

namespace TeacherReviews.Domain;

public class ApplicationDbContext : DbContext
{
    public DbSet<City> Cities { get; set; } = default!;
    public DbSet<University> Universities { get; set; } = default!;
    public DbSet<Teacher> Teachers { get; set; } = default!;
    public DbSet<Review> Reviews { get; set; } = default!;

    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
    {
    }
}