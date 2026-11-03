using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace CodeflexSystems.SMS.Models.Mapping
{
    public sealed class ClassMap : EntityTypeConfiguration<Class>
    {
        public ClassMap()
        {
            // Primary Key
            this.HasKey(t => t.ClassId);

            // Properties
            this.Property(t => t.ClassId)
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);

            this.Property(t => t.ClassName)
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
            this.ToTable("Classes");
            this.Property(t => t.ClassId).HasColumnName("ClassId");
            this.Property(t => t.ClassName).HasColumnName("ClassName");
            this.Property(t => t.Description).HasColumnName("Description");
            this.Property(t => t.CourseId).HasColumnName("CourseId");
            this.Property(t => t.CreatedDate).HasColumnName("CreatedDate");
            this.Property(t => t.CreatedBy).HasColumnName("CreatedBy");
            this.Property(t => t.ModifiedDate).HasColumnName("ModifiedDate");
            this.Property(t => t.ModifiedBy).HasColumnName("ModifiedBy");
            this.Property(t => t.RowVersion).HasColumnName("RowVersion");

            // Relationships
            this.HasRequired(t => t.Course)
                .WithMany(t => t.Classes)
                .HasForeignKey(d => d.CourseId);
        }
    }
}
