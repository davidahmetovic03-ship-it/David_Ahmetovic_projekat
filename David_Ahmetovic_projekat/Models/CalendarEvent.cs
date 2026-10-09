using SQLite;

namespace David_Ahmetovic_projekat.Models
{
    public class CalendarEvent
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }

        public string Title { get; set; }          // Naslov (npr. Kontrolni iz Matematike)
        public string Type { get; set; }           // Tip: "Kontrolni", "Pismeni", "Domaći", "Obavještenje"
        public DateTime Date { get; set; }         // Datum i vrijeme događaja
        public string Description { get; set; }    // Dodatni opis ili bilješke
        public string SubjectName { get; set; }    // Naziv predmeta
        public int UserId { get; set; }
    }
}