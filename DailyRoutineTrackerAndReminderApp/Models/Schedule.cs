using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DailyRoutineTrackerAndReminderApp.Models
{
    public class Schedule
    {
        public int ScheduleId { get; set; }
        public string Type { get; set; }
        public string Description { get; set; }
        public DateTime Date { get; set; }

        // Add user tracking
        public int UserId { get; set; }
        public User User { get; set; }
    }

}
