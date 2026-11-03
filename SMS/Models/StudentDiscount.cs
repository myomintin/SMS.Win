using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeflexSystems.SMS.Models
{
    public partial class StudentDiscount
    {
        public Guid StudentSectionId { get; set; }
        public decimal DiscountAmount { get; set; }
        public string Description { get; set; }
        public DateTime CreatedDate { get; set; }
        public Nullable<Guid> CreatedBy { get; set; }
        public DateTime ModifiedDate { get; set; }
        public Nullable<Guid> ModifiedBy { get; set; }
        public byte[] RowVersion { get; set; }

        public virtual StudentSection StudentSection { get; set; }
    }
}
