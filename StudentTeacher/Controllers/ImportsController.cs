using ClosedXML.Excel;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using StudentTeacher.Models;
using System.Data;
using IHostingEnvironment = Microsoft.AspNetCore.Hosting.IWebHostEnvironment;

namespace StudentTeacher.Controllers
{
    public class ImportsController : Controller
    {
        private readonly XISD_POEContext _context;
        private IHostingEnvironment _env;

        public ImportsController(XISD_POEContext context, IHostingEnvironment env)
        {
            _context = context;
            _env = env;
        }

        #region Generated Action Results
        // GET: ImportsController
        public ActionResult Index()
        {
            List<Campus> CampusList = _context.Campuses.ToList();
            ViewBag.Campuses = CampusList;
            return View();
        }

        // GET: ImportsController/Details/5
        public ActionResult Details(int id)
        {
            return View();
        }

        // GET: ImportsController/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: ImportsController/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(IFormCollection collection)
        {
            try
            {
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }

        // GET: ImportsController/Edit/5
        public ActionResult Edit(int id)
        {
            return View();
        }

        // POST: ImportsController/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(int id, IFormCollection collection)
        {
            try
            {
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }

        // GET: ImportsController/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: ImportsController/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(int id, IFormCollection collection)
        {
            try
            {
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }
        #endregion

        List<Import> listImports = new List<Import>();

        public ActionResult UploadData()
        {
            return View();
        }

        //Importing Excel
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UploadData(IFormFile file, string Campus)
        {
            //Check that Campus is valid
            Campus c = _context.Campuses.Find(Campus);
            if (c == null)
            {
                TempData["error"] = "Invalid Campus selected!";
                return RedirectToAction("Index", "Students");
            }

            try
            {
                listImports.Clear();

                var fileextension = Path.GetExtension(file.FileName);

                var filename = Guid.NewGuid().ToString() + fileextension;
                var filepath = Path.Combine(this._env.WebRootPath, "files", filename);
                using (FileStream fs = System.IO.File.Create(filepath))
                {
                    file.CopyTo(fs);
                }
                int rowno = 1;
                XLWorkbook workbook = XLWorkbook.OpenFromTemplate(filepath);
                var sheets = workbook.Worksheets.First();
                var rows = sheets.Rows().ToList();
                foreach (var row in rows)
                {
                    if (rowno != 1)
                    {
                        var test = row.Cell(1).Value.ToString();
                        if (string.IsNullOrWhiteSpace(test) || string.IsNullOrEmpty(test))
                        {
                            break;
                        }
                        Import import = new Import();
                        import.YearOfStudy = int.Parse(row.Cell(1).Value.ToString());
                        import.StudentSurname = row.Cell(2).Value.ToString();
                        import.StudentName = row.Cell(3).Value.ToString();
                        import.StudentNumber = row.Cell(4).Value.ToString();
                        import.EmailAddress = row.Cell(5).Value.ToString();
                        import.ContactNumber = row.Cell(6).Value.ToString();
                        import.Qualification = row.Cell(7).Value.ToString();
                        import.HomeLanguage = row.Cell(8).Value.ToString();
                        import.TeachingPlacement1 = row.Cell(9).Value.ToString();
                        import.Quintile1 = row.Cell(10).Value.ToString();
                        import.TeachingPlacement2 = row.Cell(11).Value.ToString();
                        import.Quintile2 = row.Cell(12).Value.ToString();
                        import.TeachingPlacement3 = row.Cell(13).Value.ToString();
                        import.Quintile3 = row.Cell(14).Value.ToString();
                        import.TeachingPlacement4 = row.Cell(15).Value.ToString();
                        import.Quintile4 = row.Cell(16).Value.ToString();

                        listImports.Add(import);
                    }
                    else
                    {
                        rowno = 2;
                    }
                }

                #region Save Imports to Database
                foreach (var item in listImports)
                {
                    //Student: Create New or Update Existing
                    Student student = _context.Students.Find(item.StudentNumber);
                    if (student == null) //Create new student
                    {
                        student = new Student();

                        student.Number = item.StudentNumber;
                        student.FirstName = item.StudentName;
                        student.LastName = item.StudentSurname;
                        student.Qualification = item.Qualification;
                        student.YearOfStudy = item.YearOfStudy;
                        student.Campus = c.Code;
                        student.CampusNavigation = c;

                        _context.Students.Add(student);
                    }
                    else
                    {
                        student.FirstName = item.StudentName;
                        student.LastName = item.StudentSurname;
                        student.Qualification = item.Qualification;
                        student.YearOfStudy = item.YearOfStudy;
                        student.Campus = c.Code;
                        student.CampusNavigation = c;

                        _context.Students.Update(student);
                    }

                    await _context.SaveChangesAsync();

                    //Schools: Create New if Non-Existent
                    School Placement1 = _context.Schools.Where(x => x.Name.Equals(item.TeachingPlacement1)).SingleOrDefault();
                    School Placement2 = _context.Schools.Where(x => x.Name.Equals(item.TeachingPlacement2)).SingleOrDefault();
                    School Placement3 = _context.Schools.Where(x => x.Name.Equals(item.TeachingPlacement3)).SingleOrDefault();
                    School Placement4 = _context.Schools.Where(x => x.Name.Equals(item.TeachingPlacement4)).SingleOrDefault();

                    #region Check if Schools Exist ?? Create new
                    if (Placement1 == null)//Create new School
                    {
                        Placement1 = new School();
                        Placement1.Name = item.TeachingPlacement1;
                        Placement1.Code = GenerateSchoolCode(Placement1.Name);
                        Placement1.Campus = c.Code;
                        Placement1.CampusNavigation = c;
                        Placement1.Quintile = item.Quintile1;
                        _context.Schools.Add(Placement1);
                    }
                    else
                    {
                        Placement1.Quintile = item.Quintile1;
                        _context.Schools.Update(Placement1);
                    }
                    await _context.SaveChangesAsync();

                    if (Placement2 == null)//Create new School
                    {
                        Placement2 = new School();
                        Placement2.Name = item.TeachingPlacement2;
                        Placement2.Code = GenerateSchoolCode(Placement2.Name);
                        Placement2.Campus = c.Code;
                        Placement2.CampusNavigation = c;
                        Placement2.Quintile = item.Quintile2;
                        _context.Schools.Add(Placement2);
                    }
                    else
                    {
                        Placement2.Quintile = item.Quintile2;
                        _context.Schools.Update(Placement2);
                    }
                    await _context.SaveChangesAsync();

                    if (Placement3 == null)//Create new School
                    {
                        Placement3 = new School();
                        Placement3.Name = item.TeachingPlacement3;
                        Placement3.Code = GenerateSchoolCode(Placement3.Name);
                        Placement3.Campus = c.Code;
                        Placement3.CampusNavigation = c;
                        Placement3.Quintile = item.Quintile3;
                        _context.Schools.Add(Placement3);
                    }
                    else
                    {
                        Placement3.Quintile = item.Quintile3;
                        _context.Schools.Update(Placement3);
                    }
                    await _context.SaveChangesAsync();

                    if (Placement4 == null)//Create new School
                    {
                        Placement4 = new School();
                        Placement4.Name = item.TeachingPlacement4;
                        Placement4.Code = GenerateSchoolCode(Placement4.Name);
                        Placement4.Campus = c.Code;
                        Placement4.CampusNavigation = c;
                        Placement4.Quintile = item.Quintile4;
                        _context.Schools.Add(Placement4);
                    }
                    else
                    {
                        Placement4.Quintile = item.Quintile4;
                        _context.Schools.Update(Placement4);
                    }
                    await _context.SaveChangesAsync();
                    #endregion

                    //StudentSchools: Create New or Update Existing
                    StudentSchool Year1 = _context.StudentSchools.Where(x => x.Student.Equals(student.Number) && x.PlacementYear == 1).SingleOrDefault();
                    StudentSchool Year2 = _context.StudentSchools.Where(x => x.Student.Equals(student.Number) && x.PlacementYear == 2).SingleOrDefault();
                    StudentSchool Year3 = _context.StudentSchools.Where(x => x.Student.Equals(student.Number) && x.PlacementYear == 3).SingleOrDefault();
                    StudentSchool Year4 = _context.StudentSchools.Where(x => x.Student.Equals(student.Number) && x.PlacementYear == 4).SingleOrDefault();

                    #region Check if StudentSchools Exists ?? Create New
                    if (Year1 == null)
                    {
                        Year1 = new StudentSchool();
                        Year1.Student = student.Number;
                        Year1.School = Placement1.Code;
                        Year1.PlacementYear = 1;
                        Year1.StudentNavigation = student;
                        Year1.SchoolNavigation = Placement1;
                        _context.StudentSchools.Add(Year1);
                    }
                    else
                    {
                        Year1.School = Placement1.Code;
                        Year1.SchoolNavigation = Placement1;
                        _context.StudentSchools.Update(Year1);
                    }
                    await _context.SaveChangesAsync();

                    if (Year2 == null)
                    {
                        Year2 = new StudentSchool();
                        Year2.Student = student.Number;
                        Year2.School = Placement2.Code;
                        Year2.PlacementYear = 2;
                        Year2.StudentNavigation = student;
                        Year2.SchoolNavigation = Placement2;
                        _context.StudentSchools.Add(Year2);
                    }
                    else
                    {
                        Year2.School = Placement2.Code;
                        Year2.SchoolNavigation = Placement2;
                        _context.StudentSchools.Update(Year2);
                    }
                    await _context.SaveChangesAsync();

                    if (Year3 == null)
                    {
                        Year3 = new StudentSchool();
                        Year3.Student = student.Number;
                        Year3.School = Placement3.Code;
                        Year3.PlacementYear = 3;
                        Year3.StudentNavigation = student;
                        Year3.SchoolNavigation = Placement3;
                        _context.StudentSchools.Add(Year3);
                    }
                    else
                    {
                        Year3.School = Placement3.Code;
                        Year3.SchoolNavigation = Placement3;
                        _context.StudentSchools.Update(Year3);
                    }
                    await _context.SaveChangesAsync();

                    if (Year4 == null)
                    {
                        Year4 = new StudentSchool();
                        Year4.Student = student.Number;
                        Year4.School = Placement4.Code;
                        Year4.PlacementYear = 4;
                        Year4.StudentNavigation = student;
                        Year4.SchoolNavigation = Placement4;
                        _context.StudentSchools.Add(Year4);
                    }
                    else
                    {
                        Year4.School = Placement4.Code;
                        Year4.SchoolNavigation = Placement4;
                        _context.StudentSchools.Update(Year4);
                    }
                    await _context.SaveChangesAsync();
                    #endregion
                }
                #endregion

                TempData["success"] = "Spreadsheet imported successfully!";
                return RedirectToAction("Index", "Students");
            }
            catch (Exception e)
            {
                TempData["error"] = "Importing Failed!";
                return RedirectToAction("Index", "Students");
            }
        }

        public ActionResult Export()
        {
            return View();
        }

        //Exporting to Excel
        [HttpPost]
        public async Task<FileResult> ExportImport()
        {
            DataTable dt = new DataTable("Grid");
            dt.Columns.AddRange(new DataColumn[16] { new DataColumn("Year of study within degree"),
                                            new DataColumn("Student Surname"),
                                            new DataColumn("Student Name"),
                                            new DataColumn("Student Number"),
                                            new DataColumn("Email Address"),
                                            new DataColumn("Contact number"),
                                            new DataColumn("Qualification (Foundation Phase or Intermediate Phase)"),
                                            new DataColumn("Home Language"),
                                            new DataColumn("Teaching Placment Year 1 (School Name)"),
                                            new DataColumn("YEAR 1 QUINTILE CATEGORY "),
                                            new DataColumn("Teaching Placment Year 2 (School Name)"),
                                            new DataColumn("YEAR 2 QUINTILE CATEGORY "),
                                            new DataColumn("Teaching Placment Year 3 (School Name)"),
                                            new DataColumn("YEAR 3 QUINTILE CATEGORY "),
                                            new DataColumn("Teaching Placment Year 4 (School Name)"),
                                            new DataColumn("YEAR 4 QUINTILE CATEGORY")
            });

            List<Student> students = _context.Students.ToList();

            foreach (var student in students)
            {
                //get student placements
                StudentSchool Year1 = await _context.StudentSchools.Where(x => x.Student == student.Number && x.PlacementYear == 1).SingleOrDefaultAsync();
                StudentSchool Year2 = await _context.StudentSchools.Where(x => x.Student == student.Number && x.PlacementYear == 2).SingleOrDefaultAsync();
                StudentSchool Year3 = await _context.StudentSchools.Where(x => x.Student == student.Number && x.PlacementYear == 3).SingleOrDefaultAsync();
                StudentSchool Year4 = await _context.StudentSchools.Where(x => x.Student == student.Number && x.PlacementYear == 4).SingleOrDefaultAsync();

                //get schools
                School school1 = await _context.Schools.FindAsync(Year1.School);
                School school2 = await _context.Schools.FindAsync(Year2.School);
                School school3 = await _context.Schools.FindAsync(Year3.School);
                School school4 = await _context.Schools.FindAsync(Year4.School);

                dt.Rows.Add(
                    student.YearOfStudy,                        //Year of Study
                    student.LastName,                           //Student Surname
                    student.FirstName,                          //Student Name
                    student.Number,                             //Student Number
                    student.Number + "@vcconnect.edu.za",       //Student Email
                    "N/A",                                      //Student Phone
                    student.Qualification,                      //Qualification
                    "N/A",                                      //Home Language
                    school1.Name,                               //Teaching Placement Year 1
                    school1.Quintile,                           //Quintile Category
                    school2.Name,                               //Teaching Placement Year 1
                    school2.Quintile,                           //Quintile Category
                    school3.Name,                               //Teaching Placement Year 1
                    school3.Quintile,                           //Quintile Category
                    school4.Name,                               //Teaching Placement Year 1
                    school4.Quintile                            //Quintile Category
                    );
            }

            using (XLWorkbook wb = new XLWorkbook())
            {
                wb.Worksheets.Add(dt);
                using (MemoryStream stream = new MemoryStream())
                {
                    wb.SaveAs(stream);
                    return File(stream.ToArray(), "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "BulkImport.xlsx");
                }
            }
        }

        [HttpPost]
        public async Task<FileResult> ExportStudentGradings(string StudentNumber, string SelectedYear)
        {
            if (string.IsNullOrEmpty(StudentNumber) || string.IsNullOrEmpty(SelectedYear))
            {
                return null;
            }

            //get student
            Student student = await _context.Students.FindAsync(StudentNumber);
            if (student == null)
            {
                return null;
            }

            //get gradings
            List<Grading> gradings = await _context.Gradings.Where(x => x.Student == student.Number && "" + x.YearOfStudy == SelectedYear).ToListAsync();

            DataTable dt = new DataTable("Grid");
            dt.Columns.AddRange(new DataColumn[24] { new DataColumn("Date"),
                                            new DataColumn("Year of Study"),
                                            new DataColumn("Teacher"),
                                            new DataColumn("Student Number"),
                                            new DataColumn("Student Name and Surname"),
                                            new DataColumn("Grade"),
                                            new DataColumn("Subject"),
                                            new DataColumn("Topic"),
                                            new DataColumn("Sections A to D (of lesson plan)"),
                                            new DataColumn("Sections A to D Commentary"),
                                            new DataColumn("Section E"),
                                            new DataColumn("Section E Commentary"),
                                            new DataColumn("Introductory Phase"),
                                            new DataColumn("Introductory Phase Commentary"),
                                            new DataColumn("Teaching and Learning Phase"),
                                            new DataColumn("Teaching and Learning Phase Commentary"),
                                            new DataColumn("Closure Phase"),
                                            new DataColumn("Closure Phase Commentary"),
                                            new DataColumn("Assessment"),
                                            new DataColumn("Assessment Commentary"),
                                            new DataColumn("Classroom Presence"),
                                            new DataColumn("Classroom Presence Commentary"),
                                            new DataColumn("Learning Environment"),
                                            new DataColumn("Learning Environment Commentary")
            });

            foreach (var grading in gradings)
            {
                #region get details from other classes
                //get teacher
                Teacher teacher = await _context.Teachers.FindAsync(grading.Teacher);
                //Commentary
                List<Commentary> comments = await _context.Commentaries.Where(x => x.GradingNumber == grading.Number).ToListAsync();
                //get marks
                Planning planning = await _context.Plannings.Where(x => x.GradingNumber == grading.Number).SingleOrDefaultAsync();
                Execution execution = await _context.Executions.Where(x => x.GradingNumber == grading.Number).SingleOrDefaultAsync();
                Overall overall = await _context.Overalls.Where(x => x.GradingNumber == grading.Number).SingleOrDefaultAsync();
                #endregion

                dt.Rows.Add(
                    grading.Date,
                    grading.YearOfStudy,
                    teacher.FirstName + " " + teacher.LastName,
                    student.Number,
                    student.FirstName + " " + student.LastName,
                    grading.Grade,
                    grading.Subject,
                    grading.Topic,
                    planning.SectionAtoD,
                    comments.Where(x => x.Criteria.Equals("SectionAtoD")).SingleOrDefault().Comment,
                    planning.SectionE,
                    comments.Where(x => x.Criteria.Equals("SectionE")).SingleOrDefault().Comment,
                    execution.Intro,
                    comments.Where(x => x.Criteria.Equals("Intro")).SingleOrDefault().Comment,
                    execution.Teaching,
                    comments.Where(x => x.Criteria.Equals("Teaching")).SingleOrDefault().Comment,
                    execution.Closure,
                    comments.Where(x => x.Criteria.Equals("Closure")).SingleOrDefault().Comment,
                    execution.Assessment,
                    comments.Where(x => x.Criteria.Equals("Assessment")).SingleOrDefault().Comment,
                    overall.Presence,
                    comments.Where(x => x.Criteria.Equals("Presence")).SingleOrDefault().Comment,
                    overall.Environment,
                    comments.Where(x => x.Criteria.Equals("Environment")).SingleOrDefault().Comment
                    ) ;
            }

            using (XLWorkbook wb = new XLWorkbook())
            {
                wb.Worksheets.Add(dt);
                using (MemoryStream stream = new MemoryStream())
                {
                    wb.SaveAs(stream);
                    return File(stream.ToArray(), "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", student.Number + "_Gradings.xlsx");
                }
            }
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
