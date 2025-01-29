using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using OnlineCourseManagementSystem.Models;
using System.Linq;
using System.Threading.Tasks;

namespace OnlineCourseManagementSystem.Controllers
{
    [Authorize(Roles = "Admin")]
    public class RoleManagementController : Controller
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        public RoleManagementController(UserManager<IdentityUser> userManager, RoleManager<IdentityRole> roleManager)
        {
            _userManager = userManager;
            _roleManager = roleManager;
        }

        public IActionResult Index()
        {
            var users = _userManager.Users.ToList();
            return View(users);
        }

        public async Task<IActionResult> ManageRoles(string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null) return NotFound();

            var userRoles = await _userManager.GetRolesAsync(user);
            var allRoles = _roleManager.Roles.Select(r => r.Name).ToList();

            ViewBag.AllRoles = allRoles; // Pass roles to the view

            var model = new ManageRolesViewModel
            {
                UserId = user.Id,
                UserName = user.UserName,
                CurrentRole = userRoles.FirstOrDefault() // Get current role (assuming single role per user)
            };

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> UpdateRoles(string userId, string SelectedRole)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null) return NotFound();

            var userRoles = await _userManager.GetRolesAsync(user);
            var allRoles = _roleManager.Roles.Select(r => r.Name).ToList();

            // Remove all current roles
            await _userManager.RemoveFromRolesAsync(user, userRoles);

            // Assign the newly selected role
            if (!string.IsNullOrEmpty(SelectedRole) && allRoles.Contains(SelectedRole))
            {
                await _userManager.AddToRoleAsync(user, SelectedRole);
            }

            return RedirectToAction(nameof(Index));
        }
    }
}
