using DailyRoutineTrackerAndReminderApp.appdbcontext;
using DailyRoutineTrackerAndReminderApp.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace DailyRoutineTrackerAndReminderApp.Services
{
    public class GenericDataService<T> : IDataService<T> where T : User
    {
        private readonly DailyRoutineTrackerAndReminderAppDbContextFactory _dbContextFactory;

        public GenericDataService(DailyRoutineTrackerAndReminderAppDbContextFactory dbContextFactory)
        {
            _dbContextFactory = dbContextFactory;
        }

        public async Task<T> Create(T entity)
        {
            using (DailyRoutineTrackerAndReminderAppDbContext context = _dbContextFactory.CreateDbContext())
            {
                EntityEntry<T> createdResult = await context.Set<T>().AddAsync(entity);
                await context.SaveChangesAsync();

                return createdResult.Entity;
            }
        }

        public async Task<bool> Delete(int id)
        {
            using (DailyRoutineTrackerAndReminderAppDbContext context = _dbContextFactory.CreateDbContext())
            {
                T entity = await context.Set<T>().FirstOrDefaultAsync((e) => e.UserId == id);
                context.Set<T>().Remove(entity);
                await context.SaveChangesAsync();

                return true;
            }
        }

        public async Task<IEnumerable<T>> GetAll()
        {
            using (DailyRoutineTrackerAndReminderAppDbContext context = _dbContextFactory.CreateDbContext())
            {
                IEnumerable<T> entities = await context.Set<T>().ToListAsync();

                return entities;
            }
        }

        public async Task<T> GetById(int id)
        {
            using (DailyRoutineTrackerAndReminderAppDbContext context = _dbContextFactory.CreateDbContext())
            {
                T entity = await context.Set<T>().FirstOrDefaultAsync((e) => e.UserId == id);

                return entity;
            }
        }

        public async Task<T> Update(int id, T entity)
        {
            using (DailyRoutineTrackerAndReminderAppDbContext context = _dbContextFactory.CreateDbContext())
            {
                entity.UserId = id;
                context.Set<T>().Update(entity);
                await context.SaveChangesAsync();

                return entity;
            }
        }

        public async Task<int?> GetUserIdByEmail(string email)
        {
            using (DailyRoutineTrackerAndReminderAppDbContext context = _dbContextFactory.CreateDbContext())
            {
                var user = await context.Users.FirstOrDefaultAsync(u => u.Email == email);
                return user?.UserId;
            }
        }




    }
}
