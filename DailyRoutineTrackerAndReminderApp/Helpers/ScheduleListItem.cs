using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DailyRoutineTrackerAndReminderApp.Helpers
{
    public class ScheduleListItem
    {
        public int Id { get; set; }
        public string Description { get; set; } // should match Schedule.Description
        public string Type { get; set; }
        public DateTime Date { get; set; }
        public bool IsSelected { get; set; }
    }

}
