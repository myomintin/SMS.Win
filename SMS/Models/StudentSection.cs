using System;
using System.Collections.Generic;

namespace CodeflexSystems.SMS.Models
{
    public partial class StudentSection
    {
        public StudentSection()
        {
            this.ReceiptDetails = new List<ReceiptDetail>();
        }

        public Guid StudentSectionId { get; set; }
        public Nullable<DateTime> RegisteredDate { get; set; }
        public Nullable<DateTime> ResignedDate { get; set; }
        public bool Active { get; set; }
        public Guid StudentId { get; set; }
        public Guid SectionId { get; set; }
        public DateTime CreatedDate { get; set; }
        public Nullable<Guid> CreatedBy { get; set; }
        public DateTime ModifiedDate { get; set; }
        public Nullable<Guid> ModifiedBy { get; set; }
        public byte[] RowVersion { get; set; }

        public virtual Section Section { get; set; }
        public virtual Student Student { get; set; }
        public virtual StudentDiscount StudentDiscount { get; set; }
        public virtual StudentFerryFee StudentFerryFee { get; set; }
        public virtual ICollection<ReceiptDetail> ReceiptDetails { get; set; }
    }
}
