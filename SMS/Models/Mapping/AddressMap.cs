using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace CodeflexSystems.SMS.Models.Mapping
{
    public sealed class AddressMap : EntityTypeConfiguration<Address>
    {
        public AddressMap()
        {
            // Primary Key
            this.HasKey(t => t.AddressId);

            // Properties
            this.Property(t => t.AddressId)
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);

            this.Property(t => t.Line1)
                .IsRequired()
                .HasMaxLength(250);

            this.Property(t => t.Line2)
                .HasMaxLength(250);

            this.Property(t => t.ZipCode)
                .HasMaxLength(20);

            this.Property(t => t.City)
                .HasMaxLength(128);

            this.Property(t => t.Township)
                .HasMaxLength(128);

            this.Property(t => t.State)
                .HasMaxLength(128);

            this.Property(t => t.RowVersion)
                .IsRequired()
                .IsFixedLength()
                .HasMaxLength(8)
                .IsRowVersion();

            // Table & Column Mappings
            this.ToTable("Addresses");
            this.Property(t => t.AddressId).HasColumnName("AddressId");
            this.Property(t => t.Line1).HasColumnName("Line1");
            this.Property(t => t.Line2).HasColumnName("Line2");
            this.Property(t => t.ZipCode).HasColumnName("ZipCode");
            this.Property(t => t.City).HasColumnName("City");
            this.Property(t => t.Township).HasColumnName("Township");
            this.Property(t => t.State).HasColumnName("State");
            this.Property(t => t.CountryId).HasColumnName("CountryId");
            this.Property(t => t.CreatedDate).HasColumnName("CreatedDate");
            this.Property(t => t.CreatedBy).HasColumnName("CreatedBy");
            this.Property(t => t.ModifiedDate).HasColumnName("ModifiedDate");
            this.Property(t => t.ModifiedBy).HasColumnName("ModifiedBy");
            this.Property(t => t.RowVersion).HasColumnName("RowVersion");

            // Relationships
            this.HasRequired(t => t.Country)
                .WithMany(t => t.Addresses)
                .HasForeignKey(d => d.CountryId);
        }
    }
}
