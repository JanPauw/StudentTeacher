using System;
using System.Collections.Generic;

namespace StudentTeacher.Models
{
    public partial class Module
    {
        public Module()
        {
            StudentModules = new HashSet<StudentModule>();
        }

        public string Number { get; set; } = null!;
        public string Name { get; set; } = null!;

        public virtual ICollection<StudentModule> StudentModules { get; set; }
    }
}
