using Microsoft.EntityFrameworkCore;
using TeacherReviews.Data.Entities;

namespace TeacherReviews.Data;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
    {
    }

    public DbSet<City> Cities { get; set; } = default!;
    public DbSet<University> Universities { get; set; } = default!;
    public DbSet<Teacher> Teachers { get; set; } = default!;
    public DbSet<Review> Reviews { get; set; } = default!;
}