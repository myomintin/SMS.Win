using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace CodeflexSystems.SMS.Models.Mapping
{
    public sealed class FeeTypeMap : EntityTypeConfiguration<FeeType>
    {
        public FeeTypeMap()
        {
            // Primary Key
            this.HasKey(t => t.FeeTypeId);

            // Properties
            this.Property(t => t.FeeTypeId)
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);

            this.Property(t => t.FeeTypeName)
                .IsRequired()
                .HasMaxLength(128);

            this.Property(t => t.RowVersion)
                .IsRequired()
                .IsFixedLength()
                .HasMaxLength(8)
                .IsRowVersion();

            // Table & Column Mappings
            this.ToTable("FeeTypes");
            this.Property(t => t.FeeTypeId).HasColumnName("FeeTypeId");
            this.Property(t => t.FeeTypeName).HasColumnName("FeeTypeName");
            this.Property(t => t.CreatedDate).HasColumnName("CreatedDate");
            this.Property(t => t.CreatedBy).HasColumnName("CreatedBy");
            this.Property(t => t.ModifiedDate).HasColumnName("ModifiedDate");
            this.Property(t => t.ModifiedBy).HasColumnName("ModifiedBy");
            this.Property(t => t.RowVersion).HasColumnName("RowVersion");
        }
    }
}
