using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace CodeflexSystems.SMS.Models.Mapping
{
    public sealed class SectionMap : EntityTypeConfiguration<Section>
    {
        public SectionMap()
        {
            // Primary Key
            this.HasKey(t => t.SectionId);

            // Properties
            this.Property(t => t.SectionId)
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);

            this.Property(t => t.SectionName)
                .IsRequired()
                .HasMaxLength(128);

            this.Property(t => t.Room)
                .HasMaxLength(25);

            this.Property(t => t.OpenDay)
                .HasMaxLength(50);

            this.Property(t => t.Description)
                .HasMaxLength(250);

            this.Property(t => t.RowVersion)
                .IsRequired()
                .IsFixedLength()
                .HasMaxLength(8)
                .IsRowVersion();

            // Table & Column Mappings
            this.ToTable("Sections");
            this.Property(t => t.SectionId).HasColumnName("SectionId");
            this.Property(t => t.SectionName).HasColumnName("SectionName");
            this.Property(t => t.Room).HasColumnName("Room");
            this.Property(t => t.OpenDay).HasColumnName("OpenDay");
            this.Property(t => t.StartTime).HasColumnName("StartTime");
            this.Property(t => t.EndTime).HasColumnName("EndTime");
            this.Property(t => t.CommencingDate).HasColumnName("CommencingDate");
            this.Property(t => t.CompletionDate).HasColumnName("CompletionDate");
            this.Property(t => t.Fee).HasColumnName("Fee");
            this.Property(t => t.Monthly).HasColumnName("Monthly");
            this.Property(t => t.Description).HasColumnName("Description");
            this.Property(t => t.ClassId).HasColumnName("ClassId");
            this.Property(t => t.CurrencyId).HasColumnName("CurrencyId");
            this.Property(t => t.CreatedDate).HasColumnName("CreatedDate");
            this.Property(t => t.CreatedBy).HasColumnName("CreatedBy");
            this.Property(t => t.ModifiedDate).HasColumnName("ModifiedDate");
            this.Property(t => t.ModifiedBy).HasColumnName("ModifiedBy");
            this.Property(t => t.RowVersion).HasColumnName("RowVersion");

            // Relationships
            this.HasRequired(t => t.Class)
                .WithMany(t => t.Sections)
                .HasForeignKey(d => d.ClassId);
            this.HasOptional(t => t.Currency)
                .WithMany(t => t.Sections)
                .HasForeignKey(d => d.CurrencyId);
        }
    }
}
