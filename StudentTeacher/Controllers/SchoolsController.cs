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
    public class SchoolsController : Controller
    {
        private readonly XISD_POEContext _context;

        public SchoolsController(XISD_POEContext context)
        {
            _context = context;
        }

        // GET: Schools
        public async Task<IActionResult> Index()
        {
            var xISD_POEContext = _context.Schools.Include(s => s.CampusNavigation);
            return View(await xISD_POEContext.ToListAsync());
        }

        // GET: Schools/Details/5
        public async Task<IActionResult> Details(string id)
        {
            if (id == null || _context.Schools == null)
            {
                return NotFound();
            }

            var school = await _context.Schools
                .Include(s => s.CampusNavigation)
                .FirstOrDefaultAsync(m => m.Code == id);
            if (school == null)
            {
                return NotFound();
            }

            return View(school);
        }

        // GET: Schools/Create
        public IActionResult Create()
        {
            ViewData["Campus"] = new SelectList(_context.Campuses, "Code", "Code");
            ViewBag.ListCampuses = _context.Campuses.ToList();
            return View();
        }

        // POST: Schools/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(string Name, string Quintile, string Campus)
        {
            //List of Campuses in case of Page Reload on Post
            ViewBag.ListCampuses = _context.Campuses.ToList();

            #region Input validation

            //check if name is empty
            if (String.IsNullOrEmpty(Name) || Name.Length < 4)
            {
                TempData["error"] = "Invalid Name entered!";
                return View();
            }

            //check if quintile is empty
            if (String.IsNullOrEmpty(Quintile))
            {
                TempData["error"] = "Invalid Quintile entered!";
                return View();
            }

            //check if campus is empty
            var campus = _context.Campuses.Find(Campus);
            if (campus == null)
            {
                TempData["error"] = "Invalid Campus selected!";
                return View();
            }
            #endregion

            School s = new School();
            s.Name = Name;
            s.Campus = Campus;
            s.Quintile = Quintile;
            s.Code = GenerateSchoolCode(Name);

            try
            {
                _context.Add(s);
                await _context.SaveChangesAsync();
                TempData["success"] = "School added Successfully!";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception e)
            {
                ViewData["Campus"] = new SelectList(_context.Campuses, "Code", "Code", s.Campus);
                return View(s);
            }
        }

        // GET: Schools/Edit/5
        public async Task<IActionResult> Edit(string id)
        {
            if (id == null || _context.Schools == null)
            {
                return NotFound();
            }

            var school = await _context.Schools.FindAsync(id);
            if (school == null)
            {
                return NotFound();
            }

            //populate user input fields
            string name = school.Name;
            string quintile = school.Quintile;

            ViewBag.Name = name;
            ViewBag.Quintile = quintile;

            ViewData["Campus"] = new SelectList(_context.Campuses, "Code", "Code");
            ViewBag.ListCampuses = _context.Campuses.ToList();
            return View(school);
        }

        // POST: Schools/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string id, string Name, string Quintile, string Campus)
        {
            School s = new School();

            //find school by id (schoolCode)
            s = _context.Schools.Find(id);

            if (id == null)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    //update school info
                    s.Name = Name;
                    s.Quintile = Quintile;
                    s.Campus = Campus;

                    //save changes to DB
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!SchoolExists(s.Code))
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
            ViewData["Campus"] = new SelectList(_context.Campuses, "Code", "Code", s.Campus);
            return View(s);
        }

        // GET: Schools/Delete/5
        public async Task<IActionResult> Delete(string id)
        {
            if (id == null || _context.Schools == null)
            {
                return NotFound();
            }

            var school = await _context.Schools
                .Include(s => s.CampusNavigation)
                .FirstOrDefaultAsync(m => m.Code == id);
            if (school == null)
            {
                return NotFound();
            }

            return View(school);
        }

        // POST: Schools/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            if (_context.Schools == null)
            {
                return Problem("Entity set 'XISD_POEContext.Schools'  is null.");
            }
            var school = await _context.Schools.FindAsync(id);
            if (school != null)
            {
                _context.Schools.Remove(school);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool SchoolExists(string id)
        {
            return _context.Schools.Any(e => e.Code == id);
        }


        //School Code Generator
        private string GenerateSchoolCode(string Name)
        {
            string toReturn = "";

            Name = Name.ToUpper();

            toReturn += Name.Substring(0, 4);

            Random rnd = new Random();
            int RandomNumber = rnd.Next(1000, 10000);

            toReturn += RandomNumber.ToString();

            if (!SchoolExists(toReturn))
            {
                return toReturn;
            }
            else
            {
                return GenerateSchoolCode(Name);
            }
        }


    }
}
