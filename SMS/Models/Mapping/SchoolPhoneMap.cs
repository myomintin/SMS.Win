using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace CodeflexSystems.SMS.Models.Mapping
{
    public sealed class SchoolPhoneMap : EntityTypeConfiguration<SchoolPhone>
    {
        public SchoolPhoneMap()
        {
            // Primary Key
            this.HasKey(t => t.PhoneId);

            // Properties
            this.Property(t => t.PhoneId)
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);

            this.Property(t => t.PhoneNumber)
                .IsRequired()
                .HasMaxLength(24);

            this.Property(t => t.Extension)
                .HasMaxLength(10);

            this.Property(t => t.RowVersion)
                .IsRequired()
                .IsFixedLength()
                .HasMaxLength(8)
                .IsRowVersion();

            // Table & Column Mappings
            this.ToTable("SchoolPhones");
            this.Property(t => t.PhoneId).HasColumnName("PhoneId");
            this.Property(t => t.PhoneNumber).HasColumnName("PhoneNumber");
            this.Property(t => t.Extension).HasColumnName("Extension");
            this.Property(t => t.IsDefault).HasColumnName("IsDefault");
            this.Property(t => t.SchoolId).HasColumnName("SchoolId");
            this.Property(t => t.PhoneTypeId).HasColumnName("PhoneTypeId");
            this.Property(t => t.CreatedDate).HasColumnName("CreatedDate");
            this.Property(t => t.CreatedBy).HasColumnName("CreatedBy");
            this.Property(t => t.ModifiedDate).HasColumnName("ModifiedDate");
            this.Property(t => t.ModifiedBy).HasColumnName("ModifiedBy");
            this.Property(t => t.RowVersion).HasColumnName("RowVersion");

            // Relationships
            this.HasRequired(t => t.PhoneType)
                .WithMany(t => t.SchoolPhones)
                .HasForeignKey(d => d.PhoneTypeId);
            this.HasRequired(t => t.School)
                .WithMany(t => t.SchoolPhones)
                .HasForeignKey(d => d.SchoolId);
        }
    }
}
