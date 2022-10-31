using System;
using System.Collections.Generic;

namespace StudentTeacher.Models
{
    public partial class Grading
    {
        public Grading()
        {
            Commentaries = new HashSet<Commentary>();
            Executions = new HashSet<Execution>();
            Overalls = new HashSet<Overall>();
            Plannings = new HashSet<Planning>();
        }

        public int Number { get; set; }
        public string Student { get; set; } = null!;
        public string Teacher { get; set; } = null!;
        public int YearOfStudy { get; set; }
        public DateTime Date { get; set; }
        public int Grade { get; set; }
        public string Topic { get; set; } = null!;
        public string Subject { get; set; } = null!;

        public virtual Student StudentNavigation { get; set; } = null!;
        public virtual Teacher TeacherNavigation { get; set; } = null!;
        public virtual ICollection<Commentary> Commentaries { get; set; }
        public virtual ICollection<Execution> Executions { get; set; }
        public virtual ICollection<Overall> Overalls { get; set; }
        public virtual ICollection<Planning> Plannings { get; set; }
    }
}
