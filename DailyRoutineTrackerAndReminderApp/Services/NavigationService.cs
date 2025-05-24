using System.Windows.Controls;

namespace DailyRoutineTrackerAndReminderApp.Services
{
    public class NavigationService
    {
        public static Frame MainFrame { get; set; }

        public static void NavigateTo(Page page)
        {
            MainFrame?.Navigate(page);
        }
    }
}