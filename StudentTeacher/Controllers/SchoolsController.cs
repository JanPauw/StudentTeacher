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
            return View();
        }

        // POST: Schools/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(string Name, string Province, string City, string Campus)
        {
            #region Input validation

            //check if name is empty
            if (String.IsNullOrEmpty(Name))
            {
                TempData["error"] = "Invalid Name entered!";
                return View();
            }

            //check if province is empty
            if (String.IsNullOrEmpty(Province))
            {
                TempData["error"] = "Invalid Province entered!";
                return View();
            }

            //check if city is empty
            if (String.IsNullOrEmpty(City))
            {
                TempData["error"] = "Invalid City entered!";
                return View();
            }

            //check if campus is empty
            if (String.IsNullOrEmpty(Campus))
            {
                TempData["error"] = "Invalid Campus entered!";
                return View();
            }
            #endregion

            School s = new School();
            s.Name = Name;
            s.Province = Province;
            s.City = City;
            s.Campus = Campus;
            s.Code = GenerateSchoolCode(Name);

            try
            {
                _context.Add(s);
                await _context.SaveChangesAsync();
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
            ViewData["Campus"] = new SelectList(_context.Campuses, "Code", "Code", school.Campus);
            return View(school);
        }

        // POST: Schools/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string id, [Bind("Code,Province,City,Campus")] School school)
        {
            if (id != school.Code)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(school);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!SchoolExists(school.Code))
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
            ViewData["Campus"] = new SelectList(_context.Campuses, "Code", "Code", school.Campus);
            return View(school);
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
