using DailyRoutineTrackerAndReminderApp.Models;

namespace DailyRoutineTrackerAndReminderApp.Services
{
    public static class CurrentUserService
    {
        public static CurrentUser LoggedInUser { get; private set; }

        public static void SetUser(CurrentUser user)
        {
            LoggedInUser = user;
        }

        public static int GetUserId()
        {
            return LoggedInUser?.CurrentUserId ?? throw new InvalidOperationException("No user is logged in.");
        }
    }


}
