using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace CodeflexSystems.SMS.Models.Mapping
{
    public sealed class AcademicYearMap : EntityTypeConfiguration<AcademicYear>
    {
        public AcademicYearMap()
        {
            // Primary Key
            this.HasKey(t => t.AcademicYearId);

            // Properties
            this.Property(t => t.AcademicYearId)
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);

            this.Property(t => t.AcademicYearName)
                .IsRequired()
                .HasMaxLength(128);

            this.Property(t => t.Description)
                .HasMaxLength(250);

            this.Property(t => t.RowVersion)
                .IsRequired()
                .IsFixedLength()
                .HasMaxLength(8)
                .IsRowVersion();

            // Table & Column Mappings
            this.ToTable("AcademicYears");
            this.Property(t => t.AcademicYearId).HasColumnName("AcademicYearId");
            this.Property(t => t.AcademicYearName).HasColumnName("AcademicYearName");
            this.Property(t => t.Active).HasColumnName("Active");
            this.Property(t => t.Description).HasColumnName("Description");
            this.Property(t => t.SchoolId).HasColumnName("SchoolId");
            this.Property(t => t.CreatedDate).HasColumnName("CreatedDate");
            this.Property(t => t.CreatedBy).HasColumnName("CreatedBy");
            this.Property(t => t.ModifiedDate).HasColumnName("ModifiedDate");
            this.Property(t => t.ModifiedBy).HasColumnName("ModifiedBy");
            this.Property(t => t.RowVersion).HasColumnName("RowVersion");

            // Relationships
            this.HasRequired(t => t.School)
                .WithMany(t => t.AcademicYears)
                .HasForeignKey(d => d.SchoolId);
        }
    }
}
