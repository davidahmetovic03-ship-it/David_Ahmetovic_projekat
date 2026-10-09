using SQLite;

namespace David_Ahmetovic_projekat.Models
{
    public class Absence
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }
        public int SubjectId { get; set; }        
        public DateTime Date { get; set; }
        public string Reason { get; set; }       
        public bool IsJustified { get; set; }    
    }
}