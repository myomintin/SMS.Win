using System;
using System.Collections.Generic;

namespace SMS.Models
{
    public partial class AcademicYear
    {
        public AcademicYear()
        {
            this.Courses = new List<Course>();
        }

        public Guid AcademicYearId { get; set; }
        public string AcademicYearName { get; set; }
        public bool Active { get; set; }
        public string Description { get; set; }
        public Guid SchoolId { get; set; }
        public DateTime CreatedDate { get; set; }
        public Nullable<Guid> CreatedBy { get; set; }
        public DateTime ModifiedDate { get; set; }
        public Nullable<Guid> ModifiedBy { get; set; }
        public byte[] RowVersion { get; set; }

        public virtual School School { get; set; }
        public virtual ICollection<Course> Courses { get; set; }
    }
}
