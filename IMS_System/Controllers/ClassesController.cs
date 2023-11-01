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
    public class ClassesController : Controller
    {
        private readonly ImsSystemContext _context;

        public ClassesController(ImsSystemContext context)
        {
            _context = context;
        }

        // GET: Classes
        public async Task<IActionResult> Index()
        {
            var imsSystemContext = _context.Classes.Include(x => x.Semeter).Include(x => x.Status);
            return View(await imsSystemContext.ToListAsync());
        }

        // GET: Classes/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.Classes == null)
            {
                return NotFound();
            }

            var @class = await _context.Classes
                .Include(x => x.Semeter)
                .Include(x => x.Status)
                .FirstOrDefaultAsync(m => m.ClassId == id);
            if (@class == null)
            {
                return NotFound();
            }

            return View(@class);
        }

        // GET: Classes/Create
        public IActionResult Create()
        {
            var status = _context.Statuses.Where(x => x.StatusId.ToString().Equals("1") || x.StatusId.ToString().Equals("2"));
            ViewData["SemeterId"] = new SelectList(_context.Semeters, "SemeterId", "SemeterName");
            ViewData["StatusId"] = new SelectList(status, "StatusId", "StatusName");
            return View();
        }

        // POST: Classes/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("ClassId,Title,ClassName,Description,StartDate,EndDate,StatusId,SemeterId")] Class @class)
        {
            if (ModelState.IsValid)
            {
                _context.Add(@class);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["SemeterId"] = new SelectList(_context.Semeters, "SemeterId", "SemeterName", @class.SemeterId);
            ViewData["StatusId"] = new SelectList(_context.Statuses.Where(x => x.StatusId.ToString().Equals("1") || x.StatusId.ToString().Equals("2")), "StatusId", "StatusName", @class.StatusId);
            return View(@class);
        }

        // GET: Classes/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.Classes == null)
            {
                return NotFound();
            }

            var @class = await _context.Classes.FindAsync(id);
            if (@class == null)
            {
                return NotFound();
            }
            ViewData["SemeterId"] = new SelectList(_context.Semeters, "SemeterId", "SemeterName", @class.SemeterId);
            ViewData["StatusId"] = new SelectList(_context.Statuses.Where(x => x.StatusId.ToString().Equals("1") || x.StatusId.ToString().Equals("2")), "StatusId", "StatusName", @class.StatusId);
            return View(@class);
        }

        // POST: Classes/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("ClassId,Title,Description,StartDate,EndDate,ClassName,StatusId,SemeterId")] Class @class)
        {
            if (id != @class.ClassId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(@class);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ClassExists(@class.ClassId))
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
            ViewData["SemeterId"] = new SelectList(_context.Semeters, "SemeterId", "SemeterName", @class.SemeterId);
            ViewData["StatusId"] = new SelectList(_context.Statuses.Where(x => x.StatusId.ToString().Equals("1") || x.StatusId.ToString().Equals("2")), "StatusId", "StatusName", @class.StatusId);
            return View(@class);
        }

        // GET: Classes/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.Classes == null)
            {
                return NotFound();
            }

            var @class = await _context.Classes
                .Include(x => x.Semeter)
                .Include(x => x.Status)
                .FirstOrDefaultAsync(m => m.ClassId == id);
            if (@class == null)
            {
                return NotFound();
            }

            return View(@class);
        }

        // POST: Classes/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.Classes == null)
            {
                return Problem("Entity set 'ImsSystemContext.Classes'  is null.");
            }
            var @class = await _context.Classes.FindAsync(id);
            if (@class != null)
            {
                _context.Classes.Remove(@class);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ClassExists(int id)
        {
          return (_context.Classes?.Any(e => e.ClassId == id)).GetValueOrDefault();
        }
    }
}
