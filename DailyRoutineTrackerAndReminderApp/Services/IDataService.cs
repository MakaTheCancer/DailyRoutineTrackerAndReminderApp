namespace DailyRoutineTrackerAndReminderApp.Services
{
    public interface IDataService<T>
    {
        public Task<IEnumerable<T>> GetAll();
        public Task<T> GetById(int id);
        public Task<T> Create(T entity);
        public Task<T> Update(int id, T entity);
        public Task<bool> Delete(int id);
    }
}