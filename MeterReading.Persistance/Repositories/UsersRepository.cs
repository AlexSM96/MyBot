using MeterReading.Domain;
using MeterReading.Persistance.Interfaces;
using MeterReading.Persistance.Repositories.Base;
using Microsoft.EntityFrameworkCore;

namespace MeterReading.Persistance.Repositories
{
    public class UsersRepository(IMainDbContext context) : IBaseRepository<User>
    {
        private readonly IMainDbContext _context = context;
        public async Task<IEnumerable<User>> Get() => 
            await _context.Users
            .AsNoTracking()
            .ToListAsync();


        public async Task<int> CreateAsync(User? entity)
        {
            if (entity is null) return 0;
            var userFromDb = await _context.Users
                .FirstOrDefaultAsync(u => u.TelegaramUserId == entity.TelegaramUserId);
            if (userFromDb is not null) return 0;

            await _context.Users.AddAsync(entity);
            return await _context.SaveChangesAsync(CancellationToken.None);
        }

        public async Task<int> UpdateAsync(User? entity)
        {
            if (entity is null) return 0;

            await _context.Users
                .Where(u => u.Id == entity.Id)
                .ExecuteUpdateAsync(u => u
                    .SetProperty(u => u.FirstName, entity.FirstName)
                    .SetProperty(u => u.LastName, entity.LastName)
                    .SetProperty(u => u.UserName, entity.UserName)
                    .SetProperty(u => u.TelegaramUserId, entity.TelegaramUserId)
                    .SetProperty(u => u.ChatId, entity.ChatId));

            return await _context.SaveChangesAsync(CancellationToken.None);
        }

        public async Task<int> DeleteAsync(Guid? id)
        {
            if (id is null) return 0;

            await _context.Users
                .Where(x => x.Id == id)
                .ExecuteDeleteAsync();

            return await _context.SaveChangesAsync(CancellationToken.None);
        }
    }
}
