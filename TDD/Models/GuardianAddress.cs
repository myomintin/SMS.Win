using System;
using System.Collections.Generic;

namespace SMS.Models
{
    public partial class GuardianAddress : PersonAddress
    {
        public Guid GuardianId { get; set; }

        public virtual Guardian Guardian { get; set; }
    }

    //public partial class GuardianAddress
    //{
    //    public Guid GuardianId { get; set; }
    //    public Guid AddressId { get; set; }
    //    public Guid AddressTypeId { get; set; }
    //    public DateTime CreatedDate { get; set; }
    //    public Nullable<Guid> CreatedBy { get; set; }
    //    public DateTime ModifiedDate { get; set; }
    //    public Nullable<Guid> ModifiedBy { get; set; }
    //    public byte[] RowVersion { get; set; }

    //    public virtual Address Address { get; set; }
    //    public virtual AddressType AddressType { get; set; }
    //    public virtual Guardian Guardian { get; set; }
    //}
}
