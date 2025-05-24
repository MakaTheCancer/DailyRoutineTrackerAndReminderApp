using DailyRoutineTrackerAndReminderApp.Models;
using Microsoft.EntityFrameworkCore;

namespace DailyRoutineTrackerAndReminderApp.appdbcontext
{
    public class DailyRoutineTrackerAndReminderAppDbContext : DbContext
    {

        public DbSet<User> Users { get; set; }
        public DbSet<Schedule> Schedules { get; set; }

        public DailyRoutineTrackerAndReminderAppDbContext(DbContextOptions options) : base(options) { }

    }
}
