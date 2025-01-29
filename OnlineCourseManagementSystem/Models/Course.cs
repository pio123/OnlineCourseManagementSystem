using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace OnlineCourseManagementSystem.Models
{
    public class Course
    {
        [Key]
        public int CourseId { get; set; }

        [Required]
        [MaxLength(100)]
        public string Title { get; set; }

        [Required]
        [MaxLength(500)]
        public string Description { get; set; }

        [Required]
        [Range(1, 10)]
        public int Credits { get; set; }

        [Required]
        public DateTime StartDate { get; set; }

        [Required]
        public DateTime EndDate { get; set; }

        [Required]
        public bool IsActive { get; set; }

        [NotMapped]
        public ICollection<Enrollment>? Enrollments { get; set; }
    }
}
