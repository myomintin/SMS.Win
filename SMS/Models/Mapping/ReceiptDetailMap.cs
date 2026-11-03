using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace CodeflexSystems.SMS.Models.Mapping
{
    public sealed class ReceiptDetailMap : EntityTypeConfiguration<ReceiptDetail>
    {
        public ReceiptDetailMap()
        {
            // Primary Key
            this.HasKey(t => t.ReceiptDetailId);

            // Properties
            this.Property(t => t.ReceiptDetailId)
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);

            this.Property(t => t.RowVersion)
                .IsRequired()
                .IsFixedLength()
                .HasMaxLength(8)
                .IsRowVersion();

            // Table & Column Mappings
            this.ToTable("ReceiptDetails");
            this.Property(t => t.ReceiptDetailId).HasColumnName("ReceiptDetailId");
            this.Property(t => t.LineNo).HasColumnName("LineNo");
            this.Property(t => t.FromMonth).HasColumnName("FromMonth");
            this.Property(t => t.Quantity).HasColumnName("Quantity");
            this.Property(t => t.Amount).HasColumnName("Amount");
            this.Property(t => t.DiscountAmount).HasColumnName("DiscountAmount");
            this.Property(t => t.NetAmount).HasColumnName("NetAmount");
            this.Property(t => t.ReceiptId).HasColumnName("ReceiptId");
            this.Property(t => t.FeeTypeId).HasColumnName("FeeTypeId");
            this.Property(t => t.StudentSectionId).HasColumnName("StudentSectionId");
            this.Property(t => t.RowVersion).HasColumnName("RowVersion");

            // Relationships
            this.HasRequired(t => t.FeeType)
                .WithMany(t => t.ReceiptDetails)
                .HasForeignKey(d => d.FeeTypeId);
            this.HasRequired(t => t.Receipt)
                .WithMany(t => t.ReceiptDetails)
                .HasForeignKey(d => d.ReceiptId);
            this.HasRequired(t => t.StudentSection)
                .WithMany(t => t.ReceiptDetails)
                .HasForeignKey(d => d.StudentSectionId);
        }
    }
}
