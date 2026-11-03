using System;
using System.Collections.Generic;

namespace CodeflexSystems.SMS.Models
{
    public partial class EmailType
    {
        public EmailType()
        {
            this.GuardianEmails = new List<GuardianEmail>();
            this.SchoolEmails = new List<SchoolEmail>();
            this.StudentEmails = new List<StudentEmail>();
        }

        public Guid EmailTypeId { get; set; }
        public string EmailTypeName { get; set; }
        public DateTime CreatedDate { get; set; }
        public Nullable<Guid> CreatedBy { get; set; }
        public DateTime ModifiedDate { get; set; }
        public Nullable<Guid> ModifiedBy { get; set; }
        public byte[] RowVersion { get; set; }

        public virtual ICollection<GuardianEmail> GuardianEmails { get; set; }
        public virtual ICollection<SchoolEmail> SchoolEmails { get; set; }
        public virtual ICollection<StudentEmail> StudentEmails { get; set; }
    }
}
