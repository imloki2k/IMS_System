using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using IMS_System.Models.Entities;
using Microsoft.AspNetCore.Authorization;
using IMS_System.Extentions;
using NToastNotify;

namespace IMS_System.Controllers
{
    [Authorize]
    public class ClassesController : Controller
    {
        private readonly ImsSystemContext _context;
        private readonly IToastNotification _toastNotification;
        public ClassesController(ImsSystemContext context, IToastNotification toastNotification)
        {
            _context = context;
            _toastNotification = toastNotification;
        }

        
        public async Task<IActionResult> Index()
        {
            var imsSystemContext = _context.Classes.Include(x => x.Semeter).Include(x => x.Status);
            return View(await imsSystemContext.ToListAsync());
        }

        
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

        
        public IActionResult Create()
        {
            var status = _context.Statuses.Where(x => x.StatusId.ToString().Equals("1") || x.StatusId.ToString().Equals("2"));
            ViewData["SemeterId"] = new SelectList(_context.Semeters, "SemeterId", "SemeterName");
            ViewData["StatusId"] = new SelectList(status, "StatusId", "StatusName");
            return View();
        }

        
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("ClassId,Title,ClassName,Description,StartDate,EndDate,StatusId,SemeterId")] Class @class)
        {
            if (ModelState.IsValid)
            {
                _context.Add(@class);
                await _context.SaveChangesAsync();
                _toastNotification.AddSuccessToastMessage("Create successful!");
                return RedirectToAction(nameof(Index));
            }
            else
            {
                _toastNotification.AddErrorToastMessage("Create failed!");
            }
            ViewData["SemeterId"] = new SelectList(_context.Semeters, "SemeterId", "SemeterName", @class.SemeterId);
            ViewData["StatusId"] = new SelectList(_context.Statuses.Where(x => x.StatusId.ToString().Equals("1") || x.StatusId.ToString().Equals("2")), "StatusId", "StatusName", @class.StatusId);
            return View(@class);
        }

       
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
                    _toastNotification.AddSuccessToastMessage("Update successful!");
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ClassExists(@class.ClassId))
                    {
                        _toastNotification.AddErrorToastMessage("Update failed!");
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

        public async Task<IActionResult> Cancel(int? id)
        {
            if (id == null || _context.Classes == null)
            {
                return NotFound();
            }

            var @class = await _context.Classes.FindAsync(id);
            try
            {
                @class.StatusId = 2;
                _context.Update(@class);
                await _context.SaveChangesAsync();
                _toastNotification.AddSuccessToastMessage("Cancel started class successful!");
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
                _toastNotification.AddSuccessToastMessage("Delete successful!");
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
