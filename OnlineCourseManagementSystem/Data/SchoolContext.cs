using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using OnlineCourseManagementSystem.Models;

namespace OnlineCourseManagementSystem.Data
{
    public class SchoolContext : IdentityDbContext
    {
        public SchoolContext(DbContextOptions<SchoolContext> options)
            : base(options)
        {
        }

        public DbSet<Course> Courses { get; set; }
        public DbSet<Student> Students { get; set; }
        public DbSet<Enrollment> Enrollments { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Course>(entity =>
            {
                entity.Property(c => c.Title)
                      .HasMaxLength(100)
                      .IsRequired();

                entity.Property(c => c.Description)
                      .HasMaxLength(500)
                      .IsRequired();

                entity.Property(c => c.Credits)
                      .IsRequired();

                entity.Property(c => c.StartDate)
                      .IsRequired();

                entity.Property(c => c.EndDate)
                      .IsRequired();

                entity.Property(c => c.IsActive)
                      .IsRequired();
            });

            modelBuilder.Entity<Student>(entity =>
            {
                entity.Property(s => s.FirstName)
                      .HasMaxLength(50)
                      .IsRequired();

                entity.Property(s => s.LastName)
                      .HasMaxLength(50)
                      .IsRequired();

                entity.Property(s => s.Email)
                      .HasMaxLength(100)
                      .IsRequired();

                entity.Property(s => s.DateOfBirth)
                      .IsRequired();

                entity.Property(s => s.EnrollmentDate)
                      .IsRequired();
            });

            modelBuilder.Entity<Enrollment>(entity =>
            {
                entity.Property(e => e.Grade)
                      .HasColumnType("decimal(3, 2)")
                      .IsRequired(false);

                entity.Property(e => e.EnrollmentDate)
                      .IsRequired();

                entity.Property(e => e.IsCompleted)
                      .IsRequired();

                entity.HasOne(e => e.Student)
                      .WithMany(s => s.Enrollments)
                      .HasForeignKey(e => e.StudentId)
                      .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(e => e.Course)
                      .WithMany(c => c.Enrollments)
                      .HasForeignKey(e => e.CourseId)
                      .OnDelete(DeleteBehavior.Cascade);
            });
        }
    }
}
