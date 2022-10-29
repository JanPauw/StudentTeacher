using System;
using System.Collections.Generic;

namespace StudentTeacher.Models
{
    public partial class StudentSchool
    {
        public int Id { get; set; }
        public string Student { get; set; } = null!;
        public string School { get; set; } = null!;
        public int PlacementYear { get; set; }

        public virtual School SchoolNavigation { get; set; } = null!;
        public virtual Student StudentNavigation { get; set; } = null!;
    }
}
