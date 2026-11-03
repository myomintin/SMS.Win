using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace CodeflexSystems.SMS.Models.Mapping
{
    public sealed class CountryMap : EntityTypeConfiguration<Country>
    {
        public CountryMap()
        {
            // Primary Key
            this.HasKey(t => t.CountryId);

            // Properties
            this.Property(t => t.CountryId)
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);

            this.Property(t => t.CountryName)
                .IsRequired()
                .HasMaxLength(128);

            this.Property(t => t.CountryCode)
                .HasMaxLength(3);

            this.Property(t => t.Code)
                .HasMaxLength(5);

            this.Property(t => t.RowVersion)
                .IsRequired()
                .IsFixedLength()
                .HasMaxLength(8)
                .IsRowVersion();

            // Table & Column Mappings
            this.ToTable("Countries");
            this.Property(t => t.CountryId).HasColumnName("CountryId");
            this.Property(t => t.CountryName).HasColumnName("CountryName");
            this.Property(t => t.CountryCode).HasColumnName("CountryCode");
            this.Property(t => t.Code).HasColumnName("Code");
            this.Property(t => t.CreatedDate).HasColumnName("CreatedDate");
            this.Property(t => t.CreatedBy).HasColumnName("CreatedBy");
            this.Property(t => t.ModifiedDate).HasColumnName("ModifiedDate");
            this.Property(t => t.ModifiedBy).HasColumnName("ModifiedBy");
            this.Property(t => t.RowVersion).HasColumnName("RowVersion");
        }
    }
}
