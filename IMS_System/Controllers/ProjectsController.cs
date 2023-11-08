using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using IMS_System.Models.Entities;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;

namespace IMS_System.Controllers
{
    [Authorize]
    public class ProjectsController : Controller
    {
        private readonly ImsSystemContext _context;

        public ProjectsController(ImsSystemContext context)
        {
            _context = context;
        }

        // GET: Projects
        public async Task<IActionResult> Index()
        {
            var currentUserId = GetCurrentUserId();

            var projectIds = _context.ProjectMembers
                                    .Where(pm => pm.UserId == currentUserId)
                                    .Select(pm => pm.ProjectId)
                                    .Distinct();

            // Including Milestones in the query
            var projects = await _context.Projects
                                        .Include(p => p.Status)
                                        .Include(p => p.Milestones) // Including milestones
                                        .Where(p => projectIds.Contains(p.ProjectId))
                                        .ToListAsync();

            return View(projects);
        }



        private int GetCurrentUserId()
        {
            //depend on your authentication method.
            return int.Parse(HttpContext.Session.GetString("UserId")); 
        }

        // GET: Projects/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var project = await _context.Projects
                .Include(p => p.Status)
                .FirstOrDefaultAsync(m => m.ProjectId == id);

            if (project == null)
            {
                return NotFound();
            }

            return View(project);
        }

        // GET: Projects/Create
        public IActionResult Create()
        {
            // Assuming StatusId is a foreign key for the Status entity
            ViewData["StatusId"] = new SelectList(_context.Statuses, "StatusId", "StatusName");
            return View();
        }

        // POST: Projects/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("EnglishName,VietnameseName,StatusId,Description")] Project project)
        {
            if (ModelState.IsValid)
            {
                _context.Add(project);
                await _context.SaveChangesAsync(); // Save project so it gets an ID

                // Create a new Project_member and assign the current user as the leader
                var projectMember = new ProjectMember
                {
                    ProjectId = project.ProjectId, // Use the newly created project's ID
                    UserId = int.Parse(HttpContext.Session.GetString("UserId")),
                IsLeader = 1 // Assuming 'IsLeader' is an int as per your database script
                };
                _context.Add(projectMember);
                await _context.SaveChangesAsync(); // Save the project member

                return RedirectToAction(nameof(Index));
            }

            ViewData["StatusId"] = new SelectList(_context.Statuses, "StatusId", "StatusName", project.StatusId);
            return View(project);
        }

        // GET: Projects/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var project = await _context.Projects
                .Include(p => p.Milestones) // Include the Milestones navigation property
                .Include(p => p.ProjectMembers)
                    .ThenInclude(pm => pm.User)
                .AsNoTracking()
                .SingleOrDefaultAsync(p => p.ProjectId == id);

            if (project == null)
            {
                return NotFound();
            }

            int currentUserId = int.Parse(HttpContext.Session.GetString("UserId"));  
            bool isUserLeader = project.ProjectMembers.Any(pm => pm.UserId == currentUserId && pm.IsLeader == 1);

            ViewData["IsUserLeader"] = isUserLeader;
            ViewData["StatusId"] = new SelectList(_context.Statuses, "StatusId", "StatusName", project.StatusId);

            // If you need to send the milestones to the view, for instance to a dropdown, do it as follows:
            ViewData["MilestoneId"] = new SelectList(project.Milestones, "MilestoneId", "Name");

            return View(project);
        }


   
        // POST: Projects/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("ProjectId,EnglishName,VietnameseName,StatusId,Description")] Project project)
        {
            if (id != project.ProjectId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(project);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ProjectExists(project.ProjectId))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            ViewData["StatusId"] = new SelectList(_context.Statuses, "StatusId", "StatusName", project.StatusId);
            return View(project);
        }

        // GET: Projects/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.Projects == null)
            {
                return NotFound();
            }

            var project = await _context.Projects
                .Include(p => p.Status)
                .FirstOrDefaultAsync(m => m.ProjectId == id);
            if (project == null)
            {
                return NotFound();
            }

            return View(project);
        }

        // POST: Projects/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.Projects == null)
            {
                return Problem("Entity set 'ImsSystemContext.Projects'  is null.");
            }
            var project = await _context.Projects.FindAsync(id);
            if (project != null)
            {
                _context.Projects.Remove(project);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ProjectExists(int id)
        {
            return (_context.Projects?.Any(e => e.ProjectId == id)).GetValueOrDefault();
        }
    }
}
