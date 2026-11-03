using System;
using System.Collections.Generic;

namespace CodeflexSystems.SMS.Models
{
    public partial class SchoolEmail : Email
    {
        public Guid SchoolId { get; set; }

        public virtual School School { get; set; }
    }

    //public partial class SchoolEmail
    //{
    //    public Guid EmailId { get; set; }
    //    public string EmailAddress { get; set; }
    //    public Guid SchoolId { get; set; }
    //    public Guid EmailTypeId { get; set; }
    //    public DateTime CreatedDate { get; set; }
    //    public Nullable<Guid> CreatedBy { get; set; }
    //    public DateTime ModifiedDate { get; set; }
    //    public Nullable<Guid> ModifiedBy { get; set; }
    //    public byte[] RowVersion { get; set; }
    //    public virtual EmailType EmailType { get; set; }
    //    public virtual School School { get; set; }
    //}
}
