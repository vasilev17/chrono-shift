using ChronoShift.Models.Tables;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ChronoShift.Models
{
    public class ActivityManager
    {
        public int GetRecordID(int userID, DateTime saveDate)
        {
            using (var context = new TraineeScheduleContext())
            {
                var records = context.Record
                    .Where(s => s.UserId == userID)
                    .Where(s => s.Date == saveDate)
                    .ToList();

                if (records.Any())
                {
                    return records.FirstOrDefault().Id;
                }
            }
            return -1;
        }

        public List<DayActivity> GetActivities(int recordID)
        {
            using (var context = new TraineeScheduleContext())
            {
                var activities = context.DayActivity
                    .Where(s => s.RecordId == recordID)
                    .ToList();

                return activities;
            }
        }

        public bool isActivityExisting(int recordID, int activityNum)
        {
            using (var context = new TraineeScheduleContext())
            {
                var findActivity = context.DayActivity
                    .Where(s => s.RecordId == recordID)
                    .Where(s => s.ActivityNumber == activityNum)
                    .ToList();

                return findActivity.Any();
            }
        }

        public void OverrideActivity(int recordID, int activityNum, TimeSpan saveTime, string activityDescription)
        {
            using (var context = new TraineeScheduleContext())
            {
                var findActivity = context.DayActivity
                    .Where(s => s.RecordId == recordID)
                    .Where(s => s.ActivityNumber == activityNum)
                    .ToList();

                var activity = findActivity.FirstOrDefault();
                
                activity.Time = saveTime;
                activity.Activity = activityDescription;
                context.SaveChanges();
            }
        }

        public void CreateActivity(int recordID, int activityNum, TimeSpan saveTime, string activityDescription)
        {
            using (var context = new TraineeScheduleContext())
            {
                var activity = new DayActivity
                {
                    RecordId = recordID,
                    Activity = activityDescription,
                    Time = saveTime,
                    ActivityNumber = activityNum
                };

                context.Add(activity);
                context.SaveChanges();
            }
        }

        public void CreateRecord(int userID, DateTime saveDate)
        {
            var databaseContext = new TraineeScheduleContext();
            var record = new Record
            {
                Date = saveDate,
                UserId = userID
            };
            databaseContext.Add(record);
            databaseContext.SaveChanges();
        }

    }
}
