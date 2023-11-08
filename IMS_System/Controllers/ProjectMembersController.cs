using Microsoft.AspNetCore.Mvc;
using IMS_System.Models.Entities;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.CodeAnalysis.Differencing;

namespace IMS_System.Controllers
{
    public class ProjectMembersController : Controller
    {
        private readonly ImsSystemContext _context;

        public ProjectMembersController(ImsSystemContext context)
        {
            _context = context;
        }

        // This action is for showing the form where you can enter an email to add a member
        public IActionResult AddMemberForm()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> AddProjectMember(string email, int projectId)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == email);
            if (user == null)
            {
                return Json(new { success = false, message = "User not found." });
            }

            var projectMember = new ProjectMember
            {
                ProjectId = projectId,
                UserId = user.UserId,
                IsLeader = 0 // Assuming 0 is not a leader
            };

            _context.ProjectMembers.Add(projectMember);
            await _context.SaveChangesAsync();

            return Json(new { success = true });
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SetAsLeader(int projectId, int userId)
        {
            var projectMember = await _context.ProjectMembers
                .FirstOrDefaultAsync(pm => pm.ProjectId == projectId && pm.UserId == userId);

            if (projectMember != null)
            {
                // Set this member as leader
                projectMember.IsLeader = 1;

                // Optionally, unset the leader status for all other members
                var otherLeaders = _context.ProjectMembers
                    .Where(pm => pm.ProjectId == projectId && pm.UserId != userId && pm.IsLeader == 1);

                foreach (var leader in otherLeaders)
                {
                    leader.IsLeader = 0;
                }

                await _context.SaveChangesAsync();
                TempData["Success"] = "Member set as leader successfully.";

                return Json(new { success = true });
            }

            TempData["Error"] = "Member not found.";
            return Json(new { success = false });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> KickProjectMember(int projectId, int userId)
        {
            // Find the project member in the database
            var projectMember = await _context.ProjectMembers
                .FirstOrDefaultAsync(pm => pm.ProjectId == projectId && pm.UserId == userId);

            if (projectMember == null)
            {
                TempData["Error"] = "Member not found.";
                return RedirectToAction("Edit", "Projects", new { id = projectId });
            }

            _context.ProjectMembers.Remove(projectMember);
            await _context.SaveChangesAsync();

            TempData["Success"] = "Member kicked successfully.";
            return RedirectToAction("Edit", "Projects", new { id = projectId });
        }



    }
}
