using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using IMS_System.Models.Entities;
using ClosedXML.Excel;
using DocumentFormat.OpenXml.Spreadsheet;
using System.Globalization;

namespace IMS_System.Controllers
{
    public class ProjectIssuesController : Controller
    {
        private readonly ImsSystemContext _context;

        public ProjectIssuesController(ImsSystemContext context)
        {
            _context = context;
        }
        // GET: ProjectIssues
        public async Task<IActionResult> Index(int? projectId, int? milestoneId)
        {
            if (!projectId.HasValue)
            {
                // Handle the case where no projectId is provided
                return View(new List<Issue>());
            }

            IQueryable<Issue> query = _context.Issues
                                              .Include(i => i.Status)
                                              // Assuming Issues has a navigation property to Milestones
                                              .Include(i => i.Milestones)
                                              .Where(i => i.ProjectId == projectId.Value);

            if (milestoneId.HasValue)
            {
                // Further filter the issues to only include those associated with the specified milestone
                // This assumes that there's a many-to-many or one-to-many relationship between Issues and Milestones
                // Adjust the query according to your data model
                query = query.Where(i => i.Milestones.Any(m => m.MilestoneId == milestoneId.Value));
            }

            var issues = await query.ToListAsync();
            return View(issues);
        }



        // GET: ProjectIssues/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var issue = await _context.Issues
                .Include(i => i.Project) // Include the Project entity
                .Include(i => i.Status) // Include the Status entity
                .FirstOrDefaultAsync(m => m.IssueId == id);

            if (issue == null)
            {
                return NotFound();
            }

            return View(issue);
        }


        public IActionResult Create(int projectId)
        {
            // Fetch the milestones related to the projectId.
            ViewBag.Milestones = new SelectList(_context.Milestones
                                                        .Where(m => m.ProjectId == projectId)
                                                        .ToList(), "MilestoneId", "Name");

            // Assuming Statuses are not related to the projectId and are the same for all projects.
            ViewBag.Statuses = new SelectList(_context.Statuses.ToList(), "StatusId", "StatusName");

            // Create a new issue with the projectId.
            var issue = new Issue
            {
                ProjectId = projectId
            };

            return View(issue);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("IssueName, ProjectId, MilestoneId, CreatedDate, IssueDescription, StatusId")] Issue issue)
        {
            if (ModelState.IsValid)
            {
                _context.Add(issue);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index), new { projectId = issue.ProjectId });
            }

            // Reload the necessary data for the view
            ViewBag.Milestones = new SelectList(_context.Milestones.Where(m => m.ProjectId == issue.ProjectId).ToList(), "MilestoneId", "Name", issue.MilestoneId);
            ViewBag.Statuses = new SelectList(_context.Statuses.ToList(), "StatusId", "StatusName", issue.StatusId);

            return View(issue);
        }

        // GET: ProjectIssues/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var issue = await _context.Issues.FindAsync(id);
            if (issue == null)
            {
                return NotFound();
            }
            ViewData["ProjectId"] = new SelectList(_context.Projects, "ProjectId", "EnglishName", issue.ProjectId);
            ViewData["MilestoneId"] = new SelectList(_context.Milestones, "MilestoneId", "MilestoneDescription", issue.MilestoneId);
            ViewData["StatusId"] = new SelectList(_context.Statuses, "StatusId", "StatusName", issue.StatusId);
            return View(issue);
        }


        // POST: ProjectIssues/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("IssueId,IssueName,ProjectId,MilestoneId,CreatedDate,IssueDescription,StatusId")] Issue issue)
        {
            if (id != issue.IssueId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(issue);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!IssueExists(issue.IssueId))
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
            ViewData["StatusId"] = new SelectList(_context.Statuses, "StatusId", "StatusId", issue.StatusId);
            return View(issue);
        }

        // GET: ProjectIssues/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.Issues == null)
            {
                return NotFound();
            }

            var issue = await _context.Issues
                .Include(i => i.Status)
                .FirstOrDefaultAsync(m => m.IssueId == id);
            if (issue == null)
            {
                return NotFound();
            }

            return View(issue);
        }

        // POST: ProjectIssues/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.Issues == null)
            {
                return Problem("Entity set 'ImsSystemContext.Issues'  is null.");
            }
            var issue = await _context.Issues.FindAsync(id);
            if (issue != null)
            {
                _context.Issues.Remove(issue);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool IssueExists(int id)
        {
            return (_context.Issues?.Any(e => e.IssueId == id)).GetValueOrDefault();
        }

        public async Task<IActionResult> Export()
        {
            using (var workbook = new XLWorkbook())
            {
                var worksheet = workbook.Worksheets.Add("Issues");
                var currentRow = 1;
                worksheet.Cell(currentRow, 1).Value = "Issue Name";
                worksheet.Cell(currentRow, 2).Value = "Created Date";
                worksheet.Cell(currentRow, 3).Value = "Status";
                worksheet.Cell(currentRow, 4).Value = "Issue Description";
                worksheet.Cell(currentRow, 5).Value = "Milestone";

                var issues = await _context.Issues
                    .Include(i => i.Status)
                    .Include(i => i.Milestones) // Assuming you have a navigation property for Milestones in the Issue entity
                    .ToListAsync();

                foreach (var issue in issues)
                {
                    currentRow++;
                    worksheet.Cell(currentRow, 1).Value = issue.IssueName;
                    worksheet.Cell(currentRow, 2).Value = issue.CreatedDate?.ToString("dd/MM/yyyy HH:mm"); // Format date time as you prefer
                    worksheet.Cell(currentRow, 3).Value = issue.Status?.StatusName;
                    worksheet.Cell(currentRow, 4).Value = issue.IssueDescription;
                    worksheet.Cell(currentRow, 5).Value = issue.Milestones.FirstOrDefault()?.MilestoneDescription; // Gets the first related milestone description, if any
                }

                using (var stream = new MemoryStream())
                {
                    workbook.SaveAs(stream);
                    var content = stream.ToArray();

                    return File(
                        content,
                        "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                        "issues.xlsx");
                }
            }
        }

    }
}
