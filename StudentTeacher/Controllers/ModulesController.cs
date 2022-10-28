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
    public class ModulesController : Controller
    {
        private readonly XISD_POEContext _context;

        public ModulesController(XISD_POEContext context)
        {
            _context = context;
        }

        // GET: Modules
        public async Task<IActionResult> Index()
        {
            return View(await _context.Modules.ToListAsync());
        }

        // GET: Modules/Details/5
        public async Task<IActionResult> Details(string id)
        {
            if (id == null || _context.Modules == null)
            {
                return NotFound();
            }

            var @module = await _context.Modules
                .FirstOrDefaultAsync(m => m.Number == id);
            if (@module == null)
            {
                return NotFound();
            }

            return View(@module);
        }

        // GET: Modules/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Modules/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(string Number, string Name)
        {
            #region Input validation
            //check that Number is not empty
            if (String.IsNullOrEmpty(Number) || Number.Length != 8)
            {
                TempData["error"] = "Invalid Module Number entered!";
                return View();
            }

            //Check that LetterPart is only letters
            Char[] LetterPart = Number.Substring(0, 4).ToCharArray();
            for (int i = 0; i < LetterPart.Length; i++ )
            {
                if (!Char.IsLetter(LetterPart[i]))
                {
                    TempData["error"] = "Invalid Module Code Format!";
                    return View();
                }
            }

            //Check that NumberPart is only numbers
            Char[] NumberPart = Number.Substring(4, 4).ToCharArray();
            for (int i = 0; i < NumberPart.Length; i++)
            {
                try
                {
                    int test = NumberPart[i];
                }
                catch (Exception e)
                {
                    TempData["error"] = "Invalid Module Code Format!";
                    return View();
                }
            }

            //check that Name is not empty
            if (String.IsNullOrEmpty(Name))
            {
                TempData["error"] = "Invalid Module Name entered!";
                return View();
            }

            #endregion

            Module m = new Module();
            m.Number = Number; 
            m.Name = Name;


            try
            {
                _context.Add(m);
                await _context.SaveChangesAsync();
                TempData["success"] = "Module added Successfully!";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception e)
            {
                return View(m);
            }
            
        }

        // GET: Modules/Edit/5
        public async Task<IActionResult> Edit(string id)
        {
            if (id == null || _context.Modules == null)
            {
                return NotFound();
            }

            var @module = await _context.Modules.FindAsync(id);
            if (@module == null)
            {
                return NotFound();
            }

            //populate user input fields
            string name = module.Name;

            ViewBag.Name = name;

            return View(@module);
        }

        // POST: Modules/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string id, string name)
        {
            Module m = new Module();

            //find module by id (module code)
            m = _context.Modules.Find(id);

            if (id == null)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    //update module info
                    m.Name = name;

                    //save changes to DB
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ModuleExists(m.Number))
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
            return View(m);
        }

        // GET: Modules/Delete/5
        public async Task<IActionResult> Delete(string id)
        {
            if (id == null || _context.Modules == null)
            {
                return NotFound();
            }

            var @module = await _context.Modules
                .FirstOrDefaultAsync(m => m.Number == id);
            if (@module == null)
            {
                return NotFound();
            }

            return View(@module);
        }

        // POST: Modules/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            if (_context.Modules == null)
            {
                return Problem("Entity set 'XISD_POEContext.Modules'  is null.");
            }
            var @module = await _context.Modules.FindAsync(id);
            if (@module != null)
            {
                _context.Modules.Remove(@module);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ModuleExists(string id)
        {
            return _context.Modules.Any(e => e.Number == id);
        }
    }
}
