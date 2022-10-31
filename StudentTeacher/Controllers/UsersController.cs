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
        public async Task<IActionResult> Dashboard()
        {
            //get role of logged in user
            string _role = HttpContext.Session.GetString("_role");

            //Check that Role is set in Sessions
            if (String.IsNullOrWhiteSpace(_role))
            {
                HttpContext.Session.Clear();
                return RedirectToAction("Login", "Users");
            }

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
