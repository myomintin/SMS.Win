using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace CodeflexSystems.SMS.Models.Mapping
{
    public sealed class StudentGuardianMap : EntityTypeConfiguration<StudentGuardian>
    {
        public StudentGuardianMap()
        {
            // Primary Key
            this.HasKey(t => new { t.StudentId, t.GuardianId });

            // Properties
            this.Property(t => t.StudentId)
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.None);

            this.Property(t => t.GuardianId)
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.None);

            this.Property(t => t.RowVersion)
                .IsRequired()
                .IsFixedLength()
                .HasMaxLength(8)
                .IsRowVersion();

            // Table & Column Mappings
            this.ToTable("StudentGuardians");
            this.Property(t => t.StudentId).HasColumnName("StudentId");
            this.Property(t => t.GuardianId).HasColumnName("GuardianId");
            this.Property(t => t.GuardianTypeId).HasColumnName("GuardianTypeId");
            this.Property(t => t.CreatedDate).HasColumnName("CreatedDate");
            this.Property(t => t.CreatedBy).HasColumnName("CreatedBy");
            this.Property(t => t.ModifiedDate).HasColumnName("ModifiedDate");
            this.Property(t => t.ModifiedBy).HasColumnName("ModifiedBy");
            this.Property(t => t.RowVersion).HasColumnName("RowVersion");

            // Relationships
            this.HasRequired(t => t.Guardian)
                .WithMany(t => t.StudentGuardians)
                .HasForeignKey(d => d.GuardianId);
            this.HasRequired(t => t.GuardianType)
                .WithMany(t => t.StudentGuardians)
                .HasForeignKey(d => d.GuardianTypeId);
            this.HasRequired(t => t.Student)
                .WithMany(t => t.StudentGuardians)
                .HasForeignKey(d => d.StudentId);
        }
    }
}
