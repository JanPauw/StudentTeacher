using System;
using System.Collections.Generic;

namespace StudentTeacher.Models
{
    public partial class Campus
    {
        public Campus()
        {
            Lecturers = new HashSet<Lecturer>();
            Schools = new HashSet<School>();
            Students = new HashSet<Student>();
        }

        public string Code { get; set; } = null!;
        public string Province { get; set; } = null!;
        public string City { get; set; } = null!;
        public string Name { get; set; } = null!;

        public virtual ICollection<Lecturer> Lecturers { get; set; }
        public virtual ICollection<School> Schools { get; set; }
        public virtual ICollection<Student> Students { get; set; }
    }
}
