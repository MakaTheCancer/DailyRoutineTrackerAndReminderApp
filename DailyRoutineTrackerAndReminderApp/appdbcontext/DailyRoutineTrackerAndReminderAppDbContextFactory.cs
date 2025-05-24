using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using DailyRoutineTrackerAndReminderApp.appdbcontext;


namespace DailyRoutineTrackerAndReminderApp.appdbcontext
{
    public class DailyRoutineTrackerAndReminderAppDbContextFactory : IDesignTimeDbContextFactory<DailyRoutineTrackerAndReminderAppDbContext>
    {
        public DailyRoutineTrackerAndReminderAppDbContext CreateDbContext(string[] args = null)
        {
            var options = new DbContextOptionsBuilder<DailyRoutineTrackerAndReminderAppDbContext>();
            options.UseSqlServer("Server=(localdb)\\ProjectModels;Database=DailyRoutineTrackerAndReminderAppDb;Trusted_Connection=True;TrustServerCertificate=True");

            return new DailyRoutineTrackerAndReminderAppDbContext(options.Options);
        }
    }
}
