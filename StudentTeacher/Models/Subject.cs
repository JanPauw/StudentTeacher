using System;
using System.Collections.Generic;

namespace StudentTeacher.Models
{
    public partial class Subject
    {
        public int Id { get; set; }
        public string Subject1 { get; set; } = null!;
        public string YearOfStudy { get; set; } = null!;
    }
}
