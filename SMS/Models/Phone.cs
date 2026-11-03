using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeflexSystems.SMS.Models
{
    public abstract class Phone
    {
        public Guid PhoneId { get; set; }
        public string PhoneNumber { get; set; }
        public string Extension { get; set; }
        public bool IsDefault { get; set; }
        public Guid PhoneTypeId { get; set; }
        public DateTime CreatedDate { get; set; }
        public Nullable<Guid> CreatedBy { get; set; }
        public DateTime ModifiedDate { get; set; }
        public Nullable<Guid> ModifiedBy { get; set; }
        public byte[] RowVersion { get; set; }

        public virtual PhoneType PhoneType { get; set; }

        public bool CanDelete
        {
            get { return true; }
        }

        public override string ToString()
        {
            if (this.PhoneNumber == null)
                return base.ToString();

            return this.PhoneNumber;
        }
    }
}
