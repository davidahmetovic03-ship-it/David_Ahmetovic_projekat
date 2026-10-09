using SQLite;

namespace David_Ahmetovic_projekat.Models
{
    public class User
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }

        [Unique]
        public string Username { get; set; } = string.Empty;

        public string Password { get; set; } = string.Empty; // Dodato za login/registraciju

        public string FullName { get; set; } = string.Empty;
        public string SchoolName { get; set; } = string.Empty;

        // Nova polja
        public DateTime BirthDate { get; set; } = DateTime.Now.AddYears(-15);
        public string SchoolYear { get; set; } = string.Empty; // Npr. "2024/2025"
        public string GradeLevel { get; set; } = string.Empty; // Npr. "III-2" ili "3. razred"
        public string FieldOfStudy { get; set; } = string.Empty; // Npr. "Elektrotehničar računara"
    }
}