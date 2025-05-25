using DailyRoutineTrackerAndReminderApp.appdbcontext;
using DailyRoutineTrackerAndReminderApp.Models;
using DailyRoutineTrackerAndReminderApp.Services;
using Microsoft.Win32;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Media;
using System.Windows.Threading;
using System.Windows;
using Microsoft.EntityFrameworkCore;
using System.IO;
using System.Media;

namespace DailyRoutineTrackerAndReminderApp.View
{
    public partial class homepage : Page
    {
        private DispatcherTimer alarmTimer;
        // Removed MediaPlayer field since we don't use it anymore
        private readonly UserDataService _userDataService = new UserDataService(new DailyRoutineTrackerAndReminderAppDbContextFactory());
        private int userId;
        public homepage()
        {
            InitializeComponent();
            userId = CurrentUserService.GetUserId();
            Loaded += NavigationBar_Loaded;

            DescriptionTextBox.TextChanged += (s, e) =>
            {
                DescriptionPlaceholder.Visibility = string.IsNullOrWhiteSpace(DescriptionTextBox.Text) ? Visibility.Visible : Visibility.Collapsed;
            };

            StartAlarmChecker(); // Start the alarm check timer
        }

        private void StartAlarmChecker()
        {
            alarmTimer = new DispatcherTimer
            {
                Interval = TimeSpan.FromSeconds(1)  // Check every second
            };
            alarmTimer.Tick += async (s, e) => await CheckForAlarms();
            alarmTimer.Start();
        }

        private List<int> triggeredAlarms = new(); // to avoid triggering the same alarm repeatedly

        private async Task CheckForAlarms()
        {
            using var context = new DailyRoutineTrackerAndReminderAppDbContextFactory().CreateDbContext();

            var now = DateTime.Now;

            var upcomingAlarms = await context.Schedules
                .Where(s => s.UserId == userId && s.Type == "Alarm")
                .ToListAsync();

            var alarmsToTrigger = upcomingAlarms
                .Where(a => !triggeredAlarms.Contains(a.ScheduleId) &&
                            a.Date <= now &&
                            a.Date > now.AddSeconds(-1)) // Allow for 1s leeway
                .ToList();

            foreach (var alarm in alarmsToTrigger)
            {
                triggeredAlarms.Add(alarm.ScheduleId);
                TriggerAlarm(alarm);
            }
        }

        private MediaPlayer mediaPlayer = new MediaPlayer();

        private async void TriggerAlarm(Schedule alarm)
        {
            string soundPath = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Assets", "alarm.wav");

            if (!File.Exists(soundPath))
            {
                MessageBox.Show("Alarm sound file not found.");
                return;
            }

            try
            {
                SoundPlayer soundPlayer = new SoundPlayer(soundPath);

                // Start alarm sound in background
                var playTask = Task.Run(() =>
                {
                    soundPlayer.PlayLooping();
                });

                // Show message box on UI thread and wait
                MessageBox.Show($"Alarm: {alarm.Description}", "Alarm Triggered!", MessageBoxButton.OK, MessageBoxImage.Exclamation);

                // Stop sound after user clicks OK
                soundPlayer.Stop();

                // Remove alarm from database
                using var context = new DailyRoutineTrackerAndReminderAppDbContextFactory().CreateDbContext();
                var alarmToRemove = await context.Schedules.FindAsync(alarm.ScheduleId);
                if (alarmToRemove != null)
                {
                    context.Schedules.Remove(alarmToRemove);
                    await context.SaveChangesAsync();
                }

                await LoadSchedulesAsync(); // Refresh UI
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Failed to play alarm sound: {ex.Message}");
            }
        }


        private async void NavigationBar_Loaded(object sender, RoutedEventArgs e)
        {
            string imagePath = await _userDataService.GetProfilePictureByUserId(userId);
            string username = await _userDataService.GetUsernameById(userId);

            if (!string.IsNullOrEmpty(imagePath) && File.Exists(imagePath))
            {
                AvatarImage.Source = new BitmapImage(new Uri(imagePath, UriKind.Absolute));
            }

            Username.Text = username;

            // Load schedules here
            await LoadSchedulesAsync();
        }

        private void ClickCalendar(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("calendar");
        }

        private void ClickReminder(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("reminder");
        }

        private void ClickSettings(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("settings");
        }

        private async void ClickUploadPfp(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog
            {
                Title = "Select Profile Image",
                Filter = "Image Files (*.png;*.jpg;*.jpeg)|*.png;*.jpg;*.jpeg"
            };

            if (openFileDialog.ShowDialog() == true)
            {
                string selectedFilePath = openFileDialog.FileName;
                AvatarImage.Source = new BitmapImage(new Uri(selectedFilePath));

                try
                {
                    await _userDataService.UpdateProfilePicture(userId, selectedFilePath);
                    MessageBox.Show("Profile picture updated successfully!");
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"An error occurred: {ex.Message}");
                }
            }
        }

        private void ShowSlideUpPanel(object sender, RoutedEventArgs e)
        {
            var storyboard = (Storyboard)Resources["SlideUp"];
            storyboard.Begin();
        }

        private void HideSlideUpPanel(object sender, RoutedEventArgs e)
        {
            var storyboard = (Storyboard)Resources["SlideDown"];
            storyboard.Begin();
        }

        private async void SaveSchedule_Click(object sender, RoutedEventArgs e)
        {
            if (TypeComboBox.SelectedItem is ComboBoxItem selectedItem)
            {
                string type = selectedItem.Content.ToString();
                string description = DescriptionTextBox.Text.Trim();
                DateTime? date = DatePicker.SelectedDate;

                if (string.IsNullOrWhiteSpace(description))
                {
                    MessageBox.Show("Please enter a description.");
                    return;
                }

                if (date == null)
                {
                    MessageBox.Show("Please select a date.");
                    return;
                }

                DateTime scheduleDate;

                if (type == "Alarm")
                {
                    if (HourComboBox.SelectedItem == null
                        || MinuteComboBox.SelectedItem == null
                        || SecondComboBox.SelectedItem == null
                        || AmPmComboBox.SelectedItem == null)
                    {
                        MessageBox.Show("Please select a full time (hour, minute, second, AM/PM).");
                        return;
                    }

                    if (!int.TryParse(HourComboBox.SelectedItem.ToString(), out int hour))
                    {
                        MessageBox.Show("Invalid hour format.");
                        return;
                    }

                    int minute = int.Parse(MinuteComboBox.SelectedItem.ToString());
                    int second = int.Parse(SecondComboBox.SelectedItem.ToString());
                    string ampm = (AmPmComboBox.SelectedItem as ComboBoxItem)?.Content.ToString();

                    if (ampm == "PM" && hour < 12)
                        hour += 12;
                    else if (ampm == "AM" && hour == 12)
                        hour = 0;

                    TimeSpan time = new TimeSpan(hour, minute, second);
                    scheduleDate = date.Value.Date + time;
                }
                else
                {
                    scheduleDate = date.Value.Date;
                }

                // Save to DB
                using (var context = new DailyRoutineTrackerAndReminderAppDbContextFactory().CreateDbContext())
                {
                    var newSchedule = new Schedule
                    {
                        UserId = userId,
                        Type = type,
                        Description = description,
                        Date = scheduleDate
                    };

                    context.Schedules.Add(newSchedule);
                    await context.SaveChangesAsync();
                }

                // Reload list
                await LoadSchedulesAsync();

                // Reset form
                TypeComboBox.SelectedIndex = -1;
                DescriptionTextBox.Text = "";
                DatePicker.SelectedDate = null;
                HourComboBox.SelectedIndex = -1;
                MinuteComboBox.SelectedIndex = -1;
                SecondComboBox.SelectedIndex = -1;
                AmPmComboBox.SelectedIndex = -1;
                TimePickerGrid.Visibility = Visibility.Collapsed;

                HideSlideUpPanel(sender, e);
            }
            else
            {
                MessageBox.Show("Please select a schedule type.");
            }
        }

        private void TypeComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (TypeComboBox.SelectedItem is ComboBoxItem selectedItem)
            {
                string selectedType = selectedItem.Content.ToString();
                TimePickerGrid.Visibility = selectedType == "Alarm" ? Visibility.Visible : Visibility.Collapsed;
            }
        }

        private void HourComboBox_Loaded(object sender, RoutedEventArgs e)
        {
            var combo = sender as ComboBox;
            combo.Items.Clear();
            for (int i = 1; i <= 12; i++)  // 12-hour format for AM/PM
            {
                combo.Items.Add(i.ToString("D2"));
            }
        }

        private void MinuteComboBox_Loaded(object sender, RoutedEventArgs e)
        {
            var combo = sender as ComboBox;
            combo.Items.Clear();
            for (int i = 0; i < 60; i++)
            {
                combo.Items.Add(i.ToString("D2"));
            }
        }

        private void SecondComboBox_Loaded(object sender, RoutedEventArgs e)
        {
            var combo = sender as ComboBox;
            combo.Items.Clear();
            for (int i = 0; i < 60; i++)
            {
                combo.Items.Add(i.ToString("D2"));
            }
        }

        private void AmPmComboBox_Loaded(object sender, RoutedEventArgs e)
        {
            var combo = sender as ComboBox;
            combo.Items.Clear();
            combo.Items.Add(new ComboBoxItem() { Content = "AM" });
            combo.Items.Add(new ComboBoxItem() { Content = "PM" });
        }

        private async Task LoadSchedulesAsync()
        {
            using var context = new DailyRoutineTrackerAndReminderAppDbContextFactory().CreateDbContext();

            var schedules = await context.Schedules
                .Where(s => s.UserId == userId)
                .OrderBy(s => s.Date)
                .ToListAsync();

            SchedulesListView.ItemsSource = schedules;
        }

        private void Overlay_MouseDown(object sender, MouseButtonEventArgs e)
        {
            HideSlideUpPanel(sender, e);
        }
    }
}
