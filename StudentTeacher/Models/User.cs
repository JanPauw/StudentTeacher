using System;
using System.Collections.Generic;

namespace StudentTeacher.Models
{
    public partial class User
    {
        public User()
        {
            Lecturers = new HashSet<Lecturer>();
            Teachers = new HashSet<Teacher>();
        }

        public string Email { get; set; } = null!;
        public string Password { get; set; } = null!;
        public string Type { get; set; } = null!;
        public string Role { get; set; } = null!;

        public virtual ICollection<Lecturer> Lecturers { get; set; }
        public virtual ICollection<Teacher> Teachers { get; set; }
    }
}
