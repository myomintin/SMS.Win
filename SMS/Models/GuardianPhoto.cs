using System;
using System.Collections.Generic;

namespace CodeflexSystems.SMS.Models
{
    public partial class GuardianPhoto
    {
        public Guid GuardianId { get; set; }
        public byte[] Photo { get; set; }
        public byte[] RowVersion { get; set; }

        public virtual Guardian Guardian { get; set; }
    }
}
