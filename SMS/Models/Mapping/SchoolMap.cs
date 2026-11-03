using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace CodeflexSystems.SMS.Models.Mapping
{
    public sealed class SchoolMap : EntityTypeConfiguration<School>
    {
        public SchoolMap()
        {
            // Primary Key
            this.HasKey(t => t.SchoolId);

            // Properties
            this.Property(t => t.SchoolId)
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);

            this.Property(t => t.SchoolName)
                .IsRequired()
                .HasMaxLength(128);

            this.Property(t => t.Motto)
                .HasMaxLength(128);

            this.Property(t => t.Description)
                .HasMaxLength(250);

            this.Property(t => t.RowVersion)
                .IsRequired()
                .IsFixedLength()
                .HasMaxLength(8)
                .IsRowVersion();

            // Table & Column Mappings
            this.ToTable("Schools");
            this.Property(t => t.SchoolId).HasColumnName("SchoolId");
            this.Property(t => t.SchoolName).HasColumnName("SchoolName");
            this.Property(t => t.Motto).HasColumnName("Motto");
            this.Property(t => t.Active).HasColumnName("Active");
            this.Property(t => t.Description).HasColumnName("Description");
            this.Property(t => t.AddressId).HasColumnName("AddressId");
            this.Property(t => t.CreatedDate).HasColumnName("CreatedDate");
            this.Property(t => t.CreatedBy).HasColumnName("CreatedBy");
            this.Property(t => t.ModifiedDate).HasColumnName("ModifiedDate");
            this.Property(t => t.ModifiedBy).HasColumnName("ModifiedBy");
            this.Property(t => t.RowVersion).HasColumnName("RowVersion");

            // Relationships
            this.HasRequired(t => t.Address)
                .WithMany(t => t.Schools)
                .HasForeignKey(d => d.AddressId);
        }
    }
}
