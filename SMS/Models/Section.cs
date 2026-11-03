using System;
using System.Collections.Generic;

namespace CodeflexSystems.SMS.Models
{
    public partial class Section
    {
        public Section()
        {
            this.StudentSections = new List<StudentSection>();
        }

        public Guid SectionId { get; set; }
        public string SectionName { get; set; }
        public string Room { get; set; }
        public string OpenDay { get; set; }
        public Nullable<System.TimeSpan> StartTime { get; set; }
        public Nullable<System.TimeSpan> EndTime { get; set; }
        public Nullable<DateTime> CommencingDate { get; set; }
        public Nullable<DateTime> CompletionDate { get; set; }
        public decimal? Fee { get; set; }
        public bool Monthly { get; set; }
        public string Description { get; set; }
        public Guid ClassId { get; set; }
        public Nullable<Guid> CurrencyId { get; set; }
        public DateTime CreatedDate { get; set; }
        public Nullable<Guid> CreatedBy { get; set; }
        public DateTime ModifiedDate { get; set; }
        public Nullable<Guid> ModifiedBy { get; set; }
        public byte[] RowVersion { get; set; }

        public virtual Class Class { get; set; }
        public virtual Currency Currency { get; set; }
        public virtual ICollection<StudentSection> StudentSections { get; set; }
    }
}
