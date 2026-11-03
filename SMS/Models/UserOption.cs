using System;
using System.Collections.Generic;

namespace CodeflexSystems.SMS.Models
{
    public partial class UserOption
    {
        public Guid UserId { get; set; }
        public byte AccentColorA { get; set; }
        public byte AccentColorR { get; set; }
        public byte AccentColorG { get; set; }
        public byte AccentColorB { get; set; }
        public byte ThemeIndex { get; set; }
        public byte FontSize { get; set; }
        public byte TransitionEffect { get; set; }
        public int PageSize { get; set; }
        public string DatePattern { get; set; }
        public string TimePattern { get; set; }
        public byte DateTimeIndex { get; set; }
        public byte FirstDayOfWeek { get; set; }
        public int CurrencyPositivePattern { get; set; }
        public int CurrencyNegativePattern { get; set; }
        public byte[] RowVersion { get; set; }

        public virtual User User { get; set; }
    }
}
