using System;
using System.Collections.Generic;

namespace ChronoShift.Models.Tables
{
    public partial class DayActivity
    {
        public int Id { get; set; }
        public int RecordId { get; set; }
        public int ActivityNumber { get; set; }
        public TimeSpan Time { get; set; }
        public string Activity { get; set; }

        public virtual Record Record { get; set; }
    }
}
