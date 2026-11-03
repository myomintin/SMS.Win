using System;
using System.Collections.Generic;

namespace CodeflexSystems.SMS.Models
{
    public partial class School
    {
        public School()
        {
            this.AcademicYears = new List<AcademicYear>();
            this.SchoolEmails = new List<SchoolEmail>();
            this.SchoolPhones = new List<SchoolPhone>();
        }

        public Guid SchoolId { get; set; }
        public string SchoolName { get; set; }
        public string Motto { get; set; }
        public bool Active { get; set; }
        public string Description { get; set; }
        public Guid AddressId { get; set; }
        public DateTime CreatedDate { get; set; }
        public Nullable<Guid> CreatedBy { get; set; }
        public DateTime ModifiedDate { get; set; }
        public Nullable<Guid> ModifiedBy { get; set; }
        public byte[] RowVersion { get; set; }

        public virtual Address Address { get; set; }
        public virtual ICollection<AcademicYear> AcademicYears { get; set; }
        public virtual ICollection<SchoolEmail> SchoolEmails { get; set; }
        public virtual ICollection<SchoolPhone> SchoolPhones { get; set; }
    }
}
