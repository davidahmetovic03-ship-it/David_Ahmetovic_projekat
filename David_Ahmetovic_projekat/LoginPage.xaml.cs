using David_Ahmetovic_projekat.Models;
using David_Ahmetovic_projekat.Services;

namespace David_Ahmetovic_projekat;

public partial class LoginPage : ContentPage
{
    private readonly DatabaseService _databaseService;
    private bool _isRegisterMode = false;

    public LoginPage()
    {
        InitializeComponent();
        _databaseService = new DatabaseService();
    }

    private async void OnSubmitClicked(object sender, EventArgs e)
    {
        string username = UsernameEntry.Text?.Trim() ?? string.Empty;
        string password = PasswordEntry.Text?.Trim() ?? string.Empty;

        if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(password))
        {
            await DisplayAlert("Greška", "Molimo unesite korisničko ime i lozinku.", "OK");
            return;
        }

        if (_isRegisterMode)
        {
            // === REGISTRACIJA (Novi Nalog) ===
            string confirmPassword = ConfirmPasswordEntry.Text?.Trim() ?? string.Empty;

            if (password != confirmPassword)
            {
                await DisplayAlert("Greška", "Lozinke se ne poklapaju!", "OK");
                return;
            }

            var existingUser = await _databaseService.GetUserByUsernameAsync(username);
            if (existingUser != null)
            {
                await DisplayAlert("Greška", "To korisničko ime je već zauzeto.", "OK");
                return;
            }

            var newUser = new User
            {
                Username = username,
                Password = password
            };

            await _databaseService.SaveUserAsync(newUser);

            // Sačuvaj sesiju u Preferences
            Preferences.Set("CurrentUserId", newUser.Id);
            Preferences.Set("CurrentUsername", newUser.Username);

            await DisplayAlert("Uspeh", "Nalog je uspešno kreiran!", "OK");

            // Preusmeravanje na glavnu stranicu
            NavigateToMainPage();
        }
        else
        {
            // === PRIJAVA (Postojeći Nalog) ===
            var user = await _databaseService.GetUserByUsernameAsync(username);

            if (user == null || user.Password != password)
            {
                await DisplayAlert("Neuspešna prijava", "Netačno korisničko ime ili lozinka.", "OK");
                return;
            }

            // Uspešna prijava - sačuvaj sesiju
            Preferences.Set("CurrentUserId", user.Id);
            Preferences.Set("CurrentUsername", user.Username);

            // Preusmeravanje na glavnu stranicu
            NavigateToMainPage();
        }
    }

    private void NavigateToMainPage()
    {
        // Kompatibilno prebacivanje stranice za .NET 9
        if (Application.Current?.Windows.Count > 0)
        {
            Application.Current.Windows[0].Page = new AppShell();
        }
        else if (Application.Current != null)
        {
            Application.Current.MainPage = new AppShell();
        }
    }

    private void OnToggleModeClicked(object sender, EventArgs e)
    {
        _isRegisterMode = !_isRegisterMode;

        if (_isRegisterMode)
        {
            TitleLabel.Text = "Registracija";
            SubmitButton.Text = "Registruj se";
            ConfirmPasswordContainer.IsVisible = true;
            ToggleQuestionLabel.Text = "Već imate nalog?";
            ToggleButton.Text = "Prijavite se";
        }
        else
        {
            TitleLabel.Text = "Prijava";
            SubmitButton.Text = "Prijavi se";
            ConfirmPasswordContainer.IsVisible = false;
            ToggleQuestionLabel.Text = "Nemate nalog?";
            ToggleButton.Text = "Napravite nalog";
        }
    }
}