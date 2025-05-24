using System.Windows;
using DailyRoutineTrackerAndReminderApp.appdbcontext;
using Microsoft.EntityFrameworkCore;

namespace DailyRoutineTrackerAndReminderApp
{
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
           
            var dbContext = new DailyRoutineTrackerAndReminderAppDbContextFactory().CreateDbContext();

            dbContext.Database.Migrate();

         
            Window window = new MainWindow();
            window.Show();

            base.OnStartup(e);
        }
    }
}
