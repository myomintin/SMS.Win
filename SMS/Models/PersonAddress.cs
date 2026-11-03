using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeflexSystems.SMS.Models
{
    public abstract class PersonAddress
    {
        public Guid AddressId { get; set; }
        public Guid AddressTypeId { get; set; }
        public bool IsDefault { get; set; }
        public DateTime CreatedDate { get; set; }
        public Nullable<Guid> CreatedBy { get; set; }
        public DateTime ModifiedDate { get; set; }
        public Nullable<Guid> ModifiedBy { get; set; }
        public byte[] RowVersion { get; set; }

        public virtual Address Address { get; set; }
        public virtual AddressType AddressType { get; set; }

        public bool CanDelete
        {
            get { return true; }
        }

        public override string ToString()
        {
            if (this.Address == null)
                return base.ToString();

            return this.Address.ToString();
        }
    }
}
