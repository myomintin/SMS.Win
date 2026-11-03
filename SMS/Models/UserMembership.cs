using System;
using System.Collections.Generic;

namespace CodeflexSystems.SMS.Models
{
    public partial class UserMembership
    {
        public Guid UserId { get; set; }
        public bool Disabled { get; set; }
        public Nullable<DateTime> DisabledDate { get; set; }
        public Nullable<DateTime> LogonDate { get; set; }
        public PasswordFormat PasswordFormat { get; set; }
        public string Password { get; set; }
        public Nullable<DateTime> PasswordModifiedDate { get; set; }
        public byte PasswordFailures { get; set; }
        public Nullable<DateTime> PasswordFailureDate { get; set; }
        public string PasswordQuestion { get; set; }
        public string PasswordAnswer { get; set; }
        public Nullable<DateTime> PasswordAnswerModifiedDate { get; set; }
        public byte PasswordAnswerFailures { get; set; }
        public Nullable<DateTime> PasswordAnswerFailureDate { get; set; }
        public byte[] RowVersion { get; set; }

        public virtual User User { get; set; }
    }
}
