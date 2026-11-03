using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace CodeflexSystems.SMS.Models.Mapping
{
    public sealed class UserOptionMap : EntityTypeConfiguration<UserOption>
    {
        public UserOptionMap()
        {
            // Primary Key
            this.HasKey(t => t.UserId);

            // Properties
            this.Property(t => t.UserId)
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.None);

            this.Property(t => t.DatePattern)
                .IsRequired()
                .HasMaxLength(30);

            this.Property(t => t.TimePattern)
                .IsRequired()
                .HasMaxLength(20);

            this.Property(t => t.RowVersion)
                .IsRequired()
                .IsFixedLength()
                .HasMaxLength(8)
                .IsRowVersion();

            // Table & Column Mappings
            this.ToTable("UserOptions");
            this.Property(t => t.UserId).HasColumnName("UserId");
            this.Property(t => t.AccentColorA).HasColumnName("AccentColorA");
            this.Property(t => t.AccentColorR).HasColumnName("AccentColorR");
            this.Property(t => t.AccentColorG).HasColumnName("AccentColorG");
            this.Property(t => t.AccentColorB).HasColumnName("AccentColorB");
            this.Property(t => t.ThemeIndex).HasColumnName("ThemeIndex");
            this.Property(t => t.FontSize).HasColumnName("FontSize");
            this.Property(t => t.TransitionEffect).HasColumnName("TransitionEffect");
            this.Property(t => t.PageSize).HasColumnName("PageSize");
            this.Property(t => t.DatePattern).HasColumnName("DatePattern");
            this.Property(t => t.TimePattern).HasColumnName("TimePattern");
            this.Property(t => t.DateTimeIndex).HasColumnName("DateTimeIndex");
            this.Property(t => t.FirstDayOfWeek).HasColumnName("FirstDayOfWeek");
            this.Property(t => t.CurrencyPositivePattern).HasColumnName("CurrencyPositivePattern");
            this.Property(t => t.CurrencyNegativePattern).HasColumnName("CurrencyNegativePattern");
            this.Property(t => t.RowVersion).HasColumnName("RowVersion");

            // Relationships
            this.HasRequired(t => t.User)
                .WithOptional(t => t.UserOption);
        }
    }
}
