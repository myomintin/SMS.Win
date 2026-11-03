using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace CodeflexSystems.SMS.Models.Mapping
{
    public sealed class StudentPhotoMap : EntityTypeConfiguration<StudentPhoto>
    {
        public StudentPhotoMap()
        {
            // Primary Key
            this.HasKey(t => t.StudentId);

            // Properties
            this.Property(t => t.StudentId)
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.None);

            this.Property(t => t.RowVersion)
                .IsRequired()
                .IsFixedLength()
                .HasMaxLength(8)
                .IsRowVersion();

            // Table & Column Mappings
            this.ToTable("StudentPhotos");
            this.Property(t => t.StudentId).HasColumnName("StudentId");
            this.Property(t => t.Photo).HasColumnName("Photo");
            this.Property(t => t.RowVersion).HasColumnName("RowVersion");

            // Relationships
            this.HasRequired(t => t.Student)
                .WithOptional(t => t.Photo);
        }
    }
}
