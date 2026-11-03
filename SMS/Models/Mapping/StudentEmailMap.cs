using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace CodeflexSystems.SMS.Models.Mapping
{
    public sealed class StudentEmailMap : EntityTypeConfiguration<StudentEmail>
    {
        public StudentEmailMap()
        {
            // Primary Key
            this.HasKey(t => t.EmailId);

            // Properties
            this.Property(t => t.EmailId)
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);

            this.Property(t => t.EmailAddress)
                .IsRequired()
                .HasMaxLength(128);

            this.Property(t => t.RowVersion)
                .IsRequired()
                .IsFixedLength()
                .HasMaxLength(8)
                .IsRowVersion();

            // Table & Column Mappings
            this.ToTable("StudentEmails");
            this.Property(t => t.EmailId).HasColumnName("EmailId");
            this.Property(t => t.EmailAddress).HasColumnName("EmailAddress");
            this.Property(t => t.IsDefault).HasColumnName("IsDefault");
            this.Property(t => t.StudentId).HasColumnName("StudentId");
            this.Property(t => t.EmailTypeId).HasColumnName("EmailTypeId");
            this.Property(t => t.CreatedDate).HasColumnName("CreatedDate");
            this.Property(t => t.CreatedBy).HasColumnName("CreatedBy");
            this.Property(t => t.ModifiedDate).HasColumnName("ModifiedDate");
            this.Property(t => t.ModifiedBy).HasColumnName("ModifiedBy");
            this.Property(t => t.RowVersion).HasColumnName("RowVersion");

            // Relationships
            this.HasRequired(t => t.EmailType)
                .WithMany(t => t.StudentEmails)
                .HasForeignKey(d => d.EmailTypeId);
            this.HasRequired(t => t.Student)
                .WithMany(t => t.StudentEmails)
                .HasForeignKey(d => d.StudentId);
        }
    }
}
