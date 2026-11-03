using System;
using System.Collections.Generic;

namespace SMS.Models
{
    public partial class Currency
    {
        public Currency()
        {
            this.Receipts = new List<Receipt>();
            this.Sections = new List<Section>();
        }

        public Guid CurrencyId { get; set; }
        public string CurrencyName { get; set; }
        public string CurrencyCode { get; set; }
        public string CurrencySymbol { get; set; }
        public DateTime CreatedDate { get; set; }
        public Nullable<Guid> CreatedBy { get; set; }
        public DateTime ModifiedDate { get; set; }
        public Nullable<Guid> ModifiedBy { get; set; }
        public byte[] RowVersion { get; set; }

        public virtual ICollection<Receipt> Receipts { get; set; }
        public virtual ICollection<Section> Sections { get; set; }
    }
}
