
using DailyRoutineTrackerAndReminderApp.appdbcontext;
using DailyRoutineTrackerAndReminderApp.Models;
using DailyRoutineTrackerAndReminderApp.Services;
using Microsoft.EntityFrameworkCore;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace DailyRoutineTrackerAndReminderApp.View
{
    public partial class signup : Page
    {
        public signup()
        {
            InitializeComponent();
        }

        private async void Register_Click(object sender, RoutedEventArgs e)
        {
            string email = EmailTextBox.Text.Trim();
            string password = passwordtext.Text.Trim();
            string username = Username.Text.Trim();

            if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(password) || string.IsNullOrWhiteSpace(username))
            {
                MessageBox.Show("Please fill in all fields.");
                return;
            }

            try
            {
                var factory = new DailyRoutineTrackerAndReminderAppDbContextFactory();
                using (var context = factory.CreateDbContext())
                {
                    bool userExists = await context.Users.AnyAsync(u => u.Email == email || u.Username == username);
                    if (userExists)
                    {
                        MessageBox.Show("Email or username already exists.");
                        return;
                    }

                    var newUser = new User
                    {
                        Email = email,
                        Username = username,
                        Password = password, // Plain text — consider hashing in production
                        DateJoined = DateTime.Now,
                        ProfileImage = null
                    };

                    context.Users.Add(newUser);
                    await context.SaveChangesAsync();

                    MessageBox.Show("Registration successful.");
                    Services.NavigationService.NavigateTo(new login());
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("An error occurred during registration: " + ex.Message);
            }
        }


        private void Username_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (Username.Text == "Username")
                Username.Text = "";
        }

        private void EmailTextBox_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (EmailTextBox.Text == "Email Address")
                EmailTextBox.Text = "";
        }

        private void passwordtext_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (passwordtext.Text == "Password")
                passwordtext.Text = "";
        }

        private void Back_Click(object sender, RoutedEventArgs e)
        {
            Services.NavigationService.NavigateTo(new login());
        }
    }
}
