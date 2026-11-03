using System;
using System.Collections.Generic;

namespace CodeflexSystems.SMS.Models
{
    public partial class StudentAddress : PersonAddress
    {
        public Guid StudentId { get; set; }

        public virtual Student Student { get; set; }
    }

    //public partial class StudentAddress
    //{
    //    public Guid StudentId { get; set; }
    //    public Guid AddressId { get; set; }
    //    public Guid AddressTypeId { get; set; }
    //    public DateTime CreatedDate { get; set; }
    //    public Nullable<Guid> CreatedBy { get; set; }
    //    public DateTime ModifiedDate { get; set; }
    //    public Nullable<Guid> ModifiedBy { get; set; }
    //    public byte[] RowVersion { get; set; }

    //    public virtual Address Address { get; set; }
    //    public virtual AddressType AddressType { get; set; }
    //    public virtual Student Student { get; set; }
    //}
}
