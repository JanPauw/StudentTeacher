using System;
using System.Collections.Generic;

namespace StudentTeacher.Models
{
    public partial class Execution
    {
        public int Number { get; set; }
        public int Intro { get; set; }
        public int Teaching { get; set; }
        public int Closure { get; set; }
        public int Assessment { get; set; }
        public int GradingNumber { get; set; }

        public virtual Grading GradingNumberNavigation { get; set; } = null!;
    }
}
