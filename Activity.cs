using SQLite;

namespace David_Ahmetovic_projekat.Models
{
    public class Activity
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }
        public int SubjectId { get; set; }        
        public string Title { get; set; }        
        public string Type { get; set; }         
        public DateTime DueDate { get; set; }     
        public string Status { get; set; }       
    }
}