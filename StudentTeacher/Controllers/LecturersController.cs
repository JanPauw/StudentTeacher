using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using StudentTeacher.Data;
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

        public IActionResult Register()
        {
            return View();
        }

        // POST: Lecturers/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(string FirstName, string LastName, string Email, string Password, string ConfirmPassword, string CampusCode)
        {
            #region Input Validation
            if (String.IsNullOrEmpty(FirstName))
            {
                TempData["error"] = "Invalid First Name entered!";
                return View();
            }

            if (String.IsNullOrEmpty(LastName))
            {
                TempData["error"] = "Invalid Last Name entered!";
                return View();
            }

            if (String.IsNullOrEmpty(Email))
            {
                TempData["error"] = "Invalid Email entered!";
                return View();
            }

            //Check if Email is already Registered
            var user = _context.Users.Find(Email);
            if (user != null)
            {
                TempData["error"] = "Email is already registered!";
                return View();
            }

            if (String.IsNullOrEmpty(Password))
            {
                TempData["error"] = "Invalid Password entered!";
                return View();
            }

            //Check Length of Password
            if (Password.Length < 8)
            {
                TempData["error"] = "Password should be minimum 8 characters!";
                return View();
            }

            if (String.IsNullOrEmpty(ConfirmPassword))
            {
                TempData["error"] = "Passwords do not match!";
                return View();
            }

            if (!Password.Equals(ConfirmPassword))
            {
                TempData["error"] = "Passwords do not match!";
                return View();
            }

            //Check Valid School Code
            var campus = _context.Campuses.Find(CampusCode);
            if (campus == null)
            {
                TempData["error"] = "Invalid School Code entered!";
                return View();
            }
            #endregion

            //Creating new User
            Encrypt enc = new Encrypt();
            string EncyptedPassword = enc.EncryptString(Password);

            User u = new User();
            u.Email = Email;
            u.Password = EncyptedPassword;
            u.Type = "VC Lecturer";
            u.Role = "lecturer";
            _context.Users.Add(u);
            _context.SaveChanges();

            Lecturer l = new Lecturer();
            l.Number = GenerateLectuerCode(FirstName, LastName);
            l.FirstName = FirstName;
            l.LastName = LastName;
            l.Email = Email;
            l.EmailNavigation = u;
            l.Campus = CampusCode;
            l.CampusNavigation = campus;

            try
            {
                _context.Add(l);
                await _context.SaveChangesAsync();
                TempData["success"] = "Registration Successful!";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception e)
            {
                ViewData["Campus"] = new SelectList(_context.Campuses, "Code", "Code", l.Campus);
                ViewData["Email"] = new SelectList(_context.Users, "Email", "Email", l.Email);
                TempData["error"] = "Registration Unsuccessful!";
                return View(l);
            }
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
        public async Task<IActionResult> Create(string Number, string FirstName, string LastName, string Email, string Campus)
        {
            Lecturer l = new Lecturer();

            l.Number = GenerateLectuerCode(FirstName, LastName);
            l.FirstName = FirstName;
            l.LastName = LastName;
            l.Email = Email;
            l.EmailNavigation = _context.Users.Find(Email);
            l.Campus = Campus;
            l.CampusNavigation = _context.Campuses.Find(Campus);

            try
            {
                _context.Add(l);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            catch (Exception e)
            {
                ViewData["Campus"] = new SelectList(_context.Campuses, "Code", "Code", l.Campus);
                ViewData["Email"] = new SelectList(_context.Users, "Email", "Email", l.Email);
                return View(l);
            }
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

            //populate user input fields
            string firstName = lecturer.FirstName;
            string lastName = lecturer.LastName;

            ViewBag.FirstName = firstName;
            ViewBag.LastName = lastName;

            ViewData["Campus"] = new SelectList(_context.Campuses, "Code", "Code", lecturer.Campus);
            ViewData["Email"] = new SelectList(_context.Users, "Email", "Email", lecturer.Email);
            return View(lecturer);
        }

        // POST: Lecturers/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string id, string FirstName, string LastName, string Campus)
        {
            Lecturer l = new Lecturer();

            //find lecturer by id (lecturerNumber)
            l = _context.Lecturers.Find(id);

            if (id == null)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    //update user info
                    l.FirstName = FirstName;
                    l.LastName = LastName;
                    l.Campus = Campus;

                    //save changes to DB
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!LecturerExists(l.Number))
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
            ViewData["Campus"] = new SelectList(_context.Campuses, "Code", "Code", l.Campus);
            ViewData["Email"] = new SelectList(_context.Users, "Email", "Email", l.Email);
            return View(l);
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


        //Lecturer Code Generator
        private string GenerateLectuerCode(string FirstName, string LastName)
        {
            string toReturn = "";

            FirstName = FirstName.ToUpper();
            LastName = LastName.ToUpper();

            toReturn += FirstName.Substring(0, 1);
            toReturn += LastName.Substring(0, 1);

            Random rnd = new Random();
            int RandomNumber = rnd.Next(1000, 10000);

            toReturn += RandomNumber.ToString();

            if (!LecturerExists(toReturn))
            {
                return toReturn;
            }
            else
            {
                return GenerateLectuerCode(FirstName, LastName);
            }
        }


    }
}
