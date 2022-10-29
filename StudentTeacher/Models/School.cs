using System;
using System.Collections.Generic;

namespace StudentTeacher.Models
{
    public partial class School
    {
        public School()
        {
            StudentSchools = new HashSet<StudentSchool>();
            Teachers = new HashSet<Teacher>();
        }

        public string Code { get; set; } = null!;
        public string Name { get; set; } = null!;
        public string Campus { get; set; } = null!;
        public string Quintile { get; set; } = null!;

        public virtual Campus CampusNavigation { get; set; } = null!;
        public virtual ICollection<StudentSchool> StudentSchools { get; set; }
        public virtual ICollection<Teacher> Teachers { get; set; }
    }
}
