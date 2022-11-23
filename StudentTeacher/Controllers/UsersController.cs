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
    public class UsersController : Controller
    {
        private readonly XISD_POEContext _context;

        public UsersController(XISD_POEContext context)
        {
            _context = context;
        }

        // GET: Users
        public async Task<IActionResult> Index()
        {
            return View(await _context.Users.ToListAsync());
        }

        // GET: Users/Details/5
        public async Task<IActionResult> Details(string id)
        {
            if (id == null || _context.Users == null)
            {
                return NotFound();
            }

            var user = await _context.Users
                .FirstOrDefaultAsync(m => m.Email == id);
            if (user == null)
            {
                return NotFound();
            }

            return View(user);
        }

        // GET: Users/Create
        public IActionResult Login()
        {
            return View();
        }

        // POST: Users/Login
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login([Bind("Email,Password")] User user)
        {
            //If Empty email
            if (String.IsNullOrEmpty(user.Email))
            {
                return View(user);
            }

            //If Empty password
            if (String.IsNullOrEmpty(user.Password))
            {
                return View(user);
            }

            Encrypt enc = new Encrypt();
            String EncryptedPassword = enc.EncryptString(user.Password);

            //Check if user exists in DB
            var u = _context.Users.Find(user.Email);
            if (u == null)
            {
                return View(user);
            }

            //Check if passwords match
            if (!u.Password.Equals(EncryptedPassword))
            {
                return View(user);
            }

            //Setting Session Variables
            HttpContext.Session.SetString("_email", u.Email);
            HttpContext.Session.SetString("_type", u.Type);
            HttpContext.Session.SetString("_role", u.Role);
            HttpContext.Session.SetString("_loggedIn", "true");

            //TODO: Store Session Variables
            TempData["success"] = "Logged in successfully!";
            return RedirectToAction("Dashboard", "Users");
        }

        public IActionResult LogOut()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Login", "Users");
        }

        // GET: Users/Create
        public IActionResult Register()
        {
            return View();
        }

        // POST: Users/Register
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register([Bind("Email,Password,Type,Role")] User user)
        {
            if (ModelState.IsValid)
            {
                Encrypt enc = new Encrypt();
                String EncryptedPassword = enc.EncryptString(user.Password);
                user.Password = EncryptedPassword;

                _context.Add(user);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(user);
        }

        //GET: Users/Dashboard
        public async Task<IActionResult> Dashboard(string? SearchType, string? SearchOption, string? Search)
        {
            //get role of logged in user
            string _role = HttpContext.Session.GetString("_role");

            //Check that Role is set in Sessions
            if (String.IsNullOrWhiteSpace(_role))
            {
                HttpContext.Session.Clear();
                return RedirectToAction("Login", "Users");
            }

            //var FilterAction = HttpContext.Request.Query["filterAction"];

            //if (string.IsNullOrWhiteSpace(FilterAction))
            //{
            //    return RedirectToAction("Dashboard", "Users", new { filterAction = 0 });
            //}


            //check which user is logged in by role
            switch (_role)
            {
                case "Lecturer":
                    return RedirectToAction("Dashboard", "Lecturers");
                    break;
                case "Teacher":
                    return RedirectToAction("Dashboard", "Teachers");
                    break;
                case "Supervisor":
                    return RedirectToAction("Dashboard", "Lecturers");
                    break;
            }

            //Info to disiplay on Dashboard
            ViewBag.Students = _context.Students.ToList();
            ViewBag.Schools = _context.Schools.ToList();
            ViewBag.Teachers = _context.Teachers.ToList();
            ViewBag.Lecturers = _context.Lecturers.ToList();
            ViewBag.Gradings = _context.Gradings.OrderByDescending(x => x.Date).ToList();
            ViewBag.Campuses = _context.Campuses.ToList();

            #region Check for Null or Empty in Searches
            if (String.IsNullOrEmpty(SearchType))
            {
                return View();
            }

            if (String.IsNullOrEmpty(SearchOption))
            {
                return View();
            }

            if (String.IsNullOrEmpty(Search))
            {
                return View();
            }
            #endregion

            #region Students Search
            if (SearchType == "students")
            {
                switch (SearchOption)
                {
                    case "1":
                        ViewBag.Students = _context.Students.Where(x => x.Number.Contains(Search)).ToList();
                        break;
                    case "2":
                        ViewBag.Students = _context.Students.Where(x => (x.FirstName + " " + x.LastName).Contains(Search)).ToList();
                        break;
                    case "3":
                        ViewBag.Students = _context.Students.Where(x => x.Qualification.Contains(Search)).ToList();
                        break;
                    case "4":
                        ViewBag.Students = _context.Students.Where(x => ("" + x.YearOfStudy).Contains(Search)).ToList();
                        break;
                    case "5":
                        ViewBag.Students = _context.Students.Where(x => x.CampusNavigation.Name.Contains(Search)).ToList();
                        break;
                    default:
                        ViewBag.Students = _context.Students.ToList();
                        break;
                }
            }
            #endregion

            #region Gradings Search
            if (SearchType == "gradings")
            {
                switch (SearchOption)
                {
                    case "1":
                        ViewBag.Gradings = _context.Gradings.ToList(); //Date not implemented yet
                        break;
                    case "2":
                        ViewBag.Gradings = _context.Gradings.Where(x => x.StudentNavigation.Number.Contains(Search)).ToList();
                        break;
                    case "3":
                        ViewBag.Gradings = _context.Gradings.Where(x => (x.TeacherNavigation.FirstName + " " + x.TeacherNavigation.LastName).Contains(Search)).ToList();
                        break;
                    case "4":
                        ViewBag.Gradings = _context.Gradings.Where(x => ("" + x.YearOfStudy).Contains(Search)).ToList();
                        break;
                    case "5":
                        ViewBag.Gradings = _context.Gradings.Where(x => x.Subject.Contains(Search)).ToList();
                        break;
                    default:
                        ViewBag.Gradings = _context.Gradings.ToList();
                        break;
                }
            }
            #endregion

            //(admin dashboard)
            return View();
        }

        // GET: Users/Edit/5
        public async Task<IActionResult> Edit(string id)
        {
            if (id == null || _context.Users == null)
            {
                return NotFound();
            }

            var user = await _context.Users.FindAsync(id);
            if (user == null)
            {
                return NotFound();
            }
            return View(user);
        }

        // POST: Users/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string id, [Bind("Email,Password,Type,Role")] User user)
        {
            if (id != user.Email)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(user);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!UserExists(user.Email))
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
            return View(user);
        }

        // GET: Users/Delete/5
        public async Task<IActionResult> Delete(string id)
        {
            if (id == null || _context.Users == null)
            {
                return NotFound();
            }

            var user = await _context.Users
                .FirstOrDefaultAsync(m => m.Email == id);
            if (user == null)
            {
                return NotFound();
            }

            return View(user);
        }

        // POST: Users/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            if (_context.Users == null)
            {
                return Problem("Entity set 'XISD_POEContext.Users'  is null.");
            }
            var user = await _context.Users.FindAsync(id);
            if (user != null)
            {
                _context.Users.Remove(user);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool UserExists(string id)
        {
            return _context.Users.Any(e => e.Email == id);
        }
    }
}
