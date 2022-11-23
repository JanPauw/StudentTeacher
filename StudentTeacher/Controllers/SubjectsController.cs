using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DocumentFormat.OpenXml.Bibliography;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using StudentTeacher.Models;

namespace StudentTeacher.Controllers
{
    public class SubjectsController : Controller
    {
        private readonly XISD_POEContext _context;

        public SubjectsController(XISD_POEContext context)
        {
            _context = context;
        }

        // GET: Subjects
        public async Task<IActionResult> Index()
        {
            //get year from url
            string year = HttpContext.Request.Query["year"].ToString();

            if (String.IsNullOrWhiteSpace(year))
            {
                year = "1";
            }
            else if (!year.Equals("1") && !year.Equals("2") && !year.Equals("3") && !year.Equals("4"))
            {
                year = "1";
            }

            ViewBag.Year = year;

            //get year subjects
            List<Subject> subjects = await _context.Subjects.Where(x => x.YearOfStudy.Contains(year)).ToListAsync();
            ViewBag.Subjects = subjects; 
            return View(await _context.Subjects.ToListAsync());
        }

        // GET: Subjects/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.Subjects == null)
            {
                return NotFound();
            }

            var subject = await _context.Subjects
                .FirstOrDefaultAsync(m => m.Id == id);
            if (subject == null)
            {
                return NotFound();
            }

            return View(subject);
        }

        // GET: Subjects/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Subjects/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(string Subject, string Year)
        {
            Subject subject = new Subject();

            if (string.IsNullOrEmpty(Subject))
            {
                TempData["error"] = "Invalid Subject entered!";
                return RedirectToAction("Index", "Subjects");
            }

            if (string.IsNullOrEmpty(Year))
            {
                TempData["error"] = "Invalid year selected!";
                return RedirectToAction("Index", "Subjects");
            }

            try
            {
                int test = Convert.ToInt32(Year);
            }
            catch (Exception e)
            {
                TempData["error"] = "Invalid year selected!";
                return RedirectToAction("Index", "Subjects");
            }

            subject.Subject1 = Subject;
            subject.YearOfStudy = Year;
            subject.AmountOfClasses = 0;

            _context.Subjects.Add(subject);
            await _context.SaveChangesAsync();

            TempData["success"] = "Subject added successfully!";

            return RedirectToAction("Index", "Subjects", new { year = Year}); ;
        }

        // GET: Subjects/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.Subjects == null)
            {
                return NotFound();
            }

            var subject = await _context.Subjects.FindAsync(id);
            if (subject == null)
            {
                return NotFound();
            }
            return View(subject);
        }

        // POST: Subjects/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int SubjectID, string Subject)
        {
            Subject s = _context.Subjects.Find(SubjectID);
            if (s == null)
            {
                TempData["error"] = "Invalid Subject selected!";
                return RedirectToAction("Index");
            }

            if (String.IsNullOrEmpty(Subject))
            {
                TempData["error"] = "Invalid subject entered!";
                return RedirectToAction("Index", new { year = s.YearOfStudy });
            }

            s.Subject1 = Subject;

            _context.Subjects.Update(s);
            await _context.SaveChangesAsync();

            TempData["success"] = "Subject updated!";
            return RedirectToAction("Index", new { year = s.YearOfStudy });
        }

        // GET: Subjects/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.Subjects == null)
            {
                return NotFound();
            }

            var subject = await _context.Subjects
                .FirstOrDefaultAsync(m => m.Id == id);
            if (subject == null)
            {
                return NotFound();
            }

            var year = subject.YearOfStudy;

            _context.Subjects.Remove(subject);
            await _context.SaveChangesAsync();

            TempData["success"] = "Subject deleted!";
            return RedirectToAction("Index", new {year = year});
        }

        // POST: Subjects/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.Subjects == null)
            {
                return Problem("Entity set 'XISD_POEContext.Subjects'  is null.");
            }
            var subject = await _context.Subjects.FindAsync(id);
            if (subject != null)
            {
                _context.Subjects.Remove(subject);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool SubjectExists(int id)
        {
            return _context.Subjects.Any(e => e.Id == id);
        }
    }
}
