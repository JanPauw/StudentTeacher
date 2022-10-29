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
    public class StudentModulesController : Controller
    {
        private readonly XISD_POEContext _context;

        public StudentModulesController(XISD_POEContext context)
        {
            _context = context;
        }

        // GET: StudentModules
        public async Task<IActionResult> Index()
        {
            var xISD_POEContext = _context.StudentModules.Include(s => s.ModuleNavigation).Include(s => s.StudentNavigation);
            return View(await xISD_POEContext.ToListAsync());
        }

        // GET: StudentModules/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.StudentModules == null)
            {
                return NotFound();
            }

            var studentModule = await _context.StudentModules
                .Include(s => s.ModuleNavigation)
                .Include(s => s.StudentNavigation)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (studentModule == null)
            {
                return NotFound();
            }

            return View(studentModule);
        }

        // GET: StudentModules/Create
        public IActionResult Create()
        {
            ViewData["Module"] = new SelectList(_context.Modules, "Number", "Number");
            ViewData["Student"] = new SelectList(_context.Students, "Number", "Number");
            return View();
        }

        // POST: StudentModules/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Student,Module")] StudentModule studentModule)
        {
            if (ModelState.IsValid)
            {
                _context.Add(studentModule);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["Module"] = new SelectList(_context.Modules, "Number", "Number", studentModule.Module);
            ViewData["Student"] = new SelectList(_context.Students, "Number", "Number", studentModule.Student);
            return View(studentModule);
        }

        // POST: StudentModules/Assign
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Assign(string StudentNumber, string SelectedModule)
        {
            #region Check for Null Inputs
            if (string.IsNullOrWhiteSpace(StudentNumber))
            {
                TempData["error"] = "Invalid Student Selected!";
                return RedirectToAction("Details", "Student", new { id = StudentNumber });
            }

            if (string.IsNullOrWhiteSpace(SelectedModule))
            {
                TempData["error"] = "Invalid Module Selected!";
                return RedirectToAction("Details", "Student", new { id = StudentNumber });
            }
            #endregion

            #region Check for Valid Inputs
            Student student = _context.Students.Find(StudentNumber);
            if (student == null)
            {
                TempData["error"] = "Invalid Student Selected!";
                return RedirectToAction("Details", "Student", new { id = StudentNumber });
            }

            Module module = _context.Modules.Find(SelectedModule);
            if (module == null)
            {
                TempData["error"] = "Invalid Module Selected!";
                return RedirectToAction("Details", "Student", new { id = StudentNumber });
            }
            #endregion

            StudentModule sm = new StudentModule();
            sm.Student = student.Number;
            sm.Module = module.Number;
            sm.StudentNavigation = student;
            sm.ModuleNavigation = module;

            _context.StudentModules.Add(sm);
            await _context.SaveChangesAsync();
            TempData["success"] = "Module Assigned Successfully!";
            return RedirectToAction("Details", "Students", new { id = StudentNumber });
        }

        // GET: StudentModules/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.StudentModules == null)
            {
                return NotFound();
            }

            var studentModule = await _context.StudentModules.FindAsync(id);
            if (studentModule == null)
            {
                return NotFound();
            }
            ViewData["Module"] = new SelectList(_context.Modules, "Number", "Number", studentModule.Module);
            ViewData["Student"] = new SelectList(_context.Students, "Number", "Number", studentModule.Student);
            return View(studentModule);
        }

        // POST: StudentModules/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Student,Module")] StudentModule studentModule)
        {
            if (id != studentModule.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(studentModule);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!StudentModuleExists(studentModule.Id))
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
            ViewData["Module"] = new SelectList(_context.Modules, "Number", "Number", studentModule.Module);
            ViewData["Student"] = new SelectList(_context.Students, "Number", "Number", studentModule.Student);
            return View(studentModule);
        }

        // GET: StudentModules/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.StudentModules == null)
            {
                return NotFound();
            }

            var studentModule = await _context.StudentModules
                .Include(s => s.ModuleNavigation)
                .Include(s => s.StudentNavigation)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (studentModule == null)
            {
                return NotFound();
            }

            return View(studentModule);
        }

        // POST: StudentModules/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.StudentModules == null)
            {
                return Problem("Entity set 'XISD_POEContext.StudentModules'  is null.");
            }
            var studentModule = await _context.StudentModules.FindAsync(id);
            if (studentModule != null)
            {
                _context.StudentModules.Remove(studentModule);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool StudentModuleExists(int id)
        {
            return _context.StudentModules.Any(e => e.Id == id);
        }
    }
}
