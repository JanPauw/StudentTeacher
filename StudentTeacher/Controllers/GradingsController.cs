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
            return View();
        }

        // POST: Gradings/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(
            string SectionAtoD, string SectionE, string Intro, string Teaching, string Closure,
            string Assessment, string Presence, string Environment,
            string ComSectionAtoD, string ComSectionE, string ComIntro, string ComTeaching, string ComClosure,
            string ComAssessment, string ComPresence, string ComEnvironment,
            string StudentCode, string TeacherCode)
        {
            #region Check that Student and Teacher is not null
            var student = _context.Students.Find(StudentCode);
            var teacher = _context.Teachers.Find(TeacherCode);

            if (student == null)
            {
                TempData["error"] = "Invalid Student selected!";
                return View();
            }

            if (teacher == null)
            {
                TempData["error"] = "Invalid Teacher selected!";
                return View();
            }
            #endregion

            #region Check if Radios are not null
            if (SectionAtoD.ToString() == null)
            {
                TempData["error"] = "Invalid rating selected!";
                return View();
            }
            if (SectionE.ToString() == null)
            {
                TempData["error"] = "Invalid rating selected!";
                return View();
            }
            if (Intro.ToString() == null)
            {
                TempData["error"] = "Invalid rating selected!";
                return View();
            }
            if (Teaching.ToString() == null)
            {
                TempData["error"] = "Invalid rating selected!";
                return View();
            }
            if (Closure.ToString() == null)
            {
                TempData["error"] = "Invalid rating selected!";
                return View();
            }
            if (Assessment.ToString() == null)
            {
                TempData["error"] = "Invalid rating selected!";
                return View();
            }
            if (Presence.ToString() == null)
            {
                TempData["error"] = "Invalid rating selected!";
                return View();
            }
            if (Environment.ToString() == null)
            {
                TempData["error"] = "Invalid rating selected!";
                return View();
            }

            #endregion

            #region Check if Commentary is not null
            if (string.IsNullOrWhiteSpace(ComSectionAtoD))
            {
                TempData["error"] = "Invalid Commentary!";
                return View();
            }

            if (string.IsNullOrWhiteSpace(ComSectionE))
            {
                TempData["error"] = "Invalid Commentary!";
                return View();
            }

            if (string.IsNullOrWhiteSpace(ComIntro))
            {
                TempData["error"] = "Invalid Commentary!";
                return View();
            }

            if (string.IsNullOrWhiteSpace(ComTeaching))
            {
                TempData["error"] = "Invalid Commentary!";
                return View();
            }

            if (string.IsNullOrWhiteSpace(ComClosure))
            {
                TempData["error"] = "Invalid Commentary!";
                return View();
            }

            if (string.IsNullOrWhiteSpace(ComAssessment))
            {
                TempData["error"] = "Invalid Commentary!";
                return View();
            }

            if (string.IsNullOrWhiteSpace(ComPresence))
            {
                TempData["error"] = "Invalid Commentary!";
                return View();
            }

            if (string.IsNullOrWhiteSpace(ComEnvironment))
            {
                TempData["error"] = "Invalid Commentary!";
                return View();
            }
            #endregion

            //Create new Grading
            Grading grading = new Grading();
            grading.Student = StudentCode;
            grading.Teacher = TeacherCode;
            grading.StudentNavigation = student;
            grading.TeacherNavigation = teacher;

            _context.Gradings.Add(grading);
            await _context.SaveChangesAsync();

            #region Create Grading Marks
            //Planning Section
            Planning planning = new Planning();

            planning.SectionAtoD = int.Parse(SectionAtoD);
            planning.SectionE = int.Parse(SectionE);

            planning.GradingNumber = grading.Number;
            planning.GradingNumberNavigation = grading;

            _context.Plannings.Add(planning);
            await _context.SaveChangesAsync();

            //Execution Section
            Execution execution = new Execution();

            execution.Intro = int.Parse(Intro);
            execution.Teaching = int.Parse(Teaching);
            execution.Closure = int.Parse(Closure);
            execution.Assessment = int.Parse(Assessment);

            execution.GradingNumber = grading.Number;
            execution.GradingNumberNavigation = grading;

            _context.Executions.Add(execution);
            await _context.SaveChangesAsync();

            //Overall Section
            Overall overall = new Overall();

            overall.Presence = int.Parse(Presence);
            overall.Environment = int.Parse(Environment);

            overall.GradingNumber = grading.Number;
            overall.GradingNumberNavigation = grading;

            _context.Overalls.Add(overall);
            await _context.SaveChangesAsync();
            #endregion

            #region Create Grading Commentaries
            //Planning Section
            Commentary sectionAtoD = new Commentary();
            Commentary sectionE = new Commentary();

            sectionAtoD.Criteria = "SectionAtoD";
            sectionE.Criteria = "SectionE";

            sectionAtoD.Comment = "" + ComSectionAtoD;
            sectionE.Comment = "" + ComSectionE;

            sectionAtoD.GradingNumber = grading.Number;
            sectionE.GradingNumber = grading.Number;

            sectionAtoD.GradingNumberNavigation = grading;
            sectionE.GradingNumberNavigation = grading;

            _context.Commentaries.Add(sectionAtoD);
            _context.Commentaries.Add(sectionE);

            await _context.SaveChangesAsync();

            //Execution Section
            Commentary intro = new Commentary();
            Commentary teaching = new Commentary();
            Commentary closure = new Commentary();
            Commentary assessment = new Commentary();

            intro.Criteria = "Intro";
            teaching.Criteria = "Teaching";
            closure.Criteria = "Closure";
            assessment.Criteria = "Assessment";

            intro.Comment = "" + ComIntro;
            teaching.Comment = "" + ComTeaching;
            closure.Comment = "" + ComClosure;
            assessment.Comment = "" + ComAssessment;

            intro.GradingNumber = grading.Number;
            teaching.GradingNumber = grading.Number;
            closure.GradingNumber = grading.Number;
            assessment.GradingNumber = grading.Number;

            intro.GradingNumberNavigation = grading;
            teaching.GradingNumberNavigation = grading;
            closure.GradingNumberNavigation = grading;
            assessment.GradingNumberNavigation = grading;

            _context.Commentaries.Add(intro);
            _context.Commentaries.Add(teaching);
            _context.Commentaries.Add(closure);
            _context.Commentaries.Add(assessment);

            await _context.SaveChangesAsync();

            //Overall Section
            Commentary presence = new Commentary();
            Commentary environment = new Commentary();

            presence.Criteria = "Presence";
            environment.Criteria = "Environment";

            presence.Comment = "" + ComPresence;
            environment.Comment = "" + ComEnvironment;

            presence.GradingNumber = grading.Number;
            environment.GradingNumber = grading.Number;

            presence.GradingNumberNavigation = grading;
            environment.GradingNumberNavigation = grading;

            _context.Commentaries.Add(presence);
            _context.Commentaries.Add(environment);

            await _context.SaveChangesAsync();
            #endregion

            TempData["success"] = "Grading added Successfully!";
            return RedirectToAction("Details", "Students", new { id = student.Number });
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
