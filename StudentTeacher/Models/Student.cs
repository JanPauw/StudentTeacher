using System;
using System.Collections.Generic;

namespace StudentTeacher.Models
{
    public partial class Student
    {
        public Student()
        {
            Gradings = new HashSet<Grading>();
            StudentSchools = new HashSet<StudentSchool>();
        }

        public string Number { get; set; } = null!;
        public string FirstName { get; set; } = null!;
        public string LastName { get; set; } = null!;
        public string? Qualification { get; set; }
        public int YearOfStudy { get; set; }
        public string Campus { get; set; } = null!;

        public virtual Campus CampusNavigation { get; set; } = null!;
        public virtual ICollection<Grading> Gradings { get; set; }
        public virtual ICollection<StudentSchool> StudentSchools { get; set; }
    }
}
