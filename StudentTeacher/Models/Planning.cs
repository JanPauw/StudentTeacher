using System;
using System.Collections.Generic;

namespace StudentTeacher.Models
{
    public partial class Planning
    {
        public int Number { get; set; }
        public int SectionAtoD { get; set; }
        public int SectionE { get; set; }
        public int GradingNumber { get; set; }

        public virtual Grading GradingNumberNavigation { get; set; } = null!;
    }
}
