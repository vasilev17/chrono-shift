using System;
using System.Collections.Generic;

namespace ChronoShift.Models.Tables
{
    public partial class Record
    {
        public Record()
        {
            DayActivity = new HashSet<DayActivity>();
        }

        public int Id { get; set; }
        public DateTime Date { get; set; }
        public int UserId { get; set; }

        public virtual User User { get; set; }
        public virtual ICollection<DayActivity> DayActivity { get; set; }
    }
}
