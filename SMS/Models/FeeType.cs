using System;
using System.Collections.Generic;

namespace CodeflexSystems.SMS.Models
{
    public partial class FeeType
    {
        public FeeType()
        {
            this.ReceiptDetails = new List<ReceiptDetail>();
        }

        public Guid FeeTypeId { get; set; }
        public string FeeTypeName { get; set; }
        public DateTime CreatedDate { get; set; }
        public Nullable<Guid> CreatedBy { get; set; }
        public DateTime ModifiedDate { get; set; }
        public Nullable<Guid> ModifiedBy { get; set; }
        public byte[] RowVersion { get; set; }

        public virtual ICollection<ReceiptDetail> ReceiptDetails { get; set; }
    }
}
