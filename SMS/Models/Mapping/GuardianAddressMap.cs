using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace CodeflexSystems.SMS.Models.Mapping
{
    public sealed class GuardianAddressMap : EntityTypeConfiguration<GuardianAddress>
    {
        public GuardianAddressMap()
        {
            // Primary Key
            this.HasKey(t => new { t.GuardianId, t.AddressId });

            // Properties
            this.Property(t => t.GuardianId)
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.None);

            this.Property(t => t.AddressId)
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.None);

            this.Property(t => t.RowVersion)
                .IsRequired()
                .IsFixedLength()
                .HasMaxLength(8)
                .IsRowVersion();

            // Table & Column Mappings
            this.ToTable("GuardianAddresses");
            this.Property(t => t.GuardianId).HasColumnName("GuardianId");
            this.Property(t => t.AddressId).HasColumnName("AddressId");
            this.Property(t => t.IsDefault).HasColumnName("IsDefault");
            this.Property(t => t.AddressTypeId).HasColumnName("AddressTypeId");
            this.Property(t => t.CreatedDate).HasColumnName("CreatedDate");
            this.Property(t => t.CreatedBy).HasColumnName("CreatedBy");
            this.Property(t => t.ModifiedDate).HasColumnName("ModifiedDate");
            this.Property(t => t.ModifiedBy).HasColumnName("ModifiedBy");
            this.Property(t => t.RowVersion).HasColumnName("RowVersion");

            // Relationships
            this.HasRequired(t => t.Address)
                .WithMany(t => t.GuardianAddresses)
                .HasForeignKey(d => d.AddressId);
            this.HasRequired(t => t.AddressType)
                .WithMany(t => t.GuardianAddresses)
                .HasForeignKey(d => d.AddressTypeId);
            this.HasRequired(t => t.Guardian)
                .WithMany(t => t.GuardianAddresses)
                .HasForeignKey(d => d.GuardianId);
        }
    }
}
