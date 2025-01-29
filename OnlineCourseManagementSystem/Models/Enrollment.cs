using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace OnlineCourseManagementSystem.Models
{
    public class Enrollment
    {
        [Key]
        public int EnrollmentId { get; set; }

        [Required]
        [ForeignKey("Student")]
        public int StudentId { get; set; }

        [Required]
        [ForeignKey("Course")]
        public int CourseId { get; set; }

        [Range(2.0, 6.0)]
        public double? Grade { get; set; }

        [Required]
        public DateTime EnrollmentDate { get; set; }

        [Required]
        public bool IsCompleted { get; set; }

        [NotMapped]
        public Student? Student { get; set; }
        [NotMapped]
        public Course? Course { get; set; }
    }
}
