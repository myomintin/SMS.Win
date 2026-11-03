using System;
using System.Collections.Generic;

namespace SMS.Models
{
    public partial class GuardianPhone : Phone
    {
        public Guid GuardianId { get; set; }

        public virtual Guardian Guardian { get; set; }
    }

    //public partial class GuardianPhone
    //{
    //    public Guid PhoneId { get; set; }
    //    public string PhoneNumber { get; set; }
    //    public string Extension { get; set; }
    //    public Guid ParentId { get; set; }
    //    public Guid PhoneTypeId { get; set; }
    //    public DateTime CreatedDate { get; set; }
    //    public Nullable<Guid> CreatedBy { get; set; }
    //    public DateTime ModifiedDate { get; set; }
    //    public Nullable<Guid> ModifiedBy { get; set; }
    //    public byte[] RowVersion { get; set; }
    //    public virtual Guardian Guardian { get; set; }
    //    public virtual PhoneType PhoneType { get; set; }
    //}
}
