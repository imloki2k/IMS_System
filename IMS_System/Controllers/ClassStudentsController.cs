using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using IMS_System.Models.Entities;

namespace IMS_System.Controllers
{
    public class ClassStudentsController : Controller
    {
        private readonly ImsSystemContext _context;

        public ClassStudentsController(ImsSystemContext context)
        {
            _context = context;
        }

        // GET: ClassStudents
        public async Task<IActionResult> Index()
        {
            var imsSystemContext = _context.ClassStudents.Include(c => c.Class).Include(c => c.Student);
            return View(await imsSystemContext.ToListAsync());
        }

        // GET: ClassStudents/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.ClassStudents == null)
            {
                return NotFound();
            }

            var classStudent = await _context.ClassStudents
                .Include(c => c.Class)
                .Include(c => c.Student)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (classStudent == null)
            {
                return NotFound();
            }

            return View(classStudent);
        }

        // GET: ClassStudents/Create
        public IActionResult Create()
        {
            ViewData["ClassId"] = new SelectList(_context.Classes, "ClassId", "ClassId");
            ViewData["StudentId"] = new SelectList(_context.Users, "UserId", "UserId");
            return View();
        }

        // POST: ClassStudents/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,ClassId,StudentId")] ClassStudent classStudent)
        {
            if (ModelState.IsValid)
            {
                _context.Add(classStudent);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["ClassId"] = new SelectList(_context.Classes, "ClassId", "ClassId", classStudent.ClassId);
            ViewData["StudentId"] = new SelectList(_context.Users, "UserId", "UserId", classStudent.StudentId);
            return View(classStudent);
        }

        // GET: ClassStudents/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.ClassStudents == null)
            {
                return NotFound();
            }

            var classStudent = await _context.ClassStudents.FindAsync(id);
            if (classStudent == null)
            {
                return NotFound();
            }
            ViewData["ClassId"] = new SelectList(_context.Classes, "ClassId", "ClassId", classStudent.ClassId);
            ViewData["StudentId"] = new SelectList(_context.Users, "UserId", "UserId", classStudent.StudentId);
            return View(classStudent);
        }

        // POST: ClassStudents/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,ClassId,StudentId")] ClassStudent classStudent)
        {
            if (id != classStudent.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(classStudent);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ClassStudentExists(classStudent.Id))
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
            ViewData["ClassId"] = new SelectList(_context.Classes, "ClassId", "ClassId", classStudent.ClassId);
            ViewData["StudentId"] = new SelectList(_context.Users, "UserId", "UserId", classStudent.StudentId);
            return View(classStudent);
        }

        // GET: ClassStudents/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.ClassStudents == null)
            {
                return NotFound();
            }

            var classStudent = await _context.ClassStudents
                .Include(c => c.Class)
                .Include(c => c.Student)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (classStudent == null)
            {
                return NotFound();
            }

            return View(classStudent);
        }

        // POST: ClassStudents/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.ClassStudents == null)
            {
                return Problem("Entity set 'ImsSystemContext.ClassStudents'  is null.");
            }
            var classStudent = await _context.ClassStudents.FindAsync(id);
            if (classStudent != null)
            {
                _context.ClassStudents.Remove(classStudent);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ClassStudentExists(int id)
        {
          return (_context.ClassStudents?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
