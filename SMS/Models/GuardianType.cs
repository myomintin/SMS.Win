using System;
using System.Collections.Generic;

namespace CodeflexSystems.SMS.Models
{
    public partial class GuardianType
    {
        public GuardianType()
        {
            this.StudentGuardians = new List<StudentGuardian>();
        }

        public Guid GuardianTypeId { get; set; }
        public string GuardianTypeName { get; set; }
        public bool Gender { get; set; }
        public DateTime CreatedDate { get; set; }
        public Nullable<Guid> CreatedBy { get; set; }
        public DateTime ModifiedDate { get; set; }
        public Nullable<Guid> ModifiedBy { get; set; }
        public byte[] RowVersion { get; set; }

        public virtual ICollection<StudentGuardian> StudentGuardians { get; set; }
    }
}
