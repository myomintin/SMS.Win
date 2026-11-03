using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace CodeflexSystems.SMS.Models.Mapping
{
    public sealed class GuardianMap : EntityTypeConfiguration<Guardian>
    {
        public GuardianMap()
        {
            // Primary Key
            this.HasKey(t => t.GuardianId);

            // Properties
            this.Property(t => t.GuardianId)
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);

            this.Property(t => t.Title)
                .HasMaxLength(15);

            this.Property(t => t.GuardianName)
                .IsRequired()
                .HasMaxLength(128);

            this.Property(t => t.Nationality)
                .HasMaxLength(128);

            this.Property(t => t.NIRC)
                .HasMaxLength(30);

            this.Property(t => t.Education)
                .HasMaxLength(128);

            this.Property(t => t.Employment)
                .HasMaxLength(128);

            this.Property(t => t.Designation)
                .HasMaxLength(50);

            this.Property(t => t.Passport)
                .HasMaxLength(20);

            this.Property(t => t.Visa)
                .HasMaxLength(30);

            this.Property(t => t.Description)
                .HasMaxLength(250);

            this.Property(t => t.RowVersion)
                .IsRequired()
                .IsFixedLength()
                .HasMaxLength(8)
                .IsRowVersion();

            // Table & Column Mappings
            this.ToTable("Guardians");
            this.Property(t => t.GuardianId).HasColumnName("GuardianId");
            this.Property(t => t.Title).HasColumnName("Title");
            this.Property(t => t.GuardianName).HasColumnName("GuardianName");
            this.Property(t => t.Gender).HasColumnName("Gender");
            this.Property(t => t.BirthDate).HasColumnName("BirthDate");
            this.Property(t => t.Nationality).HasColumnName("Nationality");
            this.Property(t => t.NIRC).HasColumnName("NIRC");
            this.Property(t => t.Education).HasColumnName("Education");
            this.Property(t => t.Employment).HasColumnName("Employment");
            this.Property(t => t.Designation).HasColumnName("Designation");
            this.Property(t => t.Passport).HasColumnName("Passport");
            this.Property(t => t.Visa).HasColumnName("Visa");
            this.Property(t => t.Description).HasColumnName("Description");
            this.Property(t => t.BirthCountryId).HasColumnName("BirthCountryId");
            this.Property(t => t.ReligionId).HasColumnName("ReligionId");
            this.Property(t => t.CreatedDate).HasColumnName("CreatedDate");
            this.Property(t => t.CreatedBy).HasColumnName("CreatedBy");
            this.Property(t => t.ModifiedDate).HasColumnName("ModifiedDate");
            this.Property(t => t.ModifiedBy).HasColumnName("ModifiedBy");
            this.Property(t => t.RowVersion).HasColumnName("RowVersion");

            // Relationships
            this.HasOptional(t => t.BirthCountry)
                .WithMany(t => t.Guardians)
                .HasForeignKey(d => d.BirthCountryId);
            this.HasOptional(t => t.Religion)
                .WithMany(t => t.Guardians)
                .HasForeignKey(d => d.ReligionId);
        }
    }
}
