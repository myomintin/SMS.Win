using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace CodeflexSystems.SMS.Models.Mapping
{
    public sealed class StudentSectionMap : EntityTypeConfiguration<StudentSection>
    {
        public StudentSectionMap()
        {
            // Primary Key
            this.HasKey(t => t.StudentSectionId);

            // Properties
            this.Property(t => t.StudentSectionId)
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);

            this.Property(t => t.RowVersion)
                .IsRequired()
                .IsFixedLength()
                .HasMaxLength(8)
                .IsRowVersion();

            // Table & Column Mappings
            this.ToTable("StudentSections");
            this.Property(t => t.StudentSectionId).HasColumnName("StudentSectionId");
            this.Property(t => t.RegisteredDate).HasColumnName("RegisteredDate");
            this.Property(t => t.ResignedDate).HasColumnName("ResignedDate");
            this.Property(t => t.Active).HasColumnName("Active");
            this.Property(t => t.StudentId).HasColumnName("StudentId");
            this.Property(t => t.SectionId).HasColumnName("SectionId");
            this.Property(t => t.CreatedDate).HasColumnName("CreatedDate");
            this.Property(t => t.CreatedBy).HasColumnName("CreatedBy");
            this.Property(t => t.ModifiedDate).HasColumnName("ModifiedDate");
            this.Property(t => t.ModifiedBy).HasColumnName("ModifiedBy");
            this.Property(t => t.RowVersion).HasColumnName("RowVersion");

            // Relationships
            this.HasRequired(t => t.Section)
                .WithMany(t => t.StudentSections)
                .HasForeignKey(d => d.SectionId);
            this.HasRequired(t => t.Student)
                .WithMany(t => t.StudentSections)
                .HasForeignKey(d => d.StudentId);
        }
    }
}
