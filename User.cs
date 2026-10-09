
using SQLite;

namespace David_Ahmetovic_projekat.Models
{
    public class User
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public string FullName { get; set; }
        public string SchoolName { get; set; }
    }
}
