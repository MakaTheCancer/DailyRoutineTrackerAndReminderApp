using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using System;
using System.IO;

namespace DailyRoutineTrackerAndReminderApp.appdbcontext
{
    public class DailyRoutineTrackerAndReminderAppDbContextFactory : IDesignTimeDbContextFactory<DailyRoutineTrackerAndReminderAppDbContext>
    {
        public DailyRoutineTrackerAndReminderAppDbContext CreateDbContext(string[] args = null)
        {

            string localAppData = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);

            string appFolder = Path.Combine(localAppData, "DailyRoutineTrackerAndReminderApp");

            Directory.CreateDirectory(appFolder);
       
            string dbPath = Path.Combine(appFolder, "DailyRoutineTrackerAndReminderApp.db");

            var options = new DbContextOptionsBuilder<DailyRoutineTrackerAndReminderAppDbContext>();
            options.UseSqlite($"Data Source={dbPath}");

            return new DailyRoutineTrackerAndReminderAppDbContext(options.Options);
        }
    }
}
