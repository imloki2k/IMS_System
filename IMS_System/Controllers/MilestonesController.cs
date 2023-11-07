using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using IMS_System.Models.Entities;
using NToastNotify;

namespace IMS_System.Controllers
{
    public class MilestonesController : Controller
    {
        private readonly ImsSystemContext _context;
        private readonly IToastNotification _toastNotification;
        public MilestonesController(ImsSystemContext context, IToastNotification toastNotification)
        {
            _context = context;
            _toastNotification = toastNotification;
        }

        // GET: Milestones
        public async Task<IActionResult> Index()
        {
            var imsSystemContext = _context.Milestones.Include(m => m.Assignment).Include(m => m.Class).Include(m => m.Issue).Include(m => m.Project).Include(m => m.Subject);
            return View(await imsSystemContext.ToListAsync());
        }

        // GET: Milestones/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.Milestones == null)
            {
                return NotFound();
            }

            var milestone = await _context.Milestones
                .Include(m => m.Assignment)
                .Include(m => m.Class)
                .Include(m => m.Issue)
                .Include(m => m.Project)
                .Include(m => m.Subject)
                .FirstOrDefaultAsync(m => m.MilestoneId == id);
            if (milestone == null)
            {
                return NotFound();
            }

            return View(milestone);
        }

        // GET: Milestones/Create
        public IActionResult Create()
        {
            ViewData["AssignmentId"] = new SelectList(_context.Assignments, "AssignmentId", "AssingmentName");
            ViewData["ClassId"] = new SelectList(_context.Classes, "ClassId", "ClassName");
            ViewData["IssueId"] = new SelectList(_context.Issues, "IssueId", "IssueName");
            ViewData["ProjectId"] = new SelectList(_context.Projects, "ProjectId", "EnglishName");
            ViewData["SubjectId"] = new SelectList(_context.Subjects, "SubjectId", "SubjectName");
            return View();
        }

        // POST: Milestones/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("MilestoneId,ClassId,ProjectId,IssueId,AssignmentId,SubjectId,Milestone1,MilestoneDescription")] Milestone milestone)
        {
            if (ModelState.IsValid)
            {
                _context.Add(milestone);
                await _context.SaveChangesAsync();
                _toastNotification.AddSuccessToastMessage("Create successful!");
                return RedirectToAction("Details", "Classes");
            }
            ViewData["AssignmentId"] = new SelectList(_context.Assignments, "AssignmentId", "AssingmentName", milestone.AssignmentId);
            ViewData["ClassId"] = new SelectList(_context.Classes, "ClassId", "ClassName", milestone.ClassId);
            ViewData["IssueId"] = new SelectList(_context.Issues, "IssueId", "IssueName", milestone.IssueId);
            ViewData["ProjectId"] = new SelectList(_context.Projects, "ProjectId", "EnglishName", milestone.ProjectId);
            ViewData["SubjectId"] = new SelectList(_context.Subjects, "SubjectId", "SubjectName", milestone.SubjectId);
            return View(milestone);
        }

        // GET: Milestones/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.Milestones == null)
            {
                return NotFound();
            }

            var milestone = await _context.Milestones.FindAsync(id);
            if (milestone == null)
            {
                return NotFound();
            }
            ViewData["AssignmentId"] = new SelectList(_context.Assignments, "AssignmentId", "AssingmentName", milestone.AssignmentId);
            ViewData["ClassId"] = new SelectList(_context.Classes, "ClassId", "ClassName", milestone.ClassId);
            ViewData["IssueId"] = new SelectList(_context.Issues, "IssueId", "IssueName", milestone.IssueId);
            ViewData["ProjectId"] = new SelectList(_context.Projects, "ProjectId", "EnglishName", milestone.ProjectId);
            ViewData["SubjectId"] = new SelectList(_context.Subjects, "SubjectId", "SubjectName", milestone.SubjectId);
            return View(milestone);
        }

        // POST: Milestones/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("MilestoneId,ClassId,ProjectId,IssueId,AssignmentId,SubjectId,Milestone1,MilestoneDescription")] Milestone milestone)
        {
            if (id != milestone.MilestoneId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(milestone);
                    _toastNotification.AddSuccessToastMessage("Update successful!");
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!MilestoneExists(milestone.MilestoneId))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction("Details", "Classes");
            }
            ViewData["AssignmentId"] = new SelectList(_context.Assignments, "AssignmentId", "AssingmentName", milestone.AssignmentId);
            ViewData["ClassId"] = new SelectList(_context.Classes, "ClassId", "ClassName", milestone.ClassId);
            ViewData["IssueId"] = new SelectList(_context.Issues, "IssueId", "IssueName", milestone.IssueId);
            ViewData["ProjectId"] = new SelectList(_context.Projects, "ProjectId", "EnglishName", milestone.ProjectId);
            ViewData["SubjectId"] = new SelectList(_context.Subjects, "SubjectId", "SubjectName", milestone.SubjectId);
            return View(milestone);
        }

        // GET: Milestones/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.Milestones == null)
            {
                return NotFound();
            }

            var milestone = await _context.Milestones
                .Include(m => m.Assignment)
                .Include(m => m.Class)
                .Include(m => m.Issue)
                .Include(m => m.Project)
                .Include(m => m.Subject)
                .FirstOrDefaultAsync(m => m.MilestoneId == id);
            if (milestone == null)
            {
                return NotFound();
            }

            return View(milestone);
        }

        // POST: Milestones/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.Milestones == null)
            {
                return Problem("Entity set 'ImsSystemContext.Milestones'  is null.");
            }
            var milestone = await _context.Milestones.FindAsync(id);
            if (milestone != null)
            {
                _context.Milestones.Remove(milestone);
                _toastNotification.AddSuccessToastMessage("Delete successful!");
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction("Details", "Classes");
        }

        private bool MilestoneExists(int id)
        {
          return (_context.Milestones?.Any(e => e.MilestoneId == id)).GetValueOrDefault();
        }
    }
}
