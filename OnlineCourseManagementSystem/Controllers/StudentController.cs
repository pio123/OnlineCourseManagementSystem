    using Microsoft.AspNetCore.Authorization;
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
        public class StudentController : Controller
        {
            private readonly SchoolContext _context;

            public StudentController(SchoolContext context)
            {
                _context = context;
            }

            [AllowAnonymous]
            public async Task<IActionResult> Index(string search, string sortOrder, int? pageNumber)
            {
                var students = from s in _context.Students select s;

                if (!string.IsNullOrEmpty(search))
                {
                    students = students.Where(s => s.FirstName.Contains(search) || s.LastName.Contains(search) || s.Email.Contains(search));
                }

                ViewData["NameSortParam"] = string.IsNullOrEmpty(sortOrder) ? "name_desc" : "";
                ViewData["EmailSortParam"] = sortOrder == "Email" ? "email_desc" : "Email";

                switch (sortOrder)
                {
                    case "name_desc":
                        students = students.OrderByDescending(s => s.LastName);
                        break;
                    case "Email":
                        students = students.OrderBy(s => s.Email);
                        break;
                    case "email_desc":
                        students = students.OrderByDescending(s => s.Email);
                        break;
                    default:
                        students = students.OrderBy(s => s.LastName);
                        break;
                }

                int pageSize = 5;
                return View(await PaginatedList<Student>.CreateAsync(students.AsNoTracking(), pageNumber ?? 1, pageSize));
            }

            [Authorize(Roles = "Admin,User")]
            public async Task<IActionResult> Details(int? id)
            {
                if (id == null)
                    return NotFound();

                var student = await _context.Students.FirstOrDefaultAsync(s => s.StudentId == id);
                if (student == null)
                    return NotFound();

                return View(student);
            }

            [Authorize(Roles = "Admin")]
            public IActionResult Create()
            {
                return View();
            }

            [HttpPost]
            [ValidateAntiForgeryToken]
            [Authorize(Roles = "Admin")]
            public async Task<IActionResult> Create([Bind("StudentId,FirstName,LastName,Email,DateOfBirth,EnrollmentDate")] Student student)
            {
                if (ModelState.IsValid)
                {
                    _context.Add(student);
                    await _context.SaveChangesAsync();
                    return RedirectToAction(nameof(Index));
                }
                return View(student);
            }

            [Authorize(Roles = "Admin")]
            public async Task<IActionResult> Edit(int? id)
            {
                if (id == null)
                    return NotFound();

                var student = await _context.Students.FindAsync(id);
                if (student == null)
                    return NotFound();

                return View(student);
            }

            [HttpPost]
            [ValidateAntiForgeryToken]
            [Authorize(Roles = "Admin")]
            public async Task<IActionResult> Edit(int id, [Bind("StudentId,FirstName,LastName,Email,DateOfBirth,EnrollmentDate")] Student student)
            {
                if (id != student.StudentId)
                    return NotFound();

                if (ModelState.IsValid)
                {
                    try
                    {
                        _context.Update(student);
                        await _context.SaveChangesAsync();
                    }
                    catch (DbUpdateConcurrencyException)
                    {
                        if (!StudentExists(student.StudentId))
                            return NotFound();
                        else
                            throw;
                    }
                    return RedirectToAction(nameof(Index));
                }
                return View(student);
            }

            [Authorize(Roles = "Admin")]
            public async Task<IActionResult> Delete(int? id)
            {
                if (id == null)
                    return NotFound();

                var student = await _context.Students.FirstOrDefaultAsync(s => s.StudentId == id);
                if (student == null)
                    return NotFound();

                return View(student);
            }

            [HttpPost, ActionName("Delete")]
            [ValidateAntiForgeryToken]
            [Authorize(Roles = "Admin")]
            public async Task<IActionResult> DeleteConfirmed(int id)
            {
                var student = await _context.Students.FindAsync(id);
                _context.Students.Remove(student);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            private bool StudentExists(int id)
            {
                return _context.Students.Any(e => e.StudentId == id);
            }
        }
    }
