using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using StudentTeacher.Models;

namespace StudentTeacher.Controllers
{
    public class LecturersController : Controller
    {
        private readonly XISD_POEContext _context;

        public LecturersController(XISD_POEContext context)
        {
            _context = context;
        }

        // GET: Lecturers
        public async Task<IActionResult> Index()
        {
            var xISD_POEContext = _context.Lecturers.Include(l => l.CampusNavigation).Include(l => l.EmailNavigation);
            return View(await xISD_POEContext.ToListAsync());
        }

        // GET: Lecturers/Details/5
        public async Task<IActionResult> Details(string id)
        {
            if (id == null || _context.Lecturers == null)
            {
                return NotFound();
            }

            var lecturer = await _context.Lecturers
                .Include(l => l.CampusNavigation)
                .Include(l => l.EmailNavigation)
                .FirstOrDefaultAsync(m => m.Number == id);
            if (lecturer == null)
            {
                return NotFound();
            }

            return View(lecturer);
        }

        // GET: Lecturers/Create
        public IActionResult Create()
        {
            ViewData["Campus"] = new SelectList(_context.Campuses, "Code", "Code");
            ViewData["Email"] = new SelectList(_context.Users, "Email", "Email");
            return View();
        }

        // POST: Lecturers/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Number,FirstName,LastName,Email,Campus")] Lecturer lecturer)
        {
            if (ModelState.IsValid)
            {
                _context.Add(lecturer);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["Campus"] = new SelectList(_context.Campuses, "Code", "Code", lecturer.Campus);
            ViewData["Email"] = new SelectList(_context.Users, "Email", "Email", lecturer.Email);
            return View(lecturer);
        }

        // GET: Lecturers/Edit/5
        public async Task<IActionResult> Edit(string id)
        {
            if (id == null || _context.Lecturers == null)
            {
                return NotFound();
            }

            var lecturer = await _context.Lecturers.FindAsync(id);
            if (lecturer == null)
            {
                return NotFound();
            }
            ViewData["Campus"] = new SelectList(_context.Campuses, "Code", "Code", lecturer.Campus);
            ViewData["Email"] = new SelectList(_context.Users, "Email", "Email", lecturer.Email);
            return View(lecturer);
        }

        // POST: Lecturers/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string id, [Bind("Number,FirstName,LastName,Email,Campus")] Lecturer lecturer)
        {
            if (id != lecturer.Number)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(lecturer);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!LecturerExists(lecturer.Number))
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
            ViewData["Campus"] = new SelectList(_context.Campuses, "Code", "Code", lecturer.Campus);
            ViewData["Email"] = new SelectList(_context.Users, "Email", "Email", lecturer.Email);
            return View(lecturer);
        }

        // GET: Lecturers/Delete/5
        public async Task<IActionResult> Delete(string id)
        {
            if (id == null || _context.Lecturers == null)
            {
                return NotFound();
            }

            var lecturer = await _context.Lecturers
                .Include(l => l.CampusNavigation)
                .Include(l => l.EmailNavigation)
                .FirstOrDefaultAsync(m => m.Number == id);
            if (lecturer == null)
            {
                return NotFound();
            }

            return View(lecturer);
        }

        // POST: Lecturers/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            if (_context.Lecturers == null)
            {
                return Problem("Entity set 'XISD_POEContext.Lecturers'  is null.");
            }
            var lecturer = await _context.Lecturers.FindAsync(id);
            if (lecturer != null)
            {
                _context.Lecturers.Remove(lecturer);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool LecturerExists(string id)
        {
          return _context.Lecturers.Any(e => e.Number == id);
        }
    }
}
