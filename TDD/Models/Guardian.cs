using System;
using System.Collections.Generic;

namespace SMS.Models
{
    public partial class Guardian
    {
        public Guardian()
        {
            this.GuardianAddresses = new List<GuardianAddress>();
            this.GuardianEmails = new List<GuardianEmail>();
            this.GuardianPhones = new List<GuardianPhone>();
            this.StudentGuardians = new List<StudentGuardian>();
        }

        public Guid GuardianId { get; set; }
        public string Title { get; set; }
        public string GuardianName { get; set; }
        public bool Gender { get; set; }
        public Nullable<DateTime> BirthDate { get; set; }
        public string Nationality { get; set; }
        public string NIRC { get; set; }
        public string Education { get; set; }
        public string Employment { get; set; }
        public string Designation { get; set; }
        public string Passport { get; set; }
        public string Visa { get; set; }
        public string Description { get; set; }
        public Nullable<Guid> BirthCountryId { get; set; }
        public Nullable<Guid> ReligionId { get; set; }
        public DateTime CreatedDate { get; set; }
        public Nullable<Guid> CreatedBy { get; set; }
        public DateTime ModifiedDate { get; set; }
        public Nullable<Guid> ModifiedBy { get; set; }
        public byte[] RowVersion { get; set; }

        public virtual Country BirthCountry { get; set; }
        public virtual Religion Religion { get; set; }
        public virtual GuardianPhoto Photo { get; set; }
        public virtual ICollection<GuardianAddress> GuardianAddresses { get; set; }
        public virtual ICollection<GuardianEmail> GuardianEmails { get; set; }
        public virtual ICollection<GuardianPhone> GuardianPhones { get; set; }
        public virtual ICollection<StudentGuardian> StudentGuardians { get; set; }
    }
}
