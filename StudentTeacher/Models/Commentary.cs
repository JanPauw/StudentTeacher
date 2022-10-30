using System;
using System.Collections.Generic;

namespace StudentTeacher.Models
{
    public partial class Commentary
    {
        public int Id { get; set; }
        public string Criteria { get; set; } = null!;
        public int GradingNumber { get; set; }

        public virtual Grading GradingNumberNavigation { get; set; } = null!;
    }
}
