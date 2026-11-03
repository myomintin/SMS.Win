using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace CodeflexSystems.SMS.Models.Mapping
{
    public sealed class GuardianPhotoMap : EntityTypeConfiguration<GuardianPhoto>
    {
        public GuardianPhotoMap()
        {
            // Primary Key
            this.HasKey(t => t.GuardianId);

            // Properties
            this.Property(t => t.GuardianId)
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.None);

            this.Property(t => t.RowVersion)
                .IsRequired()
                .IsFixedLength()
                .HasMaxLength(8)
                .IsRowVersion();

            // Table & Column Mappings
            this.ToTable("GuardianPhotos");
            this.Property(t => t.GuardianId).HasColumnName("GuardianId");
            this.Property(t => t.Photo).HasColumnName("Photo");
            this.Property(t => t.RowVersion).HasColumnName("RowVersion");

            // Relationships
            this.HasRequired(t => t.Guardian)
                .WithOptional(t => t.Photo);
        }
    }
}
