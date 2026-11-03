using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace CodeflexSystems.SMS.Models.Mapping
{
    public sealed class PhoneTypeMap : EntityTypeConfiguration<PhoneType>
    {
        public PhoneTypeMap()
        {
            // Primary Key
            this.HasKey(t => t.PhoneTypeId);

            // Properties
            this.Property(t => t.PhoneTypeId)
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);

            this.Property(t => t.PhoneTypeName)
                .IsRequired()
                .HasMaxLength(128);

            this.Property(t => t.RowVersion)
                .IsRequired()
                .IsFixedLength()
                .HasMaxLength(8)
                .IsRowVersion();

            // Table & Column Mappings
            this.ToTable("PhoneTypes");
            this.Property(t => t.PhoneTypeId).HasColumnName("PhoneTypeId");
            this.Property(t => t.PhoneTypeName).HasColumnName("PhoneTypeName");
            this.Property(t => t.CreatedDate).HasColumnName("CreatedDate");
            this.Property(t => t.CreatedBy).HasColumnName("CreatedBy");
            this.Property(t => t.ModifiedDate).HasColumnName("ModifiedDate");
            this.Property(t => t.ModifiedBy).HasColumnName("ModifiedBy");
            this.Property(t => t.RowVersion).HasColumnName("RowVersion");
        }
    }
}
