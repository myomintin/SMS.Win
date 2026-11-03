using System;
using System.Collections.Generic;

namespace CodeflexSystems.SMS.Models
{
    public partial class Religion
    {
        public Religion()
        {
            this.Guardians = new List<Guardian>();
            this.Students = new List<Student>();
        }

        public Guid ReligionId { get; set; }
        public string ReligionName { get; set; }
        public DateTime CreatedDate { get; set; }
        public Nullable<Guid> CreatedBy { get; set; }
        public DateTime ModifiedDate { get; set; }
        public Nullable<Guid> ModifiedBy { get; set; }
        public byte[] RowVersion { get; set; }

        public virtual ICollection<Guardian> Guardians { get; set; }
        public virtual ICollection<Student> Students { get; set; }
    }
}
