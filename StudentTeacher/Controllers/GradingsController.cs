using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DocumentFormat.OpenXml.InkML;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using StudentTeacher.Models;

namespace StudentTeacher.Controllers
{
    public class GradingsController : Controller
    {
        private readonly XISD_POEContext _context;

        public GradingsController(XISD_POEContext context)
        {
            _context = context;
        }

        // GET: Gradings
        public async Task<IActionResult> Index()
        {
            var xISD_POEContext = _context.Gradings.Include(g => g.StudentNavigation).Include(g => g.TeacherNavigation);
            return View(await xISD_POEContext.ToListAsync());
        }

        // GET: Gradings/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.Gradings == null)
            {
                return NotFound();
            }

            var grading = await _context.Gradings
                .Include(g => g.StudentNavigation)
                .Include(g => g.TeacherNavigation)
                .FirstOrDefaultAsync(m => m.Number == id);
            if (grading == null)
            {
                return NotFound();
            }

            //get marks allocations
            var SectionAtoD = _context.Plannings.Where(x => x.GradingNumber == id).SingleOrDefault().SectionAtoD;
            var SectionE = _context.Plannings.Where(x => x.GradingNumber == id).SingleOrDefault().SectionE;

            var Intro = _context.Executions.Where(x => x.GradingNumber == id).SingleOrDefault().Intro;
            var Teaching = _context.Executions.Where(x => x.GradingNumber == id).SingleOrDefault().Teaching;
            var Closure = _context.Executions.Where(x => x.GradingNumber == id).SingleOrDefault().Closure;
            var Assessment = _context.Executions.Where(x => x.GradingNumber == id).SingleOrDefault().Assessment;

            var Presence = _context.Overalls.Where(x => x.GradingNumber == id).SingleOrDefault().Presence;
            var Environment = _context.Overalls.Where(x => x.GradingNumber == id).SingleOrDefault().Environment;

            //send variables through ViewBag
            ViewBag.SectionAtoD = SectionAtoD;
            ViewBag.SectionE = SectionE;
            ViewBag.Intro = Intro;
            ViewBag.Teaching = Teaching;
            ViewBag.Closure = Closure;
            ViewBag.Assessment = Assessment;
            ViewBag.Presence = Presence;
            ViewBag.Environment = Environment;

            //get commentaries for this grading
            List<Commentary> comments = await _context.Commentaries.Where(x => x.GradingNumber == id).ToListAsync();
            ViewBag.Comments = comments;

            return View(grading);
        }

        // GET: Gradings/Create
        public IActionResult Create()
        {
            //Get Student Number from Session
            string _studentNumber = HttpContext.Session.GetString("_studentNumber");
            //Test valid Student Number
            Student student = _context.Students.Find(_studentNumber);

            if (student == null)
            {
                return RedirectToAction("Index", "Students");
            }

            //Check that user viewing this page is a teacher
            var _role = HttpContext.Session.GetString("_role");
            if (_role != "Teacher")
            {
                return RedirectToAction("Details", "Students", new { id = _studentNumber });
            }

            //Set Teacher Number
            var _email = HttpContext.Session.GetString("_email");
            Teacher teacher = _context.Teachers.Where(x => x.Email == _email).SingleOrDefault();

            if (teacher == null)
            {
                return RedirectToAction("Details", "Students", new { id = _studentNumber });
            }

            ViewBag.TeacherCode = teacher.Number;
            ViewBag.StudentCode = student.Number;

            //Get Subjects for Student
            List<Subject> subjects = _context.Subjects.Where(x => x.YearOfStudy.Contains("" + student.YearOfStudy)).OrderBy(x => x.Subject1).ToList();
            ViewBag.Subjects = subjects;

            //get student Year of Study to populate grade
            ViewBag.YearOfStudy = student.YearOfStudy;

            return View();
        }

        // POST: Gradings/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(
            string Grade, string Topic, string Subject,
            string Arating, string Erating, string INTROrating, string EXErating, string CLOSURErating,
            string ASSESSMENTrating, string PRESENCErating, string ENVIRONMENTrating,
            string ComSectionAtoD, string ComSectionE, string ComIntro, string ComTeaching, string ComClosure,
            string ComAssessment, string ComPresence, string ComEnvironment,
            string StudentCode, string TeacherCode)
        {
            #region Check that Student and Teacher is not null
            var student = _context.Students.Find(StudentCode);
            var teacher = _context.Teachers.Find(TeacherCode);

            if (student == null)
            {
                TempData["error"] = "Invalid Student selected!";
                return View();
            }

            if (teacher == null)
            {
                TempData["error"] = "Invalid Teacher selected!";
                return View();
            }
            #endregion

            #region Check that Class info is valid
            int iGrade;
            try
            {
                iGrade = int.Parse(Grade);
            }
            catch
            {
                TempData["error"] = "Invalid Grade Entered!";
                return View();
            }

            if (iGrade < 4 || iGrade > 7)
            {
                TempData["error"] = "Invalid Grade Entered!";
                return View();
            }

            if (string.IsNullOrWhiteSpace(Topic))
            {
                TempData["error"] = "Invalid Topic enetered!";
                return View();
            }

            if (string.IsNullOrWhiteSpace(Subject))
            {
                TempData["error"] = "Invalid Subject enetered!";
                return View();
            }
            #endregion

            #region Check if Radios are not null
            if (Arating.ToString() == null)
            {
                TempData["error"] = "Invalid rating selected!";
                return View();
            }
            if (Erating.ToString() == null)
            {
                TempData["error"] = "Invalid rating selected!";
                return View();
            }
            if (INTROrating.ToString() == null)
            {
                TempData["error"] = "Invalid rating selected!";
                return View();
            }
            if (EXErating.ToString() == null)
            {
                TempData["error"] = "Invalid rating selected!";
                return View();
            }
            if (CLOSURErating.ToString() == null)
            {
                TempData["error"] = "Invalid rating selected!";
                return View();
            }
            if (ASSESSMENTrating.ToString() == null)
            {
                TempData["error"] = "Invalid rating selected!";
                return View();
            }
            if (PRESENCErating.ToString() == null)
            {
                TempData["error"] = "Invalid rating selected!";
                return View();
            }
            if (ENVIRONMENTrating.ToString() == null)
            {
                TempData["error"] = "Invalid rating selected!";
                return View();
            }

            #endregion

            #region Check if Commentary is not null
            if (string.IsNullOrWhiteSpace(ComSectionAtoD))
            {
                TempData["error"] = "Invalid Commentary!";
                return View();
            }

            if (string.IsNullOrWhiteSpace(ComSectionE))
            {
                TempData["error"] = "Invalid Commentary!";
                return View();
            }

            if (string.IsNullOrWhiteSpace(ComIntro))
            {
                TempData["error"] = "Invalid Commentary!";
                return View();
            }

            if (string.IsNullOrWhiteSpace(ComTeaching))
            {
                TempData["error"] = "Invalid Commentary!";
                return View();
            }

            if (string.IsNullOrWhiteSpace(ComClosure))
            {
                TempData["error"] = "Invalid Commentary!";
                return View();
            }

            if (string.IsNullOrWhiteSpace(ComAssessment))
            {
                TempData["error"] = "Invalid Commentary!";
                return View();
            }

            if (string.IsNullOrWhiteSpace(ComPresence))
            {
                TempData["error"] = "Invalid Commentary!";
                return View();
            }

            if (string.IsNullOrWhiteSpace(ComEnvironment))
            {
                TempData["error"] = "Invalid Commentary!";
                return View();
            }
            #endregion

            //Create new Grading
            Grading grading = new Grading();
            grading.Student = StudentCode;
            grading.Teacher = TeacherCode;
            grading.StudentNavigation = student;
            grading.TeacherNavigation = teacher;
            grading.YearOfStudy = student.YearOfStudy;
            grading.Date = DateTime.Now;
            grading.Grade = iGrade;
            grading.Topic = Topic;
            grading.Subject = Subject;

            _context.Gradings.Add(grading);
            await _context.SaveChangesAsync();

            #region Create Grading Marks
            //Planning Section
            Planning planning = new Planning();

            planning.SectionAtoD = int.Parse(Arating);
            planning.SectionE = int.Parse(Erating);

            planning.GradingNumber = grading.Number;
            planning.GradingNumberNavigation = grading;

            _context.Plannings.Add(planning);
            await _context.SaveChangesAsync();

            //Execution Section
            Execution execution = new Execution();

            execution.Intro = int.Parse(INTROrating);
            execution.Teaching = int.Parse(EXErating);
            execution.Closure = int.Parse(CLOSURErating);
            execution.Assessment = int.Parse(ASSESSMENTrating);

            execution.GradingNumber = grading.Number;
            execution.GradingNumberNavigation = grading;

            _context.Executions.Add(execution);
            await _context.SaveChangesAsync();

            //Overall Section
            Overall overall = new Overall();

            overall.Presence = int.Parse(PRESENCErating);
            overall.Environment = int.Parse(ENVIRONMENTrating);

            overall.GradingNumber = grading.Number;
            overall.GradingNumberNavigation = grading;

            _context.Overalls.Add(overall);
            await _context.SaveChangesAsync();
            #endregion

            #region Create Grading Commentaries
            //Planning Section
            Commentary sectionAtoD = new Commentary();
            Commentary sectionE = new Commentary();

            sectionAtoD.Criteria = "SectionAtoD";
            sectionE.Criteria = "SectionE";

            sectionAtoD.Comment = "" + ComSectionAtoD;
            sectionE.Comment = "" + ComSectionE;

            sectionAtoD.GradingNumber = grading.Number;
            sectionE.GradingNumber = grading.Number;

            sectionAtoD.GradingNumberNavigation = grading;
            sectionE.GradingNumberNavigation = grading;

            _context.Commentaries.Add(sectionAtoD);
            _context.Commentaries.Add(sectionE);

            await _context.SaveChangesAsync();

            //Execution Section
            Commentary intro = new Commentary();
            Commentary teaching = new Commentary();
            Commentary closure = new Commentary();
            Commentary assessment = new Commentary();

            intro.Criteria = "Intro";
            teaching.Criteria = "Teaching";
            closure.Criteria = "Closure";
            assessment.Criteria = "Assessment";

            intro.Comment = "" + ComIntro;
            teaching.Comment = "" + ComTeaching;
            closure.Comment = "" + ComClosure;
            assessment.Comment = "" + ComAssessment;

            intro.GradingNumber = grading.Number;
            teaching.GradingNumber = grading.Number;
            closure.GradingNumber = grading.Number;
            assessment.GradingNumber = grading.Number;

            intro.GradingNumberNavigation = grading;
            teaching.GradingNumberNavigation = grading;
            closure.GradingNumberNavigation = grading;
            assessment.GradingNumberNavigation = grading;

            _context.Commentaries.Add(intro);
            _context.Commentaries.Add(teaching);
            _context.Commentaries.Add(closure);
            _context.Commentaries.Add(assessment);

            await _context.SaveChangesAsync();

            //Overall Section
            Commentary presence = new Commentary();
            Commentary environment = new Commentary();

            presence.Criteria = "Presence";
            environment.Criteria = "Environment";

            presence.Comment = "" + ComPresence;
            environment.Comment = "" + ComEnvironment;

            presence.GradingNumber = grading.Number;
            environment.GradingNumber = grading.Number;

            presence.GradingNumberNavigation = grading;
            environment.GradingNumberNavigation = grading;

            _context.Commentaries.Add(presence);
            _context.Commentaries.Add(environment);

            await _context.SaveChangesAsync();
            #endregion

            TempData["success"] = "Grading added Successfully!";
            return RedirectToAction("Details", "Students", new { id = student.Number });
        }

        // GET: Gradings/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.Gradings == null)
            {
                return NotFound();
            }

            var grading = await _context.Gradings.FindAsync(id);
            if (grading == null)
            {
                return NotFound();
            }

            //get marks allocations
            var SectionAtoD = _context.Plannings.Where(x => x.GradingNumber == id).SingleOrDefault().SectionAtoD;
            var SectionE = _context.Plannings.Where(x => x.GradingNumber == id).SingleOrDefault().SectionE;

            var Intro = _context.Executions.Where(x => x.GradingNumber == id).SingleOrDefault().Intro;
            var Teaching = _context.Executions.Where(x => x.GradingNumber == id).SingleOrDefault().Teaching;
            var Closure = _context.Executions.Where(x => x.GradingNumber == id).SingleOrDefault().Closure;
            var Assessment = _context.Executions.Where(x => x.GradingNumber == id).SingleOrDefault().Assessment;

            var Presence = _context.Overalls.Where(x => x.GradingNumber == id).SingleOrDefault().Presence;
            var Environment = _context.Overalls.Where(x => x.GradingNumber == id).SingleOrDefault().Environment;

            //send variables through ViewBag
            ViewBag.SectionAtoD = SectionAtoD;
            ViewBag.SectionE = SectionE;
            ViewBag.Intro = Intro;
            ViewBag.Teaching = Teaching;
            ViewBag.Closure = Closure;
            ViewBag.Assessment = Assessment;
            ViewBag.Presence = Presence;
            ViewBag.Environment = Environment;

            //get commentaries for this grading
            List<Commentary> comments = await _context.Commentaries.Where(x => x.GradingNumber == id).ToListAsync();
            ViewBag.Comments = comments;

            //get teacher
            Teacher t = _context.Teachers.Find(grading.Teacher);

            if (t == null)
            {
                return RedirectToAction("Details", new { id = id });
            }

            ViewBag.Teacher = t;

            //get student
            Student s = _context.Students.Find(grading.Student);

            if (s == null)
            {
                return RedirectToAction("Details", new { id = id });
            }

            ViewBag.Student = s;

            //get subjects
            List<Subject> subjects = _context.Subjects.Where(x => x.YearOfStudy == "" + s.YearOfStudy).ToList();

            ViewBag.Subjects = subjects;

            ViewBag.Grading = grading;

            return View(grading);
        }

        // POST: Gradings/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(
            int id, string Grade, string Subject, string Topic,
            string Arating, string Erating, string INTROrating,
            string EXErating, string CLOSURErating, string ASSESSMENTrating,
            string PRESENCErating, string ENVIRONMENTrating,
            string ComSectionAtoD, string ComSectionE, string ComIntro,
            string ComTeaching, string ComClosure, string ComAssessment,
            string ComPresence, string ComEnvironment
            )
        {
            //test id
            Grading grading = _context.Gradings.Find(id);
            if (grading == null)
            {
                return RedirectToAction("Index", "Gradings");
            }

            //get realting student
            Student student = _context.Students.Find(grading.Student);
            if (student == null)
            {
                return RedirectToAction("Index", "Gradings");
            }

            #region Null Checks
            //Grade
            if (string.IsNullOrEmpty(Grade))
            {
                TempData["error"] = "Inavlid Grade Selected";
                return View(grading);
            }
            else
            {
                try
                {
                    int iGrade = int.Parse(Grade);
                }
                catch
                {
                    TempData["error"] = "Inavlid Grade Selected";
                    return View(grading);
                }
            }

            //Subject
            if (string.IsNullOrEmpty(Subject))
            {
                TempData["error"] = "Inavlid Subject Selected";
                return View(grading);
            }
            else
            {
                bool exists = _context.Subjects.Where(x => x.Subject1 == Subject && x.YearOfStudy == "" + student.YearOfStudy).Count() > 0;
                if (!exists)
                {
                    TempData["error"] = "Inavlid Subject Selected";
                    return View(grading);
                }
            }

            //Topic
            if (string.IsNullOrEmpty(Topic))
            {
                TempData["error"] = "Inavlid Topic Entered!";
                return View(grading);
            }

            #region Check if Radios are not null
            if (Arating.ToString() == null)
            {
                TempData["error"] = "Invalid rating selected!";
                return View();
            }
            if (Erating.ToString() == null)
            {
                TempData["error"] = "Invalid rating selected!";
                return View();
            }
            if (INTROrating.ToString() == null)
            {
                TempData["error"] = "Invalid rating selected!";
                return View();
            }
            if (EXErating.ToString() == null)
            {
                TempData["error"] = "Invalid rating selected!";
                return View();
            }
            if (CLOSURErating.ToString() == null)
            {
                TempData["error"] = "Invalid rating selected!";
                return View();
            }
            if (ASSESSMENTrating.ToString() == null)
            {
                TempData["error"] = "Invalid rating selected!";
                return View();
            }
            if (PRESENCErating.ToString() == null)
            {
                TempData["error"] = "Invalid rating selected!";
                return View();
            }
            if (ENVIRONMENTrating.ToString() == null)
            {
                TempData["error"] = "Invalid rating selected!";
                return View();
            }

            #endregion

            #region Check if Commentary is not null
            if (string.IsNullOrWhiteSpace(ComSectionAtoD))
            {
                TempData["error"] = "Invalid Commentary!";
                return View();
            }

            if (string.IsNullOrWhiteSpace(ComSectionE))
            {
                TempData["error"] = "Invalid Commentary!";
                return View();
            }

            if (string.IsNullOrWhiteSpace(ComIntro))
            {
                TempData["error"] = "Invalid Commentary!";
                return View();
            }

            if (string.IsNullOrWhiteSpace(ComTeaching))
            {
                TempData["error"] = "Invalid Commentary!";
                return View();
            }

            if (string.IsNullOrWhiteSpace(ComClosure))
            {
                TempData["error"] = "Invalid Commentary!";
                return View();
            }

            if (string.IsNullOrWhiteSpace(ComAssessment))
            {
                TempData["error"] = "Invalid Commentary!";
                return View();
            }

            if (string.IsNullOrWhiteSpace(ComPresence))
            {
                TempData["error"] = "Invalid Commentary!";
                return View();
            }

            if (string.IsNullOrWhiteSpace(ComEnvironment))
            {
                TempData["error"] = "Invalid Commentary!";
                return View();
            }
            #endregion

            #endregion

            //general info of grading
            grading.Grade = Convert.ToInt32(Grade);
            grading.Subject = Subject;
            grading.Topic = Topic;

            //grading marks allocations
            //Get Current Marks Objects
            Planning gPlanning = _context.Plannings.Where(x => x.GradingNumber == grading.Number).SingleOrDefault();
            Execution gExecution = _context.Executions.Where(x => x.GradingNumber == grading.Number).SingleOrDefault();
            Overall gOverall = _context.Overalls.Where(x => x.GradingNumber == grading.Number).SingleOrDefault();
            //Check that they are not null

            if (gPlanning == null || gExecution == null || gOverall == null)
            {
                TempData["error"] = "An error has occurred! Please contact system admin if this persists!";
                return View(grading);
            }

            //Set new Marks Values
            //Planning
            gPlanning.SectionAtoD = Convert.ToInt32(Arating);
            gPlanning.SectionE = Convert.ToInt32(Erating);

            //Execution
            gExecution.Intro = Convert.ToInt32(INTROrating);
            gExecution.Teaching = Convert.ToInt32(EXErating);
            gExecution.Closure = Convert.ToInt32(CLOSURErating);
            gExecution.Assessment = Convert.ToInt32(ASSESSMENTrating);

            //Overall
            gOverall.Presence = Convert.ToInt32(PRESENCErating);
            gOverall.Environment = Convert.ToInt32(ENVIRONMENTrating);

            //set new comments values
            //get existing comments
            Commentary SectionAtoD = _context.Commentaries.Where(x => x.GradingNumber == grading.Number && x.Criteria == "SectionAtoD").SingleOrDefault();
            Commentary SectionE = _context.Commentaries.Where(x => x.GradingNumber == grading.Number && x.Criteria == "SectionE").SingleOrDefault();
            Commentary Intro = _context.Commentaries.Where(x => x.GradingNumber == grading.Number && x.Criteria == "Intro").SingleOrDefault();
            Commentary Teaching = _context.Commentaries.Where(x => x.GradingNumber == grading.Number && x.Criteria == "Teaching").SingleOrDefault();
            Commentary Closure = _context.Commentaries.Where(x => x.GradingNumber == grading.Number && x.Criteria == "Closure").SingleOrDefault();
            Commentary Assessment = _context.Commentaries.Where(x => x.GradingNumber == grading.Number && x.Criteria == "Assessment").SingleOrDefault();
            Commentary Presence = _context.Commentaries.Where(x => x.GradingNumber == grading.Number && x.Criteria == "Presence").SingleOrDefault();
            Commentary Environment = _context.Commentaries.Where(x => x.GradingNumber == grading.Number && x.Criteria == "Environment").SingleOrDefault();

            if (SectionAtoD == null || SectionE == null)
            {
                TempData["error"] = "An error has occurred! Please contact system admin if this persists!";
                return View(grading);
            }

            if (Intro == null || Teaching == null || Closure == null || Assessment == null)
            {
                TempData["error"] = "An error has occurred! Please contact system admin if this persists!";
                return View(grading);
            }

            if (Presence == null || Environment == null)
            {
                TempData["error"] = "An error has occurred! Please contact system admin if this persists!";
                return View(grading);
            }

            //new comments
            SectionAtoD.Comment = ComSectionAtoD;
            SectionE.Comment = ComSectionE;
            Intro.Comment = ComIntro;
            Teaching.Comment = ComTeaching;
            Closure.Comment = ComClosure;
            Assessment.Comment = ComAssessment;
            Presence.Comment = ComPresence;
            Environment.Comment = ComEnvironment;

            //Update comments in DB
            _context.Commentaries.Update(SectionAtoD);
            _context.Commentaries.Update(SectionE);
            _context.Commentaries.Update(Intro);
            _context.Commentaries.Update(Teaching);
            _context.Commentaries.Update(Closure);
            _context.Commentaries.Update(Assessment);
            _context.Commentaries.Update(Presence);
            _context.Commentaries.Update(Environment);

            //update grading in DB
            _context.Gradings.Update(grading);

            await _context.SaveChangesAsync();

            TempData["success"] = "Grading updated successfully!";
            return RedirectToAction("Details", "Gradings", new { id = id });
        }

        // GET: Gradings/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.Gradings == null)
            {
                return NotFound();
            }

            var grading = await _context.Gradings
                .Include(g => g.StudentNavigation)
                .Include(g => g.TeacherNavigation)
                .FirstOrDefaultAsync(m => m.Number == id);
            if (grading == null)
            {
                return NotFound();
            }

            return View(grading);
        }

        // POST: Gradings/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.Gradings == null)
            {
                return Problem("Entity set 'XISD_POEContext.Gradings'  is null.");
            }
            var grading = await _context.Gradings.FindAsync(id);
            if (grading != null)
            {
                _context.Gradings.Remove(grading);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool GradingExists(int id)
        {
            return _context.Gradings.Any(e => e.Number == id);
        }
    }
}
