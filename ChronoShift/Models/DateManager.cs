using ChronoShift.Models.Tables;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ChronoShift.Controllers
{
    public class DateManager
    {
        public int GetNumberOfDates(DateTime from, DateTime to)
        {
            if (to < from)
                throw new ArgumentException("To cannot be smaller than from.", nameof(to));

            if (to.Date == from.Date)
                return 0;

            int n = 0;
            DateTime nextDate = from;
            while (nextDate <= to.Date)
            {
                if (nextDate.DayOfWeek != DayOfWeek.Saturday && nextDate.DayOfWeek != DayOfWeek.Sunday && nextDate.Date != new DateTime(2021, 01, 01) && nextDate.Date != new DateTime(2021, 03, 03) && nextDate.Date != new DateTime(2021, 04, 30) && nextDate.Date != new DateTime(2021, 05, 03) && nextDate.Date != new DateTime(2021, 05, 04) && nextDate.Date != new DateTime(2021, 05, 06) && nextDate.Date != new DateTime(2021, 05, 24) && nextDate.Date != new DateTime(2021, 09, 06) && nextDate.Date != new DateTime(2021, 09, 22) && nextDate.Date != new DateTime(2021, 12, 24) && nextDate.Date != new DateTime(2021, 12, 27) && nextDate.Date != new DateTime(2021, 12, 28))
                    n++;
                nextDate = nextDate.AddDays(1);
            }

            return n;
        }

        public TimeSpan CalculateHoursForMonth(int id, int month)
        {
            TimeSpan totalHours = new TimeSpan(0, 0, 0, 0);

            using (var context = new TraineeScheduleContext())
            {
                //var record = context.Records.Where(s => s.UserId == id);
                //Tommorow we will work on that don't worry :D
                var record = context.Record
                    .Where(s => s.UserId == id)
                    .Where(s => s.Date.Month == month)
                    .ToList();

                for (int i = 0; i < record.Count(); i++)
                {
                    var activity = context.DayActivity
                        .Where(s => s.RecordId == record[i].Id)
                        .ToList();
                    for (int j = 0; j < activity.Count(); j++)
                    {
                        totalHours = totalHours.Add(activity[j].Time);
                    }
                }
            }

            return totalHours;
        }
    }
}
