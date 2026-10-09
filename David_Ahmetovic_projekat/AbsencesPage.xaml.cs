using David_Ahmetovic_projekat.Models;
using David_Ahmetovic_projekat.Services;

namespace David_Ahmetovic_projekat;

public partial class AbsencesPage : ContentPage
{
    private readonly DatabaseService _databaseService;

    public AbsencesPage()
    {
        InitializeComponent();
        _databaseService = new DatabaseService();
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        await LoadAbsences();
    }

    private async Task LoadAbsences()
    {
        var absences = await _databaseService.GetAbsencesAsync() ?? new List<Absence>();
        AbsencesCollectionView.ItemsSource = absences;

        // Izračunavanje sume
        int excused = absences.Count(a => a.Status == "Opravdano");
        int unexcused = absences.Count(a => a.Status == "Neopravdano");
        int pending = absences.Count(a => a.Status == "Na čekanju");

        ExcusedCountLabel.Text = excused.ToString();
        UnexcusedCountLabel.Text = unexcused.ToString();
        PendingCountLabel.Text = pending.ToString();
    }

    private async void OnAddAbsenceClicked(object sender, EventArgs e)
    {
        // 1. Izbor predmeta
        var subjects = await _databaseService.GetSubjectsAsync();
        if (subjects == null || subjects.Count == 0)
        {
            await DisplayAlert("Upozorenje", "Prvo morate dodati barem jedan predmet u tabu Predmeti.", "OK");
            return;
        }

        string[] subjectNames = subjects.Select(s => s.Name).ToArray();
        string selectedSubject = await DisplayActionSheet("Izaberite predmet:", "Odustani", null, subjectNames);
        if (selectedSubject == "Odustani" || string.IsNullOrEmpty(selectedSubject)) return;

        // 2. Status izostanka
        string[] statuses = { "Opravdano", "Neopravdano", "Na čekanju" };
        string selectedStatus = await DisplayActionSheet("Status izostanka:", "Odustani", null, statuses);
        if (selectedStatus == "Odustani" || string.IsNullOrEmpty(selectedStatus)) return;

        // 3. Razlog
        string reason = await DisplayPromptAsync("Razlog", "Unesite razlog izostanka (opcionalno):", initialValue: "Bolest");

        var newAbsence = new Absence
        {
            SubjectName = selectedSubject,
            Date = DateTime.Now,
            Status = selectedStatus,
            Reason = string.IsNullOrWhiteSpace(reason) ? "Nije navedeno" : reason
        };

        await _databaseService.SaveAbsenceAsync(newAbsence);
        await LoadAbsences();
    }

    private async void OnDeleteAbsenceClicked(object sender, EventArgs e)
    {
        var button = sender as Button;
        var absence = button?.CommandParameter as Absence;

        if (absence != null)
        {
            bool confirm = await DisplayAlert("Potvrda", "Želite li obrisati ovaj izostanak?", "Da", "Ne");
            if (confirm)
            {
                await _databaseService.DeleteAbsenceAsync(absence);
                await LoadAbsences();
            }
        }
    }

    private async void OnEditAbsenceClicked(object sender, EventArgs e)
    {
        var button = sender as Button;
        var absence = button?.CommandParameter as Absence;

        if (absence == null) return;

        string newReason = await DisplayPromptAsync("Uredi izostanak", "Unesite novi razlog ili opis:", initialValue: absence.Reason);
        if (newReason == null) return; // Korisnik otkazao

        absence.Reason = string.IsNullOrWhiteSpace(newReason) ? "Nije navedeno" : newReason;
        await _databaseService.SaveAbsenceAsync(absence);
        await LoadAbsences();
    }
}