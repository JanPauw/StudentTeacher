﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using StudentTeacher.Models;

namespace StudentTeacher.Controllers
{
    public class StudentsController : Controller
    {
        private readonly XISD_POEContext _context;

        public StudentsController(XISD_POEContext context)
        {
            _context = context;
        }

        // GET: Students
        public async Task<IActionResult> Index()
        {
            //get schools
            List<School> schools = await _context.Schools.ToListAsync();
            ViewBag.Schools = schools;

            return View(await _context.Students.ToListAsync());
        }

        // GET: Students/Details/5
        public async Task<IActionResult> Details(string id)
        {
            #region Unused Modules Code
            /*#region Get List of Student's Assigned Modules
            //Generate List of Modules
            List<StudentModule> studentModules = _context.StudentModules.Where(x => x.Student == id).ToList();
            List<Module> assignedModules = new List<Module>();

            foreach (var sm in studentModules)
            {
                assignedModules.Add(_context.Modules.Find(sm.Module));
            }
            ViewBag.Modules = assignedModules;
            #endregion

            #region Get List of Student's Un-Assigned Modules
            //Get All Modules
            List<Module> allModules = _context.Modules.ToList();

            //List of Available Modules to Assign
            List<Module> canAssign = new List<Module>();

            //Generate List of Modules where Student is NOT assigned
            foreach (var item in allModules)
            {
                StudentModule studentModule = _context.StudentModules.Where(x => x.Module == item.Number && x.Student == id).SingleOrDefault();
                if (studentModule == null)
                {
                    canAssign.Add(item);
                }
            }
            ViewBag.CanAssignModules = canAssign;
            #endregion*/
            #endregion

            if (id == null || _context.Students == null)
            {
                return NotFound();
            }

            var student = await _context.Students
                .FirstOrDefaultAsync(m => m.Number == id);
            if (student == null)
            {
                return NotFound();
            }

            //Get Current Year Details
            var YearSelected = HttpContext.Request.Query["year"];

            if (string.IsNullOrWhiteSpace(YearSelected))
            {
                return RedirectToAction("Details", "Students", new { id = id, year = "" + student.YearOfStudy });
            }

            int year = int.Parse(YearSelected);

            StudentSchool studentSchool = _context.StudentSchools.Where(x => x.Student == student.Number && x.PlacementYear == year).SingleOrDefault();
            School school = _context.Schools.Find(studentSchool.School);

            ViewBag.School = school;

            //Get Gradings info
            List<Grading> gradings = _context.Gradings.Where(x => x.Student == student.Number && x.YearOfStudy == year).ToList();
            ViewBag.Gradings = gradings;

            //Get Student Subjects
            List<Subject> subjects = await _context.Subjects.Where(x => x.YearOfStudy.Contains(YearSelected)).OrderBy(x => x.Subject1).ToListAsync();
            ViewBag.Subjects = subjects;

            List<string> TeacherNames = new List<string>();
            List<int> totals = new List<int>();

            foreach (var item in gradings)
            {
                Teacher t = _context.Teachers.Find(item.Teacher);
                TeacherNames.Add("" + t.FirstName + " " + t.LastName);
                
                totals.Add(GetTotalMarks(item.Number));
            }

            ViewBag.Teachers = TeacherNames;
            ViewBag.MarksTotals = totals;

            return View(student);
        }

        // GET: Students/Create
        public IActionResult Create()
        {
            ViewBag.ListCampuses = _context.Campuses.ToList();
            return View();
        }

        // POST: Students/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(string Number, string FirstName, string LastName, string Year)
        {
            ViewBag.ListCampuses = _context.Campuses.ToList();

            #region Input Validation
            //check that firstname is not empty
            if (String.IsNullOrEmpty(FirstName))
            {
                TempData["error"] = "Invalid First Name entered!";
                return View();
            }

            //check that firstname is not empty
            if (String.IsNullOrEmpty(LastName))
            {
                TempData["error"] = "Invalid Last Name entered!";
                return View();
            }

            //Check that Student does not already exist
            var student = _context.Students.Find(Number);
            if (student != null)
            {
                TempData["error"] = "Student Number already in use!";
                return View();
            }

            //Test if Valid Year was chosen
            try
            {
                int test = int.Parse(Year);
            }
            catch (Exception e)
            {
                TempData["error"] = "Inavlid Year of Study chosen!";
                return View();
            }
            #endregion

            Student s = new Student();
            s.Number = Number;
            s.FirstName = FirstName;
            s.LastName = LastName;
            s.YearOfStudy = int.Parse(Year);

            try
            {
                _context.Add(s);
                await _context.SaveChangesAsync();
                TempData["success"] = "Student added succesfully!";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception e)
            {
                TempData["error"] = "Error adding Student!";
                return View(s);
            }

        }

        // GET: Students/Edit/5
        public async Task<IActionResult> Edit(string id)
        {
            if (id == null || _context.Students == null)
            {
                return NotFound();
            }

            var student = await _context.Students.FindAsync(id);
            if (student == null)
            {
                return NotFound();
            }

            //populate user input fields
            string firstName = student.FirstName;
            string lastName = student.LastName;

            ViewBag.firstName = firstName;
            ViewBag.lastName = lastName;

            return View(student);
        }

        // POST: Students/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string id, string FirstName, string LastName)
        {
            Student s = new Student();

            //find student by id (studentNumber)
            s = _context.Students.Find(id);

            if (id == null)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    //update user info
                    s.FirstName = FirstName;
                    s.LastName = LastName;

                    //save changes to DB
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!StudentExists(s.Number))
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
            return View(s);
        }

        // GET: Students/Delete/5
        public async Task<IActionResult> Delete(string id)
        {
            if (id == null || _context.Students == null)
            {
                return NotFound();
            }

            var student = await _context.Students
                .FirstOrDefaultAsync(m => m.Number == id);
            if (student == null)
            {
                return NotFound();
            }

            return View(student);
        }

        // POST: Students/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            if (_context.Students == null)
            {
                return Problem("Entity set 'XISD_POEContext.Students'  is null.");
            }
            var student = await _context.Students.FindAsync(id);
            if (student != null)
            {
                _context.Students.Remove(student);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool StudentExists(string id)
        {
            return _context.Students.Any(e => e.Number == id);
        }

        public int GetTotalMarks(int id)
        {
            //Get Objects 
            var planning = _context.Plannings.Where(x => x.GradingNumber == id).SingleOrDefault();
            var execution = _context.Executions.Where(x => x.GradingNumber == id).SingleOrDefault();
            var overall = _context.Overalls.Where(x => x.GradingNumber == id).SingleOrDefault();

            //Check if any object returned null
            if (planning == null || execution == null || overall == null)
            {
                return 0;
            }

            //Planning Total
            var planningTotal = planning.SectionAtoD + planning.SectionE;
            //Execution Total
            var executionTotal = execution.Intro + execution.Teaching + execution.Closure + execution.Assessment;
            //Overall Total
            var overallTotal = overall.Presence + overall.Environment;

            return (planningTotal + executionTotal + overallTotal);
        }
    }
}
