using System;
using System.Collections.Generic;

namespace CodeflexSystems.SMS.Models
{
    public partial class SchoolPhone : Phone
    {
        public Guid SchoolId { get; set; }

        public virtual School School { get; set; }
    }

    //public partial class SchoolPhone
    //{
    //    public Guid PhoneId { get; set; }
    //    public string PhoneNumber { get; set; }
    //    public string Extension { get; set; }
    //    public Guid SchoolId { get; set; }
    //    public Guid PhoneTypeId { get; set; }
    //    public DateTime CreatedDate { get; set; }
    //    public Nullable<Guid> CreatedBy { get; set; }
    //    public DateTime ModifiedDate { get; set; }
    //    public Nullable<Guid> ModifiedBy { get; set; }
    //    public byte[] RowVersion { get; set; }
    //    public virtual PhoneType PhoneType { get; set; }
    //    public virtual School School { get; set; }
    //}
}
