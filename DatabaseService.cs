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
        }

        // --- USER METODE ---
        public async Task<User> GetUserAsync()
        {
            await Init();
            return await _database.Table<User>().FirstOrDefaultAsync();
        }

        public async Task SaveUserAsync(User user)
        {
            await Init();
            if (user.Id != 0)
                await _database.UpdateAsync(user);
            else
                await _database.InsertAsync(user);
        }

        // --- SUBJECT METODE ---
        public async Task<List<Subject>> GetSubjectsAsync()
        {
            await Init();
            return await _database.Table<Subject>().ToListAsync();
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
            return await _database.Table<Grade>().Where(g => g.SubjectId == subjectId).ToListAsync();
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
    }
}