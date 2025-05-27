using DailyRoutineTrackerAndReminderApp.Helpers;
using DailyRoutineTrackerAndReminderApp.Models;
using DailyRoutineTrackerAndReminderApp.appdbcontext;
using Microsoft.EntityFrameworkCore;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;

namespace DailyRoutineTrackerAndReminderApp.View
{
    /// <summary>
    /// Interaction logic for DeleteScheduleWindow.xaml
    /// </summary>
    public partial class DeleteScheduleWindow : Window
    {
        private readonly DailyRoutineTrackerAndReminderAppDbContext _context;
        private readonly int _userId;
        private ObservableCollection<ScheduleListItem> _schedules;

        public DeleteScheduleWindow(DailyRoutineTrackerAndReminderAppDbContext context, int userId)
        {
            InitializeComponent();
            _context = context;
            _userId = userId;

            LoadSchedules();
        }

        private void LoadSchedules()
        {
            var schedules = _context.Schedules
                .Where(s => s.UserId == _userId)
                .Select(s => new ScheduleListItem
                {
                    Id = s.ScheduleId,
                    Description = s.Description,
                    Type = s.Type,
                    Date = s.Date,
                    IsSelected = false
                })
                .ToList();

            _schedules = new ObservableCollection<ScheduleListItem>(schedules);
            ScheduleListView.ItemsSource = _schedules;
        }

        private void DeleteSelected_Click(object sender, RoutedEventArgs e)
        {
            var selectedIds = _schedules.Where(s => s.IsSelected).Select(s => s.Id).ToList();

            if (!selectedIds.Any())
            {
                MessageBox.Show("Please select at least one schedule to delete.");
                return;
            }

            var toRemove = _context.Schedules.Where(s => selectedIds.Contains(s.ScheduleId)).ToList();
            _context.Schedules.RemoveRange(toRemove);
            _context.SaveChanges();

            MessageBox.Show($"{toRemove.Count} schedule(s) deleted.");

            LoadSchedules(); // Refresh list
        }
    }
}
