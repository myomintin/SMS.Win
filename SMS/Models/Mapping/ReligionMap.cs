using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace CodeflexSystems.SMS.Models.Mapping
{
    public sealed class ReligionMap : EntityTypeConfiguration<Religion>
    {
        public ReligionMap()
        {
            // Primary Key
            this.HasKey(t => t.ReligionId);

            // Properties
            this.Property(t => t.ReligionId)
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);

            this.Property(t => t.ReligionName)
                .IsRequired()
                .HasMaxLength(128);

            this.Property(t => t.RowVersion)
                .IsRequired()
                .IsFixedLength()
                .HasMaxLength(8)
                .IsRowVersion();

            // Table & Column Mappings
            this.ToTable("Religions");
            this.Property(t => t.ReligionId).HasColumnName("ReligionId");
            this.Property(t => t.ReligionName).HasColumnName("ReligionName");
            this.Property(t => t.CreatedDate).HasColumnName("CreatedDate");
            this.Property(t => t.CreatedBy).HasColumnName("CreatedBy");
            this.Property(t => t.ModifiedDate).HasColumnName("ModifiedDate");
            this.Property(t => t.ModifiedBy).HasColumnName("ModifiedBy");
            this.Property(t => t.RowVersion).HasColumnName("RowVersion");
        }
    }
}
