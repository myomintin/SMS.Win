using System;
using System.Collections.Generic;

namespace SMS.Models
{
    public partial class ReceiptDetail
    {
        public Guid ReceiptDetailId { get; set; }
        public byte LineNo { get; set; }
        public Nullable<DateTime> FromMonth { get; set; }
        public byte Quantity { get; set; }
        public decimal Amount { get; set; }
        public Nullable<decimal> DiscountAmount { get; set; }
        public decimal NetAmount { get; set; }
        public Guid ReceiptId { get; set; }
        public Guid FeeTypeId { get; set; }
        public Guid StudentSectionId { get; set; }
        public byte[] RowVersion { get; set; }

        public virtual FeeType FeeType { get; set; }
        public virtual Receipt Receipt { get; set; }
        public virtual StudentSection StudentSection { get; set; }
    }
}
