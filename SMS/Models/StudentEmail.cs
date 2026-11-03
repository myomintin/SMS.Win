using System;
using System.Collections.Generic;

namespace CodeflexSystems.SMS.Models
{
    public partial class StudentEmail : Email
    {
        public Guid StudentId { get; set; }

        public virtual Student Student { get; set; }
    }

    //public partial class StudentEmail
    //{
    //    public Guid EmailId { get; set; }
    //    public string EmailAddress { get; set; }
    //    public Guid StudentId { get; set; }
    //    public Guid EmailTypeId { get; set; }
    //    public DateTime CreatedDate { get; set; }
    //    public Nullable<Guid> CreatedBy { get; set; }
    //    public DateTime ModifiedDate { get; set; }
    //    public Nullable<Guid> ModifiedBy { get; set; }
    //    public byte[] RowVersion { get; set; }
    //    public virtual EmailType EmailType { get; set; }
    //    public virtual Student Student { get; set; }
    //}
}
