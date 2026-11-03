using System;
using System.Collections.Generic;

namespace CodeflexSystems.SMS.Models
{
    public partial class Student
    {
        public Student()
        {
            this.StudentAddresses = new List<StudentAddress>();
            this.StudentEmails = new List<StudentEmail>();
            this.StudentGuardians = new List<StudentGuardian>();
            this.StudentPhones = new List<StudentPhone>();
            this.StudentSections = new List<StudentSection>();
        }

        public Guid StudentId { get; set; }
        public string Id { get; set; }
        public string Title { get; set; }
        public string StudentName { get; set; }
        public string OtherName { get; set; }
        public bool Gender { get; set; }
        public Nullable<DateTime> BirthDate { get; set; }
        public string Nationality { get; set; }
        public string NIRC { get; set; }
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
        public virtual StudentPhoto Photo { get; set; }
        public virtual ICollection<StudentAddress> StudentAddresses { get; set; }
        public virtual ICollection<StudentEmail> StudentEmails { get; set; }
        public virtual ICollection<StudentGuardian> StudentGuardians { get; set; }
        public virtual ICollection<StudentPhone> StudentPhones { get; set; }
        public virtual ICollection<StudentSection> StudentSections { get; set; }
    }
}
