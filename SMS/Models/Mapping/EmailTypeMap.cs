using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace CodeflexSystems.SMS.Models.Mapping
{
    public sealed class EmailTypeMap : EntityTypeConfiguration<EmailType>
    {
        public EmailTypeMap()
        {
            // Primary Key
            this.HasKey(t => t.EmailTypeId);

            // Properties
            this.Property(t => t.EmailTypeId)
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);

            this.Property(t => t.EmailTypeName)
                .IsRequired()
                .HasMaxLength(128);

            this.Property(t => t.RowVersion)
                .IsRequired()
                .IsFixedLength()
                .HasMaxLength(8)
                .IsRowVersion();

            // Table & Column Mappings
            this.ToTable("EmailTypes");
            this.Property(t => t.EmailTypeId).HasColumnName("EmailTypeId");
            this.Property(t => t.EmailTypeName).HasColumnName("EmailTypeName");
            this.Property(t => t.CreatedDate).HasColumnName("CreatedDate");
            this.Property(t => t.CreatedBy).HasColumnName("CreatedBy");
            this.Property(t => t.ModifiedDate).HasColumnName("ModifiedDate");
            this.Property(t => t.ModifiedBy).HasColumnName("ModifiedBy");
            this.Property(t => t.RowVersion).HasColumnName("RowVersion");
        }
    }
}
