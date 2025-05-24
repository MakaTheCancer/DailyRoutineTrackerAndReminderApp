using DailyRoutineTrackerAndReminderApp.Services;
using DailyRoutineTrackerAndReminderApp.View;
using System.Windows;

namespace DailyRoutineTrackerAndReminderApp
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            NavigationService.MainFrame = Main;
            Main.Navigate(new login());

        }

    }
}