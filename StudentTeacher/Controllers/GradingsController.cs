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
    public class GradingsController : Controller
    {
        private readonly XISD_POEContext _context;

        public GradingsController(XISD_POEContext context)
        {
            _context = context;
        }

        // GET: Gradings
        public async Task<IActionResult> Index()
        {
            var xISD_POEContext = _context.Gradings.Include(g => g.StudentNavigation).Include(g => g.TeacherNavigation);
            return View(await xISD_POEContext.ToListAsync());
        }

        // GET: Gradings/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.Gradings == null)
            {
                return NotFound();
            }

            var grading = await _context.Gradings
                .Include(g => g.StudentNavigation)
                .Include(g => g.TeacherNavigation)
                .FirstOrDefaultAsync(m => m.Number == id);
            if (grading == null)
            {
                return NotFound();
            }

            return View(grading);
        }

        // GET: Gradings/Create
        public IActionResult Create()
        {
            ViewData["Student"] = new SelectList(_context.Students, "Number", "Number");
            ViewData["Teacher"] = new SelectList(_context.Teachers, "Number", "Number");
            return View();
        }

        // POST: Gradings/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Number,Student,Teacher")] Grading grading)
        {
            if (ModelState.IsValid)
            {
                _context.Add(grading);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["Student"] = new SelectList(_context.Students, "Number", "Number", grading.Student);
            ViewData["Teacher"] = new SelectList(_context.Teachers, "Number", "Number", grading.Teacher);
            return View(grading);
        }

        // GET: Gradings/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.Gradings == null)
            {
                return NotFound();
            }

            var grading = await _context.Gradings.FindAsync(id);
            if (grading == null)
            {
                return NotFound();
            }
            ViewData["Student"] = new SelectList(_context.Students, "Number", "Number", grading.Student);
            ViewData["Teacher"] = new SelectList(_context.Teachers, "Number", "Number", grading.Teacher);
            return View(grading);
        }

        // POST: Gradings/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Number,Student,Teacher")] Grading grading)
        {
            if (id != grading.Number)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(grading);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!GradingExists(grading.Number))
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
            ViewData["Student"] = new SelectList(_context.Students, "Number", "Number", grading.Student);
            ViewData["Teacher"] = new SelectList(_context.Teachers, "Number", "Number", grading.Teacher);
            return View(grading);
        }

        // GET: Gradings/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.Gradings == null)
            {
                return NotFound();
            }

            var grading = await _context.Gradings
                .Include(g => g.StudentNavigation)
                .Include(g => g.TeacherNavigation)
                .FirstOrDefaultAsync(m => m.Number == id);
            if (grading == null)
            {
                return NotFound();
            }

            return View(grading);
        }

        // POST: Gradings/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.Gradings == null)
            {
                return Problem("Entity set 'XISD_POEContext.Gradings'  is null.");
            }
            var grading = await _context.Gradings.FindAsync(id);
            if (grading != null)
            {
                _context.Gradings.Remove(grading);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool GradingExists(int id)
        {
          return _context.Gradings.Any(e => e.Number == id);
        }
    }
}
