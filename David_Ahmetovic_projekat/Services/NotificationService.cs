using Plugin.LocalNotification;
using David_Ahmetovic_projekat.Models;

namespace David_Ahmetovic_projekat.Services
{
    public static class NotificationService
    {
        public static async Task RequestNotificationPermissionAsync()
        {
            if (await LocalNotificationCenter.Current.AreNotificationsEnabled() == false)
            {
                await LocalNotificationCenter.Current.RequestNotificationPermission();
            }
        }

        public static async Task ScheduleEventNotificationAsync(CalendarEvent calendarEvent)
        {
            await RequestNotificationPermissionAsync();

            // Notifikaciju zakazujemo dan ranije u 20:00 h
            DateTime notifyTime = calendarEvent.Date.Date.AddDays(-1).AddHours(20);

            // Ako je vrijeme već prošlo (npr. obaveza je za danas), stavi za 1 minut od sada
            if (notifyTime <= DateTime.Now)
            {
                notifyTime = DateTime.Now.AddMinutes(1);
            }

            var request = new NotificationRequest
            {
                NotificationId = calendarEvent.Id != 0 ? calendarEvent.Id : new Random().Next(1000, 99999),
                Title = $"Podsjetnik: {calendarEvent.Type} sutra!",
                Description = $"{calendarEvent.SubjectName}: {calendarEvent.Title}",
                Schedule = new NotificationRequestSchedule
                {
                    NotifyTime = notifyTime
                }
            };

            await LocalNotificationCenter.Current.Show(request);
        }
    }
}