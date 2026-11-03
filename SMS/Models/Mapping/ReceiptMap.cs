using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace CodeflexSystems.SMS.Models.Mapping
{
    public class ReceiptMap : EntityTypeConfiguration<Receipt>
    {
        public ReceiptMap()
        {
            // Primary Key
            this.HasKey(t => t.ReceiptId);

            // Properties
            this.Property(t => t.ReceiptId)
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);

            this.Property(t => t.ReceiptCode)
                .HasMaxLength(10);

            this.Property(t => t.Description)
                .HasMaxLength(250);

            this.Property(t => t.RowVersion)
                .IsRequired()
                .IsFixedLength()
                .HasMaxLength(8)
                .IsRowVersion();

            // Table & Column Mappings
            this.ToTable("Receipts");
            this.Property(t => t.ReceiptId).HasColumnName("ReceiptId");
            this.Property(t => t.ReceiptDate).HasColumnName("ReceiptDate");
            this.Property(t => t.ReceiptCode).HasColumnName("ReceiptCode");
            this.Property(t => t.ReceiptNo).HasColumnName("ReceiptNo");
            this.Property(t => t.TotalAmount).HasColumnName("TotalAmount");
            this.Property(t => t.TaxAmount).HasColumnName("TaxAmount");
            this.Property(t => t.NetAmount).HasColumnName("NetAmount");
            this.Property(t => t.Description).HasColumnName("Description");
            this.Property(t => t.CurrencyId).HasColumnName("CurrencyId");
            this.Property(t => t.SchoolId).HasColumnName("SchoolId");
            this.Property(t => t.CreatedDate).HasColumnName("CreatedDate");
            this.Property(t => t.CreatedBy).HasColumnName("CreatedBy");
            this.Property(t => t.ModifiedDate).HasColumnName("ModifiedDate");
            this.Property(t => t.ModifiedBy).HasColumnName("ModifiedBy");
            this.Property(t => t.RowVersion).HasColumnName("RowVersion");

            // Relationships
            this.HasRequired(t => t.Currency)
                .WithMany(t => t.Receipts)
                .HasForeignKey(d => d.CurrencyId);
        }
    }
}
