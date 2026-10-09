using David_Ahmetovic_projekat.Services;

namespace David_Ahmetovic_projekat;

public partial class ProfilePage : ContentPage
{
    private readonly DatabaseService _databaseService;

    public ProfilePage()
    {
        InitializeComponent();
        _databaseService = new DatabaseService();
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        await LoadProfileData();
    }

    private async Task LoadProfileData()
    {
        try
        {
            var user = await _databaseService.GetUserAsync();

            if (user != null)
            {
                FullNameLabel.Text = string.IsNullOrWhiteSpace(user.FullName)
                    ? "Nije uneto"
                    : user.FullName;

                UsernameLabel.Text = string.IsNullOrWhiteSpace(user.Username)
                    ? "Nije uneto"
                    : user.Username;

                BirthDateLabel.Text = user.BirthDate != default
                    ? user.BirthDate.ToString("dd.MM.yyyy")
                    : "Nije uneto";

                SchoolNameLabel.Text = string.IsNullOrWhiteSpace(user.SchoolName)
                    ? "Nije uneto"
                    : user.SchoolName;

                SchoolYearLabel.Text = string.IsNullOrWhiteSpace(user.SchoolYear)
                    ? "Nije uneto"
                    : user.SchoolYear;

                GradeLevelLabel.Text = string.IsNullOrWhiteSpace(user.GradeLevel)
                    ? "Nije uneto"
                    : user.GradeLevel;

                FieldOfStudyLabel.Text = string.IsNullOrWhiteSpace(user.FieldOfStudy)
                    ? "Nije uneto"
                    : user.FieldOfStudy;
            }
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Greška pri učitavanju profila: {ex.Message}");
        }
    }

    private async void OnEditProfileClicked(object sender, EventArgs e)
    {
        try
        {
            var user = await _databaseService.GetUserAsync();
            if (user == null)
            {
                await DisplayAlert("Greška", "Nijedan korisnik nije ulogovan.", "OK");
                return;
            }

            // 1. Unos Imena i Prezimena
            string fullName = await DisplayPromptAsync("Izmena profila", "Unesite Ime i Prezime:", initialValue: user.FullName);
            if (fullName == null) return;

            // 2. Unos Naziva škole
            string schoolName = await DisplayPromptAsync("Izmena profila", "Unesite naziv škole:", initialValue: user.SchoolName);
            if (schoolName == null) return;

            // 3. Unos Školske godine
            string schoolYear = await DisplayPromptAsync("Izmena profila", "Unesite školsku godinu (npr. 2023/2024):", initialValue: user.SchoolYear);
            if (schoolYear == null) return;

            // 4. Unos Razreda/Odeljenja
            string gradeLevel = await DisplayPromptAsync("Izmena profila", "Unesite razred/odeljenje (npr. III-2):", initialValue: user.GradeLevel);
            if (gradeLevel == null) return;

            // 5. Unos Smera / Profila
            string fieldOfStudy = await DisplayPromptAsync("Izmena profila", "Unesite smer / profil (npr. Informacioni tehnolog):", initialValue: user.FieldOfStudy);
            if (fieldOfStudy == null) return;

            // 6. Unos Datuma rođenja
            string birthDateStr = await DisplayPromptAsync("Izmena profila", "Unesite datum rođenja (format: DD.MM.GGGG):",
                initialValue: user.BirthDate != default ? user.BirthDate.ToString("dd.MM.yyyy") : "");
            if (birthDateStr == null) return;

            // Ažuriranje tekstualnih polja
            user.FullName = fullName;
            user.SchoolName = schoolName;
            user.SchoolYear = schoolYear;
            user.GradeLevel = gradeLevel;
            user.FieldOfStudy = fieldOfStudy;

            // Konverzija unetog datuma u DateTime
            if (DateTime.TryParse(birthDateStr, out DateTime parsedDate))
            {
                user.BirthDate = parsedDate;
            }

            // Sačuvaj izmene u bazi
            await _databaseService.SaveUserAsync(user);

            // Osveži prikaz na stranici
            await LoadProfileData();

            await DisplayAlert("Uspeh", "Profil je uspešno izmenjen!", "OK");
        }
        catch (Exception ex)
        {
            await DisplayAlert("Greška", $"Došlo je do greške: {ex.Message}", "OK");
        }
    }

    private async void OnLogoutClicked(object sender, EventArgs e)
    {
        bool confirm = await DisplayAlert(
            "Odjava",
            "Da li ste sigurni da želite da se odjavite?",
            "Da",
            "Ne");

        if (confirm)
        {
            Preferences.Remove("CurrentUserId");
            Preferences.Remove("CurrentUsername");

            if (Application.Current != null)
            {
                Application.Current.MainPage = new NavigationPage(new LoginPage());
            }
        }
    }
}