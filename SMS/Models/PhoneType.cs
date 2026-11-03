using System;
using System.Collections.Generic;

namespace CodeflexSystems.SMS.Models
{
    public partial class PhoneType
    {
        public PhoneType()
        {
            this.GuardianPhones = new List<GuardianPhone>();
            this.SchoolPhones = new List<SchoolPhone>();
            this.StudentPhones = new List<StudentPhone>();
        }

        public Guid PhoneTypeId { get; set; }
        public string PhoneTypeName { get; set; }
        public DateTime CreatedDate { get; set; }
        public Nullable<Guid> CreatedBy { get; set; }
        public DateTime ModifiedDate { get; set; }
        public Nullable<Guid> ModifiedBy { get; set; }
        public byte[] RowVersion { get; set; }

        public virtual ICollection<GuardianPhone> GuardianPhones { get; set; }
        public virtual ICollection<SchoolPhone> SchoolPhones { get; set; }
        public virtual ICollection<StudentPhone> StudentPhones { get; set; }
    }
}
