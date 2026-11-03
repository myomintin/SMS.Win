using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace CodeflexSystems.SMS.Models.Mapping
{
    public sealed class UserMembershipMap : EntityTypeConfiguration<UserMembership>
    {
        public UserMembershipMap()
        {
            // Primary Key
            this.HasKey(t => t.UserId);

            // Properties
            this.Property(t => t.UserId)
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.None);

            this.Property(t => t.Password)
                .IsRequired()
                .HasMaxLength(128);

            this.Property(t => t.PasswordQuestion)
                .HasMaxLength(128);

            this.Property(t => t.PasswordAnswer)
                .HasMaxLength(128);

            this.Property(t => t.RowVersion)
                .IsRequired()
                .IsFixedLength()
                .HasMaxLength(8)
                .IsRowVersion();

            // Table & Column Mappings
            this.ToTable("UserMemberships");
            this.Property(t => t.UserId).HasColumnName("UserId");
            this.Property(t => t.Disabled).HasColumnName("Disabled");
            this.Property(t => t.DisabledDate).HasColumnName("DisabledDate");
            this.Property(t => t.LogonDate).HasColumnName("LogonDate");
            this.Property(t => t.PasswordFormat).HasColumnName("PasswordFormat");
            this.Property(t => t.Password).HasColumnName("Password");
            this.Property(t => t.PasswordModifiedDate).HasColumnName("PasswordModifiedDate");
            this.Property(t => t.PasswordFailures).HasColumnName("PasswordFailures");
            this.Property(t => t.PasswordFailureDate).HasColumnName("PasswordFailureDate");
            this.Property(t => t.PasswordQuestion).HasColumnName("PasswordQuestion");
            this.Property(t => t.PasswordAnswer).HasColumnName("PasswordAnswer");
            this.Property(t => t.PasswordAnswerModifiedDate).HasColumnName("PasswordAnswerModifiedDate");
            this.Property(t => t.PasswordAnswerFailures).HasColumnName("PasswordAnswerFailures");
            this.Property(t => t.PasswordAnswerFailureDate).HasColumnName("PasswordAnswerFailureDate");
            this.Property(t => t.RowVersion).HasColumnName("RowVersion");

            // Relationships
            this.HasRequired(t => t.User)
                .WithOptional(t => t.UserMembership);
        }
    }
}
