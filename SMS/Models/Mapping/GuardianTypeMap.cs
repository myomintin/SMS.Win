using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace CodeflexSystems.SMS.Models.Mapping
{
    public sealed class GuardianTypeMap : EntityTypeConfiguration<GuardianType>
    {
        public GuardianTypeMap()
        {
            // Primary Key
            this.HasKey(t => t.GuardianTypeId);

            // Properties
            this.Property(t => t.GuardianTypeId)
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);

            this.Property(t => t.GuardianTypeName)
                .IsRequired()
                .HasMaxLength(128);

            this.Property(t => t.RowVersion)
                .IsRequired()
                .IsFixedLength()
                .HasMaxLength(8)
                .IsRowVersion();

            // Table & Column Mappings
            this.ToTable("GuardianTypes");
            this.Property(t => t.GuardianTypeId).HasColumnName("GuardianTypeId");
            this.Property(t => t.GuardianTypeName).HasColumnName("GuardianTypeName");
            this.Property(t => t.Gender).HasColumnName("Gender");
            this.Property(t => t.CreatedDate).HasColumnName("CreatedDate");
            this.Property(t => t.CreatedBy).HasColumnName("CreatedBy");
            this.Property(t => t.ModifiedDate).HasColumnName("ModifiedDate");
            this.Property(t => t.ModifiedBy).HasColumnName("ModifiedBy");
            this.Property(t => t.RowVersion).HasColumnName("RowVersion");
        }
    }
}
