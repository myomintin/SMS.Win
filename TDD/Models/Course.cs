using System;
using System.Collections.Generic;

namespace SMS.Models
{
    public partial class Course
    {
        public Course()
        {
            this.Classes = new List<Class>();
        }

        public Guid CourseId { get; set; }
        public string CourseName { get; set; }
        public string Description { get; set; }
        public Guid AcademicYearId { get; set; }
        public DateTime CreatedDate { get; set; }
        public Nullable<Guid> CreatedBy { get; set; }
        public DateTime ModifiedDate { get; set; }
        public Nullable<Guid> ModifiedBy { get; set; }
        public byte[] RowVersion { get; set; }

        public virtual AcademicYear AcademicYear { get; set; }
        public virtual ICollection<Class> Classes { get; set; }
    }
}
