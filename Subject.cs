using SQLite;

namespace David_Ahmetovic_projekat.Models
{
    public class Subject
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }
        public string Name { get; set; }          
        public string TeacherName { get; set; }   
        public string OpenDoors { get; set; }    
        public string Classroom { get; set; }     
    }
}