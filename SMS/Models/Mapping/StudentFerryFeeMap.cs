using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace CodeflexSystems.SMS.Models.Mapping
{
    public sealed class StudentFerryFeeMap : EntityTypeConfiguration<StudentFerryFee>
    {
        public StudentFerryFeeMap()
        {
            // Primary Key
            this.HasKey(t => t.StudentSectionId);

            // Properties
            this.Property(t => t.StudentSectionId)
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.None);

            this.Property(t => t.Description)
                .HasMaxLength(250);

            this.Property(t => t.RowVersion)
                .IsRequired()
                .IsFixedLength()
                .HasMaxLength(8)
                .IsRowVersion();

            // Table & Column Mappings
            this.ToTable("StudentFerryFees");
            this.Property(t => t.StudentSectionId).HasColumnName("StudentSectionId");
            this.Property(t => t.Fee).HasColumnName("Fee");
            this.Property(t => t.Monthly).HasColumnName("Monthly");
            this.Property(t => t.Description).HasColumnName("Description");
            this.Property(t => t.CreatedDate).HasColumnName("CreatedDate");
            this.Property(t => t.CreatedBy).HasColumnName("CreatedBy");
            this.Property(t => t.ModifiedDate).HasColumnName("ModifiedDate");
            this.Property(t => t.ModifiedBy).HasColumnName("ModifiedBy");
            this.Property(t => t.RowVersion).HasColumnName("RowVersion");

            // Relationships
            this.HasRequired(t => t.StudentSection)
                .WithOptional(t => t.StudentFerryFee);
        }
    }
}
