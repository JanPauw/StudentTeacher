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
    public class CampusController : Controller
    {
        private readonly XISD_POEContext _context;

        public CampusController(XISD_POEContext context)
        {
            _context = context;
        }

        // GET: Campus
        public async Task<IActionResult> Index()
        {
              return View(await _context.Campuses.ToListAsync());
        }

        // GET: Campus/Details/5
        public async Task<IActionResult> Details(string id)
        {
            if (id == null || _context.Campuses == null)
            {
                return NotFound();
            }

            var campus = await _context.Campuses
                .FirstOrDefaultAsync(m => m.Code == id);
            if (campus == null)
            {
                return NotFound();
            }

            List<Student> studentList = _context.Students.Where(x => x.Campus == id).ToList();
            List<School> schoolList = _context.Schools.Where(x => x.Campus == id).ToList();

            ViewBag.StudentList = studentList;
            ViewBag.SchoolList = schoolList;

            return View(campus);
        }

        // GET: Campus/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Campus/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(string Province, string City, string Name)
        {
            #region Input validation
            //check if Province is empty
            if (String.IsNullOrEmpty(Province))
            {
                TempData["error"] = "Invalid Province entered!";
                return View();
            }

            //check if City is empty
            if (String.IsNullOrEmpty(City) || City.Length < 4)
            {
                TempData["error"] = "Invalid City entered!";
                return View();
            }

            //check if Name is empty
            if (String.IsNullOrWhiteSpace(Name))
            {
                TempData["error"] = "Invalid Campus Name entered!";
                return View();
            }
            #endregion

            Campus c = new Campus();
            c.Code = GenerateCampusCode(City);
            c.Province = Province;
            c.City = City;
            c.Name = Name;

            try
            {
                _context.Add(c);
                await _context.SaveChangesAsync();
                TempData["success"] = "Campus added Successfully!";
                return RedirectToAction(nameof(Index));
            }
            catch(Exception e)
            {
                ViewData["Campus"] = new SelectList(_context.Campuses, "Code", "Code", c.Code);
                return View(c);
            }
        }

        // GET: Campus/Edit/5
        public async Task<IActionResult> Edit(string id)
        {
            if (id == null || _context.Campuses == null)
            {
                return NotFound();
            }

            var campus = await _context.Campuses.FindAsync(id);
            if (campus == null)
            {
                return NotFound();
            }
            return View(campus);
        }

        // POST: Campus/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string id, [Bind("Code,Province,City")] Campus campus)
        {
            if (id != campus.Code)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(campus);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!CampusExists(campus.Code))
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
            return View(campus);
        }

        // GET: Campus/Delete/5
        public async Task<IActionResult> Delete(string id)
        {
            if (id == null || _context.Campuses == null)
            {
                return NotFound();
            }

            var campus = await _context.Campuses
                .FirstOrDefaultAsync(m => m.Code == id);
            if (campus == null)
            {
                return NotFound();
            }

            return View(campus);
        }

        // POST: Campus/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            if (_context.Campuses == null)
            {
                return Problem("Entity set 'XISD_POEContext.Campuses'  is null.");
            }
            var campus = await _context.Campuses.FindAsync(id);
            if (campus != null)
            {
                _context.Campuses.Remove(campus);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool CampusExists(string id)
        {
          return _context.Campuses.Any(e => e.Code == id);
        }

        //Campus Code Generator
        private string GenerateCampusCode(string City)
        {
            string toReturn = "";

            City = City.ToUpper();

            toReturn += City.Substring(0, 4);

            Random rnd = new Random();
            int RandomNumber = rnd.Next(1000, 10000);

            toReturn += RandomNumber.ToString();

            if (!CampusExists(toReturn))
            {
                return toReturn;
            }
            else
            {
                return GenerateCampusCode(City);
            }
        }


    }
}
