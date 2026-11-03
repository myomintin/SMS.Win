using System;
using System.Collections.Generic;

namespace CodeflexSystems.SMS.Models
{
    public partial class Country
    {
        public Country()
        {
            this.Addresses = new List<Address>();
            this.Guardians = new List<Guardian>();
            this.Students = new List<Student>();
        }

        public Guid CountryId { get; set; }
        public string CountryName { get; set; }
        public string CountryCode { get; set; }
        public string Code { get; set; }
        public DateTime CreatedDate { get; set; }
        public Nullable<Guid> CreatedBy { get; set; }
        public DateTime ModifiedDate { get; set; }
        public Nullable<Guid> ModifiedBy { get; set; }
        public byte[] RowVersion { get; set; }

        public virtual ICollection<Address> Addresses { get; set; }
        public virtual ICollection<Guardian> Guardians { get; set; }
        public virtual ICollection<Student> Students { get; set; }
    }
}
