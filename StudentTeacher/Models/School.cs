using System;
using System.Collections.Generic;

namespace StudentTeacher.Models
{
    public partial class School
    {
        public School()
        {
            Teachers = new HashSet<Teacher>();
        }

        public string Code { get; set; } = null!;
        public string? Province { get; set; }
        public string? City { get; set; }
        public string? Campus { get; set; }

        public virtual Campus? CampusNavigation { get; set; }
        public virtual ICollection<Teacher> Teachers { get; set; }
    }
}
