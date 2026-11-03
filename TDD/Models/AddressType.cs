using System;
using System.Collections.Generic;

namespace SMS.Models
{
    public partial class AddressType
    {
        public AddressType()
        {
            this.GuardianAddresses = new List<GuardianAddress>();
            this.StudentAddresses = new List<StudentAddress>();
        }

        public Guid AddressTypeId { get; set; }
        public string AddressTypeName { get; set; }
        public DateTime CreatedDate { get; set; }
        public Nullable<Guid> CreatedBy { get; set; }
        public DateTime ModifiedDate { get; set; }
        public Nullable<Guid> ModifiedBy { get; set; }
        public byte[] RowVersion { get; set; }

        public virtual ICollection<GuardianAddress> GuardianAddresses { get; set; }
        public virtual ICollection<StudentAddress> StudentAddresses { get; set; }
    }
}
