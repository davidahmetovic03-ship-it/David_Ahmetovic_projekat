using David_Ahmetovic_projekat.Models;
using David_Ahmetovic_projekat.Services;
using David_Ahmetovic_projekat;

namespace David_Ahmetovic_projekat
{
    public partial class App : Application
    {
        public App()
        {
            InitializeComponent();

            // Provjeravamo da li postoji sačuvan ID korisnika
            int currentUserId = Preferences.Get("CurrentUserId", 0);

            if (currentUserId > 0)
            {
                // Korisnik je već ulogovan
                MainPage = new AppShell();
            }
            else
            {
                // Korisnik nije ulogovan, ide na Login stranicu
                MainPage = new NavigationPage(new LoginPage());
            }
        }
    }
}