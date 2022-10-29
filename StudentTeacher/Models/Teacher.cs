using System;
using System.Collections.Generic;

namespace StudentTeacher.Models
{
    public partial class Teacher
    {
        public string Number { get; set; } = null!;
        public string FirstName { get; set; } = null!;
        public string LastName { get; set; } = null!;
        public string School { get; set; } = null!;
        public string Email { get; set; } = null!;

        public virtual User EmailNavigation { get; set; } = null!;
        public virtual School SchoolNavigation { get; set; } = null!;
    }
}
