using David_Ahmetovic_projekat.Models;
using David_Ahmetovic_projekat.Services;

namespace David_Ahmetovic_projekat;

public partial class SubjectsPage : ContentPage
{
    private readonly DatabaseService _databaseService;

    public SubjectsPage()
    {
        InitializeComponent();
        _databaseService = new DatabaseService();
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        await LoadSubjects();
    }

    private async Task LoadSubjects()
    {
        // Uzimamo ID ulogovanog korisnika
        int currentUserId = Preferences.Get("CurrentUserId", 0);

        // Učitavamo samo predmete trenutno prijavljenog korisnika
        var userSubjects = await _databaseService.GetUserSubjectsAsync(currentUserId);

        SubjectsCollectionView.ItemsSource = userSubjects;
    }

    private async void OnAddSubjectClicked(object sender, EventArgs e)
    {
        string name = await DisplayPromptAsync(
            "Novi Predmet",
            "Unesite naziv predmeta (npr. Matematika):");

        if (string.IsNullOrWhiteSpace(name))
            return;

        string teacher = await DisplayPromptAsync(
            "Profesor",
            "Unesite ime i prezime profesora:");

        string classroom = await DisplayPromptAsync(
            "Učionica",
            "Unesite kabinet/učionicu (npr. Kabinet 102):");

        string openDoors = await DisplayPromptAsync(
            "Otvorena Vrata",
            "Unesite termin otvorenih vrata (npr. Sreda 12h):");

        // Uzimamo ID ulogovanog korisnika
        int currentUserId = Preferences.Get("CurrentUserId", 0);

        var newSubject = new Subject
        {
            UserId = currentUserId,
            Name = name.Trim(),
            TeacherName = teacher?.Trim() ?? "",
            Classroom = classroom?.Trim() ?? "",
            OpenDoors = openDoors?.Trim() ?? ""
        };

        await _databaseService.SaveSubjectAsync(newSubject);
        await LoadSubjects();
    }

    private async void OnEditSubjectClicked(object sender, EventArgs e)
    {
        var button = sender as Button;
        var subject = button?.CommandParameter as Subject;

        if (subject == null)
            return;

        string newName = await DisplayPromptAsync(
            "Uredi predmet",
            "Novi naziv predmeta:",
            initialValue: subject.Name);

        if (string.IsNullOrWhiteSpace(newName))
            return;

        string newTeacher = await DisplayPromptAsync(
            "Uredi predmet",
            "Novi profesor:",
            initialValue: subject.TeacherName);

        string newClassroom = await DisplayPromptAsync(
            "Uredi predmet",
            "Nova učionica/kabinet:",
            initialValue: subject.Classroom);

        string newOpenDoors = await DisplayPromptAsync(
            "Uredi predmet",
            "Novi termin otvorenih vrata:",
            initialValue: subject.OpenDoors);

        subject.Name = newName.Trim();
        subject.TeacherName = newTeacher?.Trim() ?? "";
        subject.Classroom = newClassroom?.Trim() ?? "";
        subject.OpenDoors = newOpenDoors?.Trim() ?? "";

        await _databaseService.SaveSubjectAsync(subject);
        await LoadSubjects();
    }

    private async void OnDeleteSubjectClicked(object sender, EventArgs e)
    {
        var button = sender as Button;
        var subject = button?.CommandParameter as Subject;

        if (subject != null)
        {
            bool confirm = await DisplayAlert(
                "Potvrda",
                $"Da li ste sigurni da želite obrisati predmet '{subject.Name}'?",
                "Da",
                "Ne");

            if (confirm)
            {
                await _databaseService.DeleteSubjectAsync(subject);
                await LoadSubjects();
            }
        }
    }

    private async void OnOpenGradesClicked(object sender, EventArgs e)
    {
        await Navigation.PushAsync(new AddGradePage());
    }
}