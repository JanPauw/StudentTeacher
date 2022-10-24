using System;
using System.Collections.Generic;

namespace StudentTeacher.Models
{
    public partial class StudentModule
    {
        public int Id { get; set; }
        public string Student { get; set; } = null!;
        public string Module { get; set; } = null!;

        public virtual Module ModuleNavigation { get; set; } = null!;
        public virtual Student StudentNavigation { get; set; } = null!;
    }
}
