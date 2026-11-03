using System;
using System.Collections.Generic;

namespace CodeflexSystems.SMS.Models
{
    public partial class User
    {
        public User()
        {
            this.UserPermissions = new List<UserPermission>();
        }

        public Guid UserId { get; set; }
        public string UserName { get; set; }
        public string FullName { get; set; }
        public Nullable<DateTime> BirthDate { get; set; }
        public string EmailAddress { get; set; }
        public string Description { get; set; }
        public DateTime CreatedDate { get; set; }
        public Nullable<Guid> CreatedBy { get; set; }
        public DateTime ModifiedDate { get; set; }
        public Nullable<Guid> ModifiedBy { get; set; }
        public byte[] RowVersion { get; set; }

        public virtual UserMembership UserMembership { get; set; }
        public virtual UserOption UserOption { get; set; }
        public virtual ICollection<UserPermission> UserPermissions { get; set; }
    }
}
