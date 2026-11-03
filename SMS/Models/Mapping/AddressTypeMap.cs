using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace CodeflexSystems.SMS.Models.Mapping
{
    public sealed class AddressTypeMap : EntityTypeConfiguration<AddressType>
    {
        public AddressTypeMap()
        {
            // Primary Key
            this.HasKey(t => t.AddressTypeId);

            // Properties
            this.Property(t => t.AddressTypeId)
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);

            this.Property(t => t.AddressTypeName)
                .IsRequired()
                .HasMaxLength(128);

            this.Property(t => t.RowVersion)
                .IsRequired()
                .IsFixedLength()
                .HasMaxLength(8)
                .IsRowVersion();

            // Table & Column Mappings
            this.ToTable("AddressTypes");
            this.Property(t => t.AddressTypeId).HasColumnName("AddressTypeId");
            this.Property(t => t.AddressTypeName).HasColumnName("AddressTypeName");
            this.Property(t => t.CreatedDate).HasColumnName("CreatedDate");
            this.Property(t => t.CreatedBy).HasColumnName("CreatedBy");
            this.Property(t => t.ModifiedDate).HasColumnName("ModifiedDate");
            this.Property(t => t.ModifiedBy).HasColumnName("ModifiedBy");
            this.Property(t => t.RowVersion).HasColumnName("RowVersion");
        }
    }
}
