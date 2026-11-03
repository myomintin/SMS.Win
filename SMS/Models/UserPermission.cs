using System;
using System.Collections.Generic;

namespace CodeflexSystems.SMS.Models
{
    public partial class UserPermission
    {
        public Guid UserId { get; set; }
        public Guid MenuId { get; set; }
        public Permission Permission { get; set; }
        public DateTime CreatedDate { get; set; }
        public Nullable<Guid> CreatedBy { get; set; }
        public DateTime ModifiedDate { get; set; }
        public Nullable<Guid> ModifiedBy { get; set; }
        public byte[] RowVersion { get; set; }

        public virtual User User { get; set; }
    }
}
