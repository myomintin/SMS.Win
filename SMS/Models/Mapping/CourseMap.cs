using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace CodeflexSystems.SMS.Models.Mapping
{
    public sealed class CourseMap : EntityTypeConfiguration<Course>
    {
        public CourseMap()
        {
            // Primary Key
            this.HasKey(t => t.CourseId);

            // Properties
            this.Property(t => t.CourseId)
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);

            this.Property(t => t.CourseName)
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
            this.ToTable("Courses");
            this.Property(t => t.CourseId).HasColumnName("CourseId");
            this.Property(t => t.CourseName).HasColumnName("CourseName");
            this.Property(t => t.Description).HasColumnName("Description");
            this.Property(t => t.AcademicYearId).HasColumnName("AcademicYearId");
            this.Property(t => t.CreatedDate).HasColumnName("CreatedDate");
            this.Property(t => t.CreatedBy).HasColumnName("CreatedBy");
            this.Property(t => t.ModifiedDate).HasColumnName("ModifiedDate");
            this.Property(t => t.ModifiedBy).HasColumnName("ModifiedBy");
            this.Property(t => t.RowVersion).HasColumnName("RowVersion");

            // Relationships
            this.HasRequired(t => t.AcademicYear)
                .WithMany(t => t.Courses)
                .HasForeignKey(d => d.AcademicYearId);
        }
    }
}
