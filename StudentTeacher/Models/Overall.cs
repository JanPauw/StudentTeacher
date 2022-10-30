using System;
using System.Collections.Generic;

namespace StudentTeacher.Models
{
    public partial class Overall
    {
        public int Number { get; set; }
        public int Presence { get; set; }
        public int Environment { get; set; }
        public int GradingNumber { get; set; }

        public virtual Grading GradingNumberNavigation { get; set; } = null!;
    }
}
