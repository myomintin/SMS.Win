using System;
using System.Collections.Generic;

namespace CodeflexSystems.SMS.Models
{
    public partial class StudentPhone : Phone
    {
        public Guid StudentId { get; set; }

        public virtual Student Student { get; set; }
    }

    //public partial class StudentPhone
    //{
    //    public Guid PhoneId { get; set; }
    //    public string PhoneNumber { get; set; }
    //    public string Extension { get; set; }
    //    public Guid StudentId { get; set; }
    //    public Guid PhoneTypeId { get; set; }
    //    public DateTime CreatedDate { get; set; }
    //    public Nullable<Guid> CreatedBy { get; set; }
    //    public DateTime ModifiedDate { get; set; }
    //    public Nullable<Guid> ModifiedBy { get; set; }
    //    public byte[] RowVersion { get; set; }
    //    public virtual PhoneType PhoneType { get; set; }
    //    public virtual Student Student { get; set; }
    //}
}
