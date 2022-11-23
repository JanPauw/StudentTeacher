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
    public class TeachersController : Controller
    {
        private readonly XISD_POEContext _context;

        public TeachersController(XISD_POEContext context)
        {
            _context = context;
        }

        // GET: Teachers
        public async Task<IActionResult> Index()
        {
            var xISD_POEContext = _context.Teachers.Include(t => t.EmailNavigation).Include(t => t.SchoolNavigation);
            return View(await xISD_POEContext.ToListAsync());
        }

        // GET: Dashboard
        public IActionResult Dashboard()
        {
            //get current logged in teacher
            string email = HttpContext.Session.GetString("_email");

            if (string.IsNullOrWhiteSpace(email))
            {
                return RedirectToAction("LogOut", "Users");
            }

            Teacher loggedIn = _context.Teachers.Where(x => x.Email == email).SingleOrDefault();

            if (loggedIn == null)
            {
                return RedirectToAction("LogOut", "Users");
            }

            //get role of logged in user
            string _role = HttpContext.Session.GetString("_role");

            //Check that Role is set in Sessions
            if (String.IsNullOrWhiteSpace(_role))
            {
                HttpContext.Session.Clear();
                return RedirectToAction("Login", "Users");
            }

            var FilterAction = HttpContext.Request.Query["filterAction"];

            if (string.IsNullOrWhiteSpace(FilterAction))
            {
                return RedirectToAction("Dashboard", "Teachers", new { filterAction = 0 });
            }

            //check which user is logged in by role
            switch (_role)
            {
                case "Lecturer":
                    return RedirectToAction("Dashboard", "Lecturers");
                case "Supervisor":
                    return RedirectToAction("Dashboard", "Lecturers");
            }

            #region Get List of Teacher's Students
            string school = loggedIn.School;

            //get StudentSchools mathcing teacher school
            List<StudentSchool> studentSchools = _context.StudentSchools.Where(x => x.School == school).ToList();
            List<Student> studentList = new List<Student>();

            foreach (var item in studentSchools)
            {
                //get student
                Student s = _context.Students.Find(item.Student);

                //check null
                if (s == null)
                {
                    TempData["error"] = "Please contact system admin!";
                    return RedirectToAction("LogOut", "Users");
                }

                StudentSchool ss = studentSchools.Where(x => x.Student == s.Number).SingleOrDefault();

                //Check null
                if (ss != null)
                {
                    studentList.Add(s);
                }
            }
            #endregion

            //Info to disiplay on Dashboard
            ViewBag.Students = studentList;
            ViewBag.Schools = _context.Schools.ToList();
            ViewBag.Teachers = _context.Teachers.ToList();
            ViewBag.Lecturers = _context.Lecturers.ToList();
            ViewBag.Gradings = _context.Gradings.Where(x => x.TeacherNavigation.Email == email).OrderByDescending(x => x.Date).ToList();
            ViewBag.Campuses = _context.Campuses.ToList();



            //(dashboard)
            return View();
        }

        // GET: Teachers/Details/5
        public async Task<IActionResult> Details(string id)
        {
            if (id == null || _context.Teachers == null)
            {
                return NotFound();
            }

            var teacher = await _context.Teachers
                .Include(t => t.EmailNavigation)
                .Include(t => t.SchoolNavigation)
                .FirstOrDefaultAsync(m => m.Number == id);
            if (teacher == null)
            {
                return NotFound();
            }

            return View(teacher);
        }

        public IActionResult Register()
        {
            return View();
        }

        // POST: Teachers/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(string FirstName, string LastName, string SchoolCode, string Email, string Password, string ConfirmPassword)
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
            var school = _context.Schools.Find(SchoolCode);
            if (school == null)
            {
                TempData["error"] = "Invalid School Code entered!";
                return View();
            }
            #endregion

            //Encryption
            Encrypt enc = new Encrypt();

            //Create new User
            User u = new User();
            u.Email = Email;
            u.Password = enc.EncryptString(Password);
            u.Type = "School Teacher";
            u.Role = "Teacher";
            _context.Users.Add(u);
            _context.SaveChanges();

            Teacher t = new Teacher();
            t.Number = GenerateTeacherCode(FirstName, LastName);
            t.FirstName = FirstName;
            t.LastName = LastName;
            t.School = SchoolCode;
            t.SchoolNavigation = school;
            t.Email = Email;
            t.EmailNavigation = user;

            try
            {
                _context.Add(t);
                await _context.SaveChangesAsync();
                TempData["success"] = "Register Successful!";
                return RedirectToAction("Login", "Users");
            }
            catch (Exception e)
            {
                TempData["error"] = "Register Unsuccessful!";
                ViewData["Email"] = new SelectList(_context.Users, "Email", "Email", t.Email);
                ViewData["School"] = new SelectList(_context.Schools, "Code", "Code", t.School);
                return View(t);
            }
        }

        // GET: Teachers/Create
        public IActionResult Create()
        {
            ViewData["Email"] = new SelectList(_context.Users, "Email", "Email");
            ViewData["School"] = new SelectList(_context.Schools, "Code", "Code");
            return View();
        }

        // POST: Teachers/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(string Number, string FirstName, string LastName, string School, string Email)
        {
            Teacher t = new Teacher();
            t.Number = Number;
            t.FirstName = FirstName;
            t.LastName = LastName;
            t.School = School;
            t.SchoolNavigation = _context.Schools.Find(School);
            t.Email = Email;
            t.EmailNavigation = _context.Users.Find(Email);

            try
            {
                _context.Add(t);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            catch (Exception e)
            {
                ViewData["Email"] = new SelectList(_context.Users, "Email", "Email", t.Email);
                ViewData["School"] = new SelectList(_context.Schools, "Code", "Code", t.School);
                return View(t);
            }
        }

        // GET: Teachers/Edit/5
        public async Task<IActionResult> Edit(string id)
        {
            if (id == null || _context.Teachers == null)
            {
                return NotFound();
            }

            var teacher = await _context.Teachers.FindAsync(id);
            if (teacher == null)
            {
                return NotFound();
            }

            //populate user input fields
            string firstName = teacher.FirstName;
            string lastName = teacher.LastName;
            string school = teacher.School;

            ViewBag.FirstName = firstName;
            ViewBag.LastName = lastName;
            //ViewBag.School = school;

            ViewData["Email"] = new SelectList(_context.Users, "Email", "Email", teacher.Email);
            ViewData["School"] = new SelectList(_context.Schools, "Code", "Code", teacher.School);
            return View(teacher);
        }

        // POST: Teachers/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string id, string FirstName, string LastName, string School)
        {
            Teacher t = new Teacher();

            //find teacher by id (teacherNumber)
            t = _context.Teachers.Find(id);

            if (id == null)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    //update teacher info
                    t.FirstName = FirstName;
                    t.LastName = LastName;
                    t.School = School;

                    //save changes to DB
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!TeacherExists(t.Number))
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
            ViewData["Email"] = new SelectList(_context.Users, "Email", "Email", t.Email);
            ViewData["School"] = new SelectList(_context.Schools, "Code", "Code", t.School);
            return View(t);
        }

        // GET: Teachers/Delete/5
        public async Task<IActionResult> Delete(string id)
        {
            if (id == null || _context.Teachers == null)
            {
                return NotFound();
            }

            var teacher = await _context.Teachers
                .Include(t => t.EmailNavigation)
                .Include(t => t.SchoolNavigation)
                .FirstOrDefaultAsync(m => m.Number == id);
            if (teacher == null)
            {
                return NotFound();
            }

            return View(teacher);
        }

        // POST: Teachers/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            if (_context.Teachers == null)
            {
                return Problem("Entity set 'XISD_POEContext.Teachers'  is null.");
            }
            var teacher = await _context.Teachers.FindAsync(id);
            if (teacher != null)
            {
                _context.Teachers.Remove(teacher);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool TeacherExists(string id)
        {
            return _context.Teachers.Any(e => e.Number == id);
        }

        //Teacher Code Generator
        private string GenerateTeacherCode(string FirstName, string LastName)
        {
            string toReturn = "";

            FirstName = FirstName.ToUpper();
            LastName = LastName.ToUpper();

            toReturn += FirstName.Substring(0, 1);
            toReturn += LastName.Substring(0, 1);

            Random rnd = new Random();
            int RandomNumber = rnd.Next(1000, 10000);

            toReturn += RandomNumber.ToString();

            if (!TeacherExists(toReturn))
            {
                return toReturn;
            }
            else
            {
                return GenerateTeacherCode(FirstName, LastName);
            }
        }
    }
}
