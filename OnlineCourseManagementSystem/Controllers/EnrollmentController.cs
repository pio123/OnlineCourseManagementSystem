using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using OnlineCourseManagementSystem.Data;
using OnlineCourseManagementSystem.Models;
using OnlineCourseManagementSystem.Reusables;
using System.Linq;
using System.Threading.Tasks;

namespace OnlineCourseManagementSystem.Controllers
{
    [Authorize]
    public class EnrollmentController : Controller
    {
        private readonly SchoolContext _context;
        private readonly UserManager<IdentityUser> _userManager;

        public EnrollmentController(SchoolContext context, UserManager<IdentityUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        [Authorize(Roles = "Admin,User")]
        public async Task<IActionResult> Index(int? pageNumber)
        {
            var enrollments = _context.Enrollments
                .Include(e => e.Student)
                .Include(e => e.Course);

            int pageSize = 5;
            return View(await PaginatedList<Enrollment>.CreateAsync(enrollments.AsNoTracking(), pageNumber ?? 1, pageSize));
        }

        [Authorize(Roles = "Admin,User")]
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
                return NotFound();

            var enrollment = await _context.Enrollments
                .Include(e => e.Student)
                .Include(e => e.Course)
                .FirstOrDefaultAsync(e => e.EnrollmentId == id);

            if (enrollment == null)
                return NotFound();

            return View(enrollment);
        }

        [HttpPost]
        [Authorize(Roles = "User")]
        public async Task<IActionResult> Enroll(int courseId)
        {
            var userEmail = _userManager.GetUserName(User);
            var student = await _context.Students.FirstOrDefaultAsync(s => s.Email == userEmail);

            if (student == null)
            {
                return BadRequest("Student not found.");
            }

            bool alreadyEnrolled = await _context.Enrollments.AnyAsync(e => e.StudentId == student.StudentId && e.CourseId == courseId);
            if (alreadyEnrolled)
            {
                return BadRequest("Already enrolled in this course.");
            }

            var enrollment = new Enrollment
            {
                StudentId = student.StudentId,
                CourseId = courseId,
                EnrollmentDate = DateTime.UtcNow,
                IsCompleted = false,
                Grade = null
            };

            _context.Enrollments.Add(enrollment);
            await _context.SaveChangesAsync();

            return RedirectToAction("Index", "Course");
        }

        [Authorize(Roles = "Admin")]
        public IActionResult Create()
        {
            ViewBag.Students = new SelectList(_context.Students, "StudentId", "FirstName");
            ViewBag.Courses = new SelectList(_context.Courses, "CourseId", "Title");
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Create([Bind("EnrollmentId,StudentId,CourseId,Grade,EnrollmentDate,IsCompleted")] Enrollment enrollment)
        {
            if (ModelState.IsValid)
            {
                _context.Add(enrollment);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewBag.Students = new SelectList(_context.Students, "StudentId", "FirstName", enrollment.StudentId);
            ViewBag.Courses = new SelectList(_context.Courses, "CourseId", "Title", enrollment.CourseId);
            return View(enrollment);
        }

        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
                return NotFound();

            var enrollment = await _context.Enrollments.FindAsync(id);
            if (enrollment == null)
                return NotFound();

            ViewBag.Students = new SelectList(_context.Students, "StudentId", "FirstName", enrollment.StudentId);
            ViewBag.Courses = new SelectList(_context.Courses, "CourseId", "Title", enrollment.CourseId);
            return View(enrollment);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Edit(int id, [Bind("EnrollmentId,StudentId,CourseId,Grade,EnrollmentDate,IsCompleted")] Enrollment enrollment)
        {
            if (id != enrollment.EnrollmentId)
                return NotFound();

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(enrollment);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!EnrollmentExists(enrollment.EnrollmentId))
                        return NotFound();
                    else
                        throw;
                }
                return RedirectToAction(nameof(Index));
            }
            ViewBag.Students = new SelectList(_context.Students, "StudentId", "FirstName", enrollment.StudentId);
            ViewBag.Courses = new SelectList(_context.Courses, "CourseId", "Title", enrollment.CourseId);
            return View(enrollment);
        }

        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
                return NotFound();

            var enrollment = await _context.Enrollments
                .Include(e => e.Student)
                .Include(e => e.Course)
                .FirstOrDefaultAsync(e => e.EnrollmentId == id);

            if (enrollment == null)
                return NotFound();

            return View(enrollment);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var enrollment = await _context.Enrollments.FindAsync(id);
            _context.Enrollments.Remove(enrollment);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool EnrollmentExists(int id)
        {
            return _context.Enrollments.Any(e => e.EnrollmentId == id);
        }
    }
}
