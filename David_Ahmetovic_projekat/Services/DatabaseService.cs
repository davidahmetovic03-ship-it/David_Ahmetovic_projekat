using SQLite;
using David_Ahmetovic_projekat.Models;

namespace David_Ahmetovic_projekat.Services
{
    public class DatabaseService
    {
        private SQLiteAsyncConnection _database;

        private async Task Init()
        {
            if (_database != null)
                return;

            // Putanja do lokacije na telefonu gdje se čuva baza
            var dbPath = Path.Combine(FileSystem.AppDataDirectory, "SchoolActivities.db3");
            _database = new SQLiteAsyncConnection(dbPath);

            // Kreiranje tabela u bazi ako već ne postoje
            await _database.CreateTableAsync<User>();
            await _database.CreateTableAsync<Subject>();
            await _database.CreateTableAsync<Grade>();
            await _database.CreateTableAsync<Activity>();
            await _database.CreateTableAsync<Absence>();
            await _database.CreateTableAsync<ScheduleItem>();
            await _database.CreateTableAsync<CalendarEvent>();
        }

        // --- USER METODE ---
        public async Task<User> GetUserAsync()
        {
            await Init();
            return await _database.Table<User>().FirstOrDefaultAsync();
        }

        public async Task<User> GetUserByUsernameAsync(string username)
        {
            await Init();
            return await _database.Table<User>()
                                 .Where(u => u.Username == username)
                                 .FirstOrDefaultAsync();
        }

        public async Task<User> GetUserByIdAsync(int id)
        {
            await Init();
            return await _database.Table<User>()
                                 .Where(u => u.Id == id)
                                 .FirstOrDefaultAsync();
        }

        public async Task<int> SaveUserAsync(User user)
        {
            await Init();

            if (user.Id != 0)
                return await _database.UpdateAsync(user);
            else
                return await _database.InsertAsync(user);
        }

        // --- SUBJECT METODE ---
        public async Task<List<Subject>> GetSubjectsAsync()
        {
            await Init();
            return await _database.Table<Subject>().ToListAsync();
        }

        // Dobavljanje predmeta samo za određenog korisnika
        public async Task<List<Subject>> GetUserSubjectsAsync(int userId)
        {
            await Init();

            return await _database.Table<Subject>()
                                  .Where(s => s.UserId == userId)
                                  .ToListAsync();
        }

        public async Task<int> SaveSubjectAsync(Subject subject)
        {
            await Init();

            if (subject.Id != 0)
                return await _database.UpdateAsync(subject);
            else
                return await _database.InsertAsync(subject);
        }

        public async Task<int> DeleteSubjectAsync(Subject subject)
        {
            await Init();
            return await _database.DeleteAsync(subject);
        }

        // --- GRADE METODE ---
        public async Task<List<Grade>> GetGradesForSubjectAsync(int subjectId)
        {
            await Init();

            return await _database.Table<Grade>()
                                  .Where(g => g.SubjectId == subjectId)
                                  .ToListAsync();
        }

        public async Task<int> SaveGradeAsync(Grade grade)
        {
            await Init();
            return await _database.InsertAsync(grade);
        }

        // --- ACTIVITY METODE ---
        public async Task<List<Activity>> GetActivitiesAsync()
        {
            await Init();
            return await _database.Table<Activity>().ToListAsync();
        }

        public async Task<int> SaveActivityAsync(Activity activity)
        {
            await Init();

            if (activity.Id != 0)
                return await _database.UpdateAsync(activity);
            else
                return await _database.InsertAsync(activity);
        }

        // --- ABSENCE METODE ---
        public async Task<List<Absence>> GetAbsencesAsync()
        {
            await Init();
            return await _database.Table<Absence>().ToListAsync();
        }

        public async Task<int> SaveAbsenceAsync(Absence absence)
        {
            await Init();
            return await _database.InsertAsync(absence);
        }

        public async Task<int> DeleteAbsenceAsync(Absence absence)
        {
            await Init();
            return await _database.DeleteAsync(absence);
        }

        // --- SCHEDULE METODE ---
        public async Task<List<ScheduleItem>> GetScheduleAsync()
        {
            await Init();
            return await _database.Table<ScheduleItem>().ToListAsync();
        }

        public async Task<List<ScheduleItem>> GetScheduleByDayAsync(string day)
        {
            await Init();

            return await _database.Table<ScheduleItem>()
                                  .Where(s => s.DayOfWeek == day)
                                  .ToListAsync();
        }

        public async Task<int> SaveScheduleItemAsync(ScheduleItem item)
        {
            await Init();

            if (item.Id != 0)
                return await _database.UpdateAsync(item);
            else
                return await _database.InsertAsync(item);
        }

        public async Task<int> DeleteScheduleItemAsync(ScheduleItem item)
        {
            await Init();
            return await _database.DeleteAsync(item);
        }

        // --- CALENDAR EVENT METODE ---
        public async Task<List<CalendarEvent>> GetCalendarEventsAsync()
        {
            await Init();

            return await _database.Table<CalendarEvent>()
                                  .OrderBy(e => e.Date)
                                  .ToListAsync();
        }

        public async Task<List<CalendarEvent>> GetCalendarEventsByDateAsync(DateTime date)
        {
            await Init();

            DateTime startOfDay = date.Date;
            DateTime endOfDay = date.Date.AddDays(1).AddTicks(-1);

            return await _database.Table<CalendarEvent>()
                                  .Where(e => e.Date >= startOfDay && e.Date <= endOfDay)
                                  .ToListAsync();
        }

        public async Task<int> SaveCalendarEventAsync(CalendarEvent calendarEvent)
        {
            await Init();

            if (calendarEvent.Id != 0)
                return await _database.UpdateAsync(calendarEvent);
            else
                return await _database.InsertAsync(calendarEvent);
        }

        public async Task<int> DeleteCalendarEventAsync(CalendarEvent calendarEvent)
        {
            await Init();
            return await _database.DeleteAsync(calendarEvent);
        }
    }
}