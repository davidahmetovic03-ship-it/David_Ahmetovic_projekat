using David_Ahmetovic_projekat.Models;
using David_Ahmetovic_projekat.Services;

namespace David_Ahmetovic_projekat;

public partial class CalendarPage : ContentPage
{
    private readonly DatabaseService _databaseService;

    public CalendarPage()
    {
        InitializeComponent();
        _databaseService = new DatabaseService();
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        await LoadEventsForSelectedDate();
    }

    private async Task LoadEventsForSelectedDate()
    {
        try
        {
            DateTime selectedDate = EventDatePicker.Date;
            var events = await _databaseService.GetCalendarEventsByDateAsync(selectedDate);
            EventsCollectionView.ItemsSource = events;
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Greška pri učitavanju kalendara: {ex.Message}");
        }
    }

    private async void OnDateSelected(object sender, DateChangedEventArgs e)
    {
        await LoadEventsForSelectedDate();
    }

    private async void OnAddEventClicked(object sender, EventArgs e)
    {
        try
        {
            // 1. Izbor tipa obaveze
            string[] types = { "Kontrolni", "Pismeni", "Domaći", "Obavještenje" };
            string selectedType = await DisplayActionSheet("Izaberite tip obaveze:", "Odustani", null, types);
            if (selectedType == "Odustani" || string.IsNullOrEmpty(selectedType)) return;

            // 2. Izbor predmeta
            int currentUserId = Preferences.Get("CurrentUserId", -1);
            var subjects = await _databaseService.GetUserSubjectsAsync(currentUserId);
            string subjectName = "Opšte";

            if (subjects != null && subjects.Count > 0)
            {
                string[] subjectNames = subjects.Select(s => s.Name).ToArray();
                string selectedSub = await DisplayActionSheet("Izaberite predmet:", "Odustani", null, subjectNames);
                if (selectedSub != "Odustani" && !string.IsNullOrEmpty(selectedSub))
                {
                    subjectName = selectedSub;
                }
            }

            // 3. Naslov i Opis
            string title = await DisplayPromptAsync("Naslov obaveze", "Unesite naslov obaveze (npr. Kviz lekcija 3):");
            if (string.IsNullOrWhiteSpace(title)) return;

            string description = await DisplayPromptAsync("Opis obaveze", "Dodatne napomene (opcionalno):");

            var newEvent = new CalendarEvent
            {
                Title = title.Trim(),
                Type = selectedType,
                SubjectName = subjectName,
                Description = description?.Trim() ?? "",
                Date = EventDatePicker.Date,
                UserId = currentUserId
            };

            // Čuvanje u bazi
            await _databaseService.SaveCalendarEventAsync(newEvent);

            // Osvežavanje prikaza
            await LoadEventsForSelectedDate();

            // Sigurno zakazivanje notifikacije
            try
            {
                await NotificationService.ScheduleEventNotificationAsync(newEvent);
            }
            catch (Exception notifEx)
            {
                System.Diagnostics.Debug.WriteLine($"Notifikacija nije mogla biti zakazana: {notifEx.Message}");
            }

            await DisplayAlert("Uspeh", "Obaveza je uspešno sačuvana!", "OK");
        }
        catch (Exception ex)
        {
            await DisplayAlert("Greška", $"Došlo je do greške pri čuvanju: {ex.Message}", "OK");
        }
    }

    private async void OnEditEventClicked(object sender, EventArgs e)
    {
        try
        {
            var button = sender as Button;
            var calendarEvent = button?.CommandParameter as CalendarEvent;

            if (calendarEvent == null) return;

            // 1. Izmena tipa
            string[] types = { "Kontrolni", "Pismeni", "Domaći", "Obavještenje" };
            string newType = await DisplayActionSheet("Izmeni tip obaveze:", "Zadrži postojeći", null, types);
            if (newType != "Zadrži postojeći" && !string.IsNullOrEmpty(newType))
            {
                calendarEvent.Type = newType;
            }

            // 2. Izmena naslova
            string newTitle = await DisplayPromptAsync("Uredi naslov", "Novi naslov obaveze:", initialValue: calendarEvent.Title);
            if (!string.IsNullOrWhiteSpace(newTitle))
            {
                calendarEvent.Title = newTitle.Trim();
            }

            // 3. Izmena opisa
            string newDescription = await DisplayPromptAsync("Uredi opis", "Novi opis obaveze:", initialValue: calendarEvent.Description);
            if (newDescription != null)
            {
                calendarEvent.Description = newDescription.Trim();
            }

            // Čuvanje promena u bazi
            await _databaseService.SaveCalendarEventAsync(calendarEvent);

            // Osvežavanje ekrana
            await LoadEventsForSelectedDate();
        }
        catch (Exception ex)
        {
            await DisplayAlert("Greška", $"Nije uspela izmena: {ex.Message}", "OK");
        }
    }

    private async void OnDeleteEventClicked(object sender, EventArgs e)
    {
        try
        {
            var button = sender as Button;
            var calendarEvent = button?.CommandParameter as CalendarEvent;

            if (calendarEvent != null)
            {
                bool confirm = await DisplayAlert("Potvrda", $"Želite li obrisati '{calendarEvent.Title}'?", "Da", "Ne");
                if (confirm)
                {
                    await _databaseService.DeleteCalendarEventAsync(calendarEvent);
                    await LoadEventsForSelectedDate();
                }
            }
        }
        catch (Exception ex)
        {
            await DisplayAlert("Greška", $"Nije uspelo brisanje: {ex.Message}", "OK");
        }
    }
}