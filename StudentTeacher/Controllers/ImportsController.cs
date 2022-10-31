using ClosedXML.Excel;
using Microsoft.AspNetCore.Mvc;
using StudentTeacher.Models;
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

                TempData["success"] = "Surely it worked?";
                return RedirectToAction("Index", "Students");
            }
            catch (Exception e)
            {
                TempData["error"] = "Importing Failed!";
                return RedirectToAction("Index", "Students");
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
