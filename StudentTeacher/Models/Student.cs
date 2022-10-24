using System;
using System.Collections.Generic;

namespace StudentTeacher.Models
{
    public partial class Student
    {
        public Student()
        {
            StudentModules = new HashSet<StudentModule>();
        }

        public string Number { get; set; } = null!;
        public string FirstName { get; set; } = null!;
        public string LastName { get; set; } = null!;

        public virtual ICollection<StudentModule> StudentModules { get; set; }
    }
}
