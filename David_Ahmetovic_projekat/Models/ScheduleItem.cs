using SQLite;

namespace David_Ahmetovic_projekat.Models
{
    public class ScheduleItem
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }

        public int SubjectId { get; set; }
        public string SubjectName { get; set; }
        public string DayOfWeek { get; set; } // Ponedjeljak, Utorak...
        public string StartTime { get; set; } // npr. "08:00"
        public string EndTime { get; set; }   // npr. "09:30"
        public string Classroom { get; set; }
    }
}
