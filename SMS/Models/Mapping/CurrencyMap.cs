using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace CodeflexSystems.SMS.Models.Mapping
{
    public sealed class CurrencyMap : EntityTypeConfiguration<Currency>
    {
        public CurrencyMap()
        {
            // Primary Key
            this.HasKey(t => t.CurrencyId);

            // Properties
            this.Property(t => t.CurrencyId)
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);

            this.Property(t => t.CurrencyName)
                .IsRequired()
                .HasMaxLength(128);

            this.Property(t => t.CurrencyCode)
                .IsRequired()
                .HasMaxLength(5);

            this.Property(t => t.CurrencySymbol)
                .HasMaxLength(5);

            this.Property(t => t.RowVersion)
                .IsRequired()
                .IsFixedLength()
                .HasMaxLength(8)
                .IsRowVersion();

            // Table & Column Mappings
            this.ToTable("Currencies");
            this.Property(t => t.CurrencyId).HasColumnName("CurrencyId");
            this.Property(t => t.CurrencyName).HasColumnName("CurrencyName");
            this.Property(t => t.CurrencyCode).HasColumnName("CurrencyCode");
            this.Property(t => t.CurrencySymbol).HasColumnName("CurrencySymbol");
            this.Property(t => t.CreatedDate).HasColumnName("CreatedDate");
            this.Property(t => t.CreatedBy).HasColumnName("CreatedBy");
            this.Property(t => t.ModifiedDate).HasColumnName("ModifiedDate");
            this.Property(t => t.ModifiedBy).HasColumnName("ModifiedBy");
            this.Property(t => t.RowVersion).HasColumnName("RowVersion");
        }
    }
}
