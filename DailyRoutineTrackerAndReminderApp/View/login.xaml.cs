using DailyRoutineTrackerAndReminderApp.appdbcontext;
using DailyRoutineTrackerAndReminderApp.Models;
using DailyRoutineTrackerAndReminderApp.Services;
using Microsoft.EntityFrameworkCore;
using System.Net.Http;
using System.Text;
using System.Windows;
using System.Windows.Controls;

namespace DailyRoutineTrackerAndReminderApp.View
{
    /// <summary>
    /// Interaction logic for LogInNav.xaml
    /// </summary>
    public partial class login : Page
    {
        private readonly GenericDataService<User> _userService;
        public login()
        {
            InitializeComponent();
            _userService = new GenericDataService<User>(new DailyRoutineTrackerAndReminderAppDbContextFactory());
        }



        private async void LoginButton_Click(object sender, RoutedEventArgs e)
        {
            string email = EmailTextBox.Text.Trim();
            string password = passwordtext.Text.Trim();

            if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(password))
            {
                MessageBox.Show("Please enter both email and password.");
                return;
            }

            try
            {
                var factory = new DailyRoutineTrackerAndReminderAppDbContextFactory();
                using (var context = factory.CreateDbContext())
                {
                    var user = await context.Users.FirstOrDefaultAsync(u => u.Email == email && u.Password == password);

                    if (user == null)
                    {
                        MessageBox.Show("Invalid email or password.");
                        return;
                    }

                    CurrentUserService.SetUser(new CurrentUser
                    {
                        CurrentUserId = user.UserId,
                        Email = user.Email
                    });

                    MessageBox.Show("Login successful.");
                    Services.NavigationService.NavigateTo(new homepage());
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("An error occurred during login: " + ex.Message);
            }

        }


        private void EmailTextBox_PreviewMouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if (EmailTextBox.Text == "Email Address")
                EmailTextBox.Clear();
        }

        private void passwordtext_PreviewMouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if (passwordtext.Text == "Password")
                passwordtext.Clear();
        }

        private void Register_Click(object sender, RoutedEventArgs e)
        {
            Services.NavigationService.NavigateTo(new signup());
        }
    }
}
