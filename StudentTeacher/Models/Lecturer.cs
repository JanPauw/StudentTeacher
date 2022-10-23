using System;
using System.Collections.Generic;

namespace StudentTeacher.Models
{
    public partial class Lecturer
    {
        public string Number { get; set; } = null!;
        public string FirstName { get; set; } = null!;
        public string LastName { get; set; } = null!;
        public string Email { get; set; } = null!;
        public string? Campus { get; set; }

        public virtual Campus? CampusNavigation { get; set; }
        public virtual User EmailNavigation { get; set; } = null!;
    }
}
