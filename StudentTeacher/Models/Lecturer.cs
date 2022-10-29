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
        public string Campus { get; set; } = null!;

        public virtual Campus CampusNavigation { get; set; } = null!;
        public virtual User EmailNavigation { get; set; } = null!;
    }
}
