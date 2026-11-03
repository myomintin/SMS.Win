using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace CodeflexSystems.SMS.Models.Mapping
{
    public sealed class StudentMap : EntityTypeConfiguration<Student>
    {
        public StudentMap()
        {
            // Primary Key
            this.HasKey(t => t.StudentId);

            // Properties
            this.Property(t => t.StudentId)
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);

            this.Property(t => t.Id)
                .HasMaxLength(10);

            this.Property(t => t.Title)
                .HasMaxLength(15);

            this.Property(t => t.StudentName)
                .IsRequired()
                .HasMaxLength(128);

            this.Property(t => t.OtherName)
                .HasMaxLength(128);

            this.Property(t => t.Nationality)
                .HasMaxLength(128);

            this.Property(t => t.NIRC)
                .HasMaxLength(30);

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
            this.ToTable("Students");
            this.Property(t => t.StudentId).HasColumnName("StudentId");
            this.Property(t => t.Id).HasColumnName("Id");
            this.Property(t => t.Title).HasColumnName("Title");
            this.Property(t => t.StudentName).HasColumnName("StudentName");
            this.Property(t => t.OtherName).HasColumnName("OtherName");
            this.Property(t => t.Gender).HasColumnName("Gender");
            this.Property(t => t.BirthDate).HasColumnName("BirthDate");
            this.Property(t => t.Nationality).HasColumnName("Nationality");
            this.Property(t => t.NIRC).HasColumnName("NIRC");
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
                .WithMany(t => t.Students)
                .HasForeignKey(d => d.BirthCountryId);
            this.HasOptional(t => t.Religion)
                .WithMany(t => t.Students)
                .HasForeignKey(d => d.ReligionId);
        }
    }
}
