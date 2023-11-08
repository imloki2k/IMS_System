using IMS_System.Models.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using NToastNotify;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace IMS_System.Controllers
{
    [Authorize(Roles = "ADMIN,MANAGER")]
    public class AdminController : Controller
    {
        private readonly ImsSystemContext _context;

        private readonly IToastNotification _toastNotification;

        public AdminController(ImsSystemContext context, IToastNotification toastNotification)
        {
            _context = context;
            _toastNotification = toastNotification;
        }

        public IActionResult Index()
        {
            var lstUsers = _context.Users.ToList();
     
            return View("ManageUsers",lstUsers);
        }


        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.Users == null)
            {
                return NotFound();
            }

            var user = await _context.Users
                .FirstOrDefaultAsync(m => m.UserId == id);
            if (user == null)
            {
                return NotFound();
            }

            return View(user);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int? id)
        {
            if (id == null || _context.Users == null)
            {
                return NotFound();
            }
            var user = await _context.Users.FindAsync(id);
            _context.Users.Remove(user);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }


        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var user = await _context.Users.FindAsync(id);
            if (user == null)
            {
                return NotFound();
            }

            // Populate roles for the drop-down list
            ViewBag.Roles = new SelectList(_context.Roles, "RoleId", "RoleName", user.RoleId);
            return View(user);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("UserId,Name,Mobile,Email,RoleId")] User user, IFormFile avatarFile)
        {
            if (id != user.UserId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    // Handle the file upload
                    if (avatarFile != null && avatarFile.Length > 0)
                    {
                        var fileName = Path.GetFileName(avatarFile.FileName);
                        var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/images/avatars", fileName);

                        using (var stream = new FileStream(filePath, FileMode.Create))
                        {
                            await avatarFile.CopyToAsync(stream);
                        }

                        // Update the user's avatar file path
                        user.Avatar = fileName; // Or the path as needed
                    }

                    _context.Update(user);
                    await _context.SaveChangesAsync();
                    _toastNotification.AddSuccessToastMessage("Edit successfull!");
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!UserExists(user.UserId))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction("ManageUsers");
            }
            // Repopulate roles if edit fails
            ViewBag.Roles = new SelectList(_context.Roles, "RoleId", "RoleName", user.RoleId);
            return View(user);
        }


        private bool UserExists(int id)
        {
            return _context.Users.Any(e => e.UserId == id);
        }

        // Placeholder for password hashing method - replace with your actual hashing mechanism.
        private string SomeHashingFunction(string password)
        {
            // Implement password hashing logic here.
            return password; // This is not secure. Replace with real hashing.
        }


        /// <summary>
        /// Subject Management
        /// </summary>
        /// <returns></returns>
        public async Task<IActionResult> ManageSubjects()
        {
            var subjects = await _context.Subjects
                .Include(s => s.Status) // This will include the Status in the query
                .Include(s => s.Class)
                .ToListAsync();
            return View(subjects);
        }

        public async Task<IActionResult> CreateSubject()
        {
            // Populate the Status dropdown list
            ViewBag.Statuses = new SelectList(await _context.Statuses.ToListAsync(), "StatusId", "StatusName");
            // Populate the Class dropdown list
            ViewBag.Classes = new SelectList(await _context.Classes.ToListAsync(), "ClassId", "ClassName");

            return View("CreateSubject");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateSubject([Bind("SubjectName,SubjectCode,Description,StatusId,ClassId")] Subject subject)
        {
            if (ModelState.IsValid)
            {
                _context.Add(subject);
                await _context.SaveChangesAsync();
                _toastNotification.AddSuccessToastMessage("Subject created successfully.");
                return RedirectToAction(nameof(ManageSubjects));
            }
            return View(subject);
        }

        public async Task<IActionResult> EditSubject(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var subject = await _context.Subjects.FindAsync(id);
            if (subject == null)
            {
                return NotFound();
            }
            // Populate the Class dropdown list
            ViewBag.Classes = new SelectList(await _context.Classes.ToListAsync(), "ClassId", "ClassName", subject.ClassId);
            return View(subject);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditSubject(int id, [Bind("SubjectId,SubjectName,SubjectCode,Description,StatusId,ClassId")] Subject subject)
        {
            if (id != subject.SubjectId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                if (!SubjectExists(subject.SubjectId))
                {
                    return NotFound();
                }

                try
                {
                    _context.Update(subject);
                    await _context.SaveChangesAsync();
                    _toastNotification.AddSuccessToastMessage("Subject updated successfully.");
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!SubjectExists(subject.SubjectId))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(ManageSubjects));
            }

            return View(subject);
        }

        public async Task<IActionResult> DeleteSubject(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var subject = await _context.Subjects
                .FirstOrDefaultAsync(m => m.SubjectId == id);
            if (subject == null)
            {
                return NotFound();
            }

            return View(subject);
        }

        [HttpPost, ActionName("DeleteSubject")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteSubjectConfirmed(int id)
        {
            var subject = await _context.Subjects.FindAsync(id);
            if (subject != null)
            {
                _context.Subjects.Remove(subject);
                await _context.SaveChangesAsync();
                // Add a success notification
                _toastNotification.AddSuccessToastMessage("Subject deleted successfully.");
            }
            return RedirectToAction(nameof(ManageSubjects));
        }

        [HttpPost]
        public async Task<IActionResult> ChangeSubjectStatus(int id, int statusId)
        {
            var subject = await _context.Subjects.FindAsync(id);
            if (subject == null)
            {
                return NotFound();
            }

            var status = await _context.Statuses.FindAsync(statusId);
            if (status == null)
            {
                return NotFound("Status not found.");
            }

            subject.StatusId = statusId;
            try
            {
                _context.Update(subject);
                await _context.SaveChangesAsync();
                _toastNotification.AddSuccessToastMessage($"Subject status has been updated.");
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!SubjectExists(subject.SubjectId))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return RedirectToAction(nameof(ManageSubjects));
        }


        private bool SubjectExists(int id)
        {
            return _context.Subjects.Any(e => e.SubjectId == id);
        }

        public async Task<IActionResult> DetailsSubject(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var subject = await _context.Subjects
                .Include(s => s.Class) // Include the Class navigation property if it's needed
                .FirstOrDefaultAsync(m => m.SubjectId == id);
            if (subject == null)
            {
                return NotFound();
            }

            return View(subject);
        }



    }
}
