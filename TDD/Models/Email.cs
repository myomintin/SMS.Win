using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SMS.Models
{
    public abstract class Email
    {
        public Guid EmailId { get; set; }
        public string EmailAddress { get; set; }
        public bool IsDefault { get; set; }
        public Guid EmailTypeId { get; set; }
        public DateTime CreatedDate { get; set; }
        public Nullable<Guid> CreatedBy { get; set; }
        public DateTime ModifiedDate { get; set; }
        public Nullable<Guid> ModifiedBy { get; set; }
        public byte[] RowVersion { get; set; }

        public virtual EmailType EmailType { get; set; }

        public bool CanDelete
        {
            get { return true; }
        }

        public override string ToString()
        {
            if (this.EmailAddress == null)
                return base.ToString();

            return this.EmailAddress;
        }
    }
}
