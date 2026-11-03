using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeflexSystems.SMS.Models
{
    public abstract class PersonGuardian
    {
        public Guid GuardianId { get; set; }
        public Guid GuardianTypeId { get; set; }
        public DateTime CreatedDate { get; set; }
        public Nullable<Guid> CreatedBy { get; set; }
        public DateTime ModifiedDate { get; set; }
        public Nullable<Guid> ModifiedBy { get; set; }
        public byte[] RowVersion { get; set; }

        public virtual Guardian Guardian { get; set; }
        public virtual GuardianType GuardianType { get; set; }

        public bool CanDelete
        {
            get { return true; }
        }

        public override string ToString()
        {
            if (this.Guardian == null)
                return base.ToString();

            return this.Guardian.ToString();
        }
    }
}
