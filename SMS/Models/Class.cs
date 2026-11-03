using System;
using System.Collections.Generic;

namespace CodeflexSystems.SMS.Models
{
    public partial class Class
    {
        public Class()
        {
            this.Sections = new List<Section>();
        }

        public Guid ClassId { get; set; }
        public string ClassName { get; set; }
        public string Description { get; set; }
        public Guid CourseId { get; set; }
        public DateTime CreatedDate { get; set; }
        public Nullable<Guid> CreatedBy { get; set; }
        public DateTime ModifiedDate { get; set; }
        public Nullable<Guid> ModifiedBy { get; set; }
        public byte[] RowVersion { get; set; }

        public virtual Course Course { get; set; }
        public virtual ICollection<Section> Sections { get; set; }
    }
}
