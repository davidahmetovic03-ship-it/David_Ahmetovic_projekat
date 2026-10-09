using David_Ahmetovic_projekat.Models;
using David_Ahmetovic_projekat.Services;

namespace David_Ahmetovic_projekat;

public partial class SchedulePage : ContentPage
{
    private readonly DatabaseService _databaseService;
    private string _selectedDay = "Ponedeljak";

    public SchedulePage()
    {
        InitializeComponent();
        _databaseService = new DatabaseService();
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        await LoadSchedule();
    }

    private async Task LoadSchedule()
    {
        var items = await _databaseService.GetScheduleByDayAsync(_selectedDay);
        ScheduleCollectionView.ItemsSource = items;
    }

    private async void OnDaySelected(object sender, EventArgs e)
    {
        if (sender is Button button)
        {
            _selectedDay = button.Text;

            // Vizuelno označavanje izabranog dana
            foreach (var child in DaysStackLayout.Children)
            {
                if (child is Button btn)
                {
                    if (btn.Text == _selectedDay)
                    {
                        btn.BackgroundColor = Color.FromArgb("#512BD4");
                        btn.TextColor = Colors.White;
                    }
                    else
                    {
                        btn.BackgroundColor = Color.FromArgb("#E0E0E0");
                        btn.TextColor = Colors.Black;
                    }
                }
            }

            await LoadSchedule();
        }
    }

    private async void OnAddScheduleItemClicked(object sender, EventArgs e)
    {
        var subjects = await _databaseService.GetSubjectsAsync();
        if (subjects.Count == 0)
        {
            await DisplayAlert("Upozorenje", "Prvo morate dodati barem jedan predmet u tabu Predmeti.", "OK");
            return;
        }

        string[] subjectNames = subjects.Select(s => s.Name).ToArray();
        string selectedSubject = await DisplayActionSheet("Izaberite predmet:", "Odustani", null, subjectNames);
        if (selectedSubject == "Odustani" || string.IsNullOrEmpty(selectedSubject)) return;

        var subjectObj = subjects.First(s => s.Name == selectedSubject);

        string startTime = await DisplayPromptAsync("Vreme početka", "Unesite vreme (npr. 08:00):", initialValue: "08:00");
        if (string.IsNullOrWhiteSpace(startTime)) return;

        string endTime = await DisplayPromptAsync("Vreme završetka", "Unesite vreme (npr. 09:30):", initialValue: "09:30");
        string classroom = await DisplayPromptAsync("Učionica", "Unesite kabinet/učionicu:", initialValue: subjectObj.Classroom);

        var newItem = new ScheduleItem
        {
            SubjectId = subjectObj.Id,
            SubjectName = subjectObj.Name,
            DayOfWeek = _selectedDay,
            StartTime = $"{startTime} - {endTime}",
            Classroom = classroom
        };

        await _databaseService.SaveScheduleItemAsync(newItem);
        await LoadSchedule();
    }

    private async void OnDeleteScheduleItemClicked(object sender, EventArgs e)
    {
        var button = sender as Button;
        var item = button?.CommandParameter as ScheduleItem;

        if (item != null)
        {
            bool confirm = await DisplayAlert("Potvrda", $"Želite li obrisati čas '{item.SubjectName}' za {_selectedDay}?", "Da", "Ne");
            if (confirm)
            {
                await _databaseService.DeleteScheduleItemAsync(item);
                await LoadSchedule();
            }
        }
    }

    private async void OnExportPdfClicked(object sender, EventArgs e)
    {
        var allItems = await _databaseService.GetScheduleAsync();
        if (allItems == null || allItems.Count == 0)
        {
            await DisplayAlert("Upozorenje", "Nema časova u rasporedu za izvoz.", "OK");
            return;
        }

        var user = await _databaseService.GetUserAsync();
        string name = user != null ? user.FullName : "Korisnik";

        await PdfService.GenerateAndShareSchedulePdfAsync(allItems, name);
    }
}