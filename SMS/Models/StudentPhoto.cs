using System;
using System.Collections.Generic;

namespace CodeflexSystems.SMS.Models
{
    public partial class StudentPhoto
    {
        public Guid StudentId { get; set; }
        public byte[] Photo { get; set; }
        public byte[] RowVersion { get; set; }

        public virtual Student Student { get; set; }
    }
}
