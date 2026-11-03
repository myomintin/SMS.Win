using System;
using System.Collections.Generic;

namespace SMS.Models
{
    public partial class Address
    {
        public Address()
        {
            this.GuardianAddresses = new List<GuardianAddress>();
            this.Schools = new List<School>();
            this.StudentAddresses = new List<StudentAddress>();
        }

        public Guid AddressId { get; set; }
        public string Line1 { get; set; }
        public string Line2 { get; set; }
        public string ZipCode { get; set; }
        public string City { get; set; }
        public string Township { get; set; }
        public string State { get; set; }
        public Guid CountryId { get; set; }
        public DateTime CreatedDate { get; set; }
        public Nullable<Guid> CreatedBy { get; set; }
        public DateTime ModifiedDate { get; set; }
        public Nullable<Guid> ModifiedBy { get; set; }
        public byte[] RowVersion { get; set; }

        public virtual Country Country { get; set; }
        public virtual ICollection<GuardianAddress> GuardianAddresses { get; set; }
        public virtual ICollection<School> Schools { get; set; }
        public virtual ICollection<StudentAddress> StudentAddresses { get; set; }
    }
}
