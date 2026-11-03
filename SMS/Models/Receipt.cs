using System;
using System.Collections.Generic;

namespace CodeflexSystems.SMS.Models
{
    public partial class Receipt
    {
        public Receipt()
        {
            this.ReceiptDetails = new List<ReceiptDetail>();
        }

        public Guid ReceiptId { get; set; }
        public DateTime ReceiptDate { get; set; }
        public string ReceiptCode { get; set; }
        public int ReceiptNo { get; set; }
        public decimal TotalAmount { get; set; }
        public Nullable<decimal> TaxAmount { get; set; }
        public decimal NetAmount { get; set; }
        public string Description { get; set; }
        public Guid CurrencyId { get; set; }
        public Guid SchoolId { get; set; }
        public DateTime CreatedDate { get; set; }
        public Nullable<Guid> CreatedBy { get; set; }
        public DateTime ModifiedDate { get; set; }
        public Nullable<Guid> ModifiedBy { get; set; }
        public byte[] RowVersion { get; set; }

        public virtual Currency Currency { get; set; }
        public virtual ICollection<ReceiptDetail> ReceiptDetails { get; set; }
    }
}
