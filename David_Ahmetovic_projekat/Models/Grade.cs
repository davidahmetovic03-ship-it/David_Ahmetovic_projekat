using SQLite;

namespace David_Ahmetovic_projekat.Models
{
    public class Grade
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }

        public int SubjectId { get; set; } // ID predmeta na koji se odnosi ocena

        public int Value { get; set; } // Vrednost ocene (1-5)

        public string Description { get; set; } // Opis (npr. "Kontrolni iz algebre")

        public DateTime Date { get; set; } = DateTime.Now;
        public int UserId { get; set; }
    }
}
