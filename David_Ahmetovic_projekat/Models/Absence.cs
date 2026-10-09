using SQLite;

namespace David_Ahmetovic_projekat.Models
{
    public class Absence
    {
        [SQLite.PrimaryKey, SQLite.AutoIncrement]
        public int Id { get; set; }

        public int UserId { get; set; }

        public string SubjectName { get; set; } = string.Empty;

        public DateTime Date { get; set; }

        public string Reason { get; set; } = string.Empty;

        public string Status { get; set; } = string.Empty; // Npr. "Opravdano", "Neopravdano", "Na čekanju"
    }
}
