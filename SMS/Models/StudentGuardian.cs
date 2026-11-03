using System;
using System.Collections.Generic;

namespace CodeflexSystems.SMS.Models
{
    public partial class StudentGuardian : PersonGuardian
    {
        public Guid StudentId { get; set; }

        public virtual Student Student { get; set; }
    }

    //public partial class StudentGuardian
    //{
    //    public Guid StudentId { get; set; }
    //    public Guid GuardianId { get; set; }
    //    public Guid GuardianTypeId { get; set; }
    //    public DateTime CreatedDate { get; set; }
    //    public Nullable<Guid> CreatedBy { get; set; }
    //    public DateTime ModifiedDate { get; set; }
    //    public Nullable<Guid> ModifiedBy { get; set; }
    //    public byte[] RowVersion { get; set; }

    //    public virtual Guardian Guardian { get; set; }
    //    public virtual GuardianType GuardianType { get; set; }
    //    public virtual Student Student { get; set; }
    //}
}
