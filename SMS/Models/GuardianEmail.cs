using System;
using System.Collections.Generic;

namespace CodeflexSystems.SMS.Models
{
    public partial class GuardianEmail : Email
    {
        public Guid GuardianId { get; set; }

        public virtual Guardian Guardian { get; set; }
    }

    //public partial class GuardianEmail
    //{
    //    public Guid EmailId { get; set; }
    //    public string EmailAddress { get; set; }
    //    public Guid ParentId { get; set; }
    //    public Guid EmailTypeId { get; set; }
    //    public DateTime CreatedDate { get; set; }
    //    public Nullable<Guid> CreatedBy { get; set; }
    //    public DateTime ModifiedDate { get; set; }
    //    public Nullable<Guid> ModifiedBy { get; set; }
    //    public byte[] RowVersion { get; set; }
    //    public virtual EmailType EmailType { get; set; }
    //    public virtual Guardian Guardian { get; set; }
    //}
}
