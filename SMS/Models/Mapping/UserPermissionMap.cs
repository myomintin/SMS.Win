using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace CodeflexSystems.SMS.Models.Mapping
{
    public sealed class UserPermissionMap : EntityTypeConfiguration<UserPermission>
    {
        public UserPermissionMap()
        {
            // Primary Key
            this.HasKey(t => new { t.UserId, t.MenuId });

            // Properties
            this.Property(t => t.UserId)
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.None);

            this.Property(t => t.MenuId)
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.None);

            this.Property(t => t.RowVersion)
                .IsRequired()
                .IsFixedLength()
                .HasMaxLength(8)
                .IsRowVersion();

            // Table & Column Mappings
            this.ToTable("UserPermissions");
            this.Property(t => t.UserId).HasColumnName("UserId");
            this.Property(t => t.MenuId).HasColumnName("MenuId");
            this.Property(t => t.Permission).HasColumnName("Permission");
            this.Property(t => t.CreatedDate).HasColumnName("CreatedDate");
            this.Property(t => t.CreatedBy).HasColumnName("CreatedBy");
            this.Property(t => t.ModifiedDate).HasColumnName("ModifiedDate");
            this.Property(t => t.ModifiedBy).HasColumnName("ModifiedBy");
            this.Property(t => t.RowVersion).HasColumnName("RowVersion");

            // Relationships
            this.HasRequired(t => t.User)
                .WithMany(t => t.UserPermissions)
                .HasForeignKey(d => d.UserId);
        }
    }
}
