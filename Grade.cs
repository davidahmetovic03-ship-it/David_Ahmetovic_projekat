using SQLite;

namespace David_Ahmetovic_projekat.Models
{
    public class Grade
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }
        public int SubjectId { get; set; }        
        public int Value { get; set; }            
        public string Description { get; set; }  
        public DateTime Date { get; set; }
    }
}