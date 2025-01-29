using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OnlineCourseManagementSystem.Data;
using OnlineCourseManagementSystem.Models;
using OnlineCourseManagementSystem.Reusables;
using System.Linq;
using System.Threading.Tasks;

namespace OnlineCourseManagementSystem.Controllers
{
    [Authorize]
    public class CourseController : Controller
    {
        private readonly SchoolContext _context;
        private readonly UserManager<IdentityUser> _userManager;

        public CourseController(SchoolContext context, UserManager<IdentityUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        [AllowAnonymous]
        public async Task<IActionResult> Index(string search, string sortOrder, int? pageNumber)
        {
            var userEmail = _userManager.GetUserName(User);
            var student = await _context.Students.FirstOrDefaultAsync(s => s.Email == userEmail);
            var enrolledCourseIds = student != null ? _context.Enrollments.Where(e => e.StudentId == student.StudentId)
                                                     .Select(e => e.CourseId)
                                                     .ToHashSet() : new HashSet<int>();

            var courses = _context.Courses.AsQueryable();

            if (!string.IsNullOrEmpty(search))
            {
                courses = courses.Where(c => c.Title.Contains(search) || c.Description.Contains(search));
            }

            ViewData["TitleSortParam"] = string.IsNullOrEmpty(sortOrder) ? "title_desc" : "";
            ViewData["CreditsSortParam"] = sortOrder == "Credits" ? "credits_desc" : "Credits";

            switch (sortOrder)
            {
                case "title_desc":
                    courses = courses.OrderByDescending(c => c.Title);
                    break;
                case "Credits":
                    courses = courses.OrderBy(c => c.Credits);
                    break;
                case "credits_desc":
                    courses = courses.OrderByDescending(c => c.Credits);
                    break;
                default:
                    courses = courses.OrderBy(c => c.Title);
                    break;
            }

            int pageSize = 5;
            var paginatedList = await PaginatedList<Course>.CreateAsync(courses.AsNoTracking(), pageNumber ?? 1, pageSize);

            ViewBag.EnrolledCourses = enrolledCourseIds;
            return View(paginatedList);
        }

        [Authorize(Roles = "Admin,User")]
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
                return NotFound();

            var course = await _context.Courses.FirstOrDefaultAsync(c => c.CourseId == id);
            if (course == null)
                return NotFound();

            return View(course);
        }

        [Authorize(Roles = "Admin")]
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Create([Bind("CourseId,Title,Description,Credits,StartDate,EndDate,IsActive")] Course course)
        {
            if (ModelState.IsValid)
            {
                _context.Add(course);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(course);
        }

        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
                return NotFound();

            var course = await _context.Courses.FindAsync(id);
            if (course == null)
                return NotFound();

            return View(course);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Edit(int id, [Bind("CourseId,Title,Description,Credits,StartDate,EndDate,IsActive")] Course course)
        {
            if (id != course.CourseId)
                return NotFound();

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(course);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!CourseExists(course.CourseId))
                        return NotFound();
                    else
                        throw;
                }
                return RedirectToAction(nameof(Index));
            }
            return View(course);
        }

        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
                return NotFound();

            var course = await _context.Courses.FirstOrDefaultAsync(c => c.CourseId == id);
            if (course == null)
                return NotFound();

            return View(course);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var course = await _context.Courses.FindAsync(id);
            _context.Courses.Remove(course);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool CourseExists(int id)
        {
            return _context.Courses.Any(e => e.CourseId == id);
        }
    }
}
