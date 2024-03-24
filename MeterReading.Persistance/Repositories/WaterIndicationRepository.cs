using MeterReading.Domain;
using MeterReading.Persistance.Interfaces;
using MeterReading.Persistance.Repositories.Base;
using Microsoft.EntityFrameworkCore;

namespace MeterReading.Persistance.Repositories
{
    public class WaterIndicationRepository(IMainDbContext context) : IBaseRepository<WaterIndication>
    {
        private readonly IMainDbContext _context = context;

        public async Task<IEnumerable<WaterIndication>> Get() => 
            await _context.WaterIndications
            .AsNoTracking()
            .ToListAsync();


        public async Task<int> CreateAsync(WaterIndication? entity)
        {
            if (entity is null) return 0;
            await _context.WaterIndications.AddAsync(entity);
            return await _context.SaveChangesAsync(CancellationToken.None);
        }

        public async Task<int> UpdateAsync(WaterIndication? entity)
        {
            if (entity is null) return 0;

            await _context.WaterIndications
                .Where(i => i.Id == entity.Id)
                .ExecuteUpdateAsync(x => x.SetProperty(i => i.AmountColdWater, entity.AmountColdWater)
                    .SetProperty(i => i.AmountHotWater, entity.AmountHotWater)
                    .SetProperty(i => i.CreationUser, entity.CreationUser)
                    .SetProperty(i => i.CreationDate, entity.CreationDate));

            return await _context.SaveChangesAsync(CancellationToken.None);
        }

        public async Task<int> DeleteAsync(Guid? id)
        {
            if (id is null) return 0;

            await _context.WaterIndications
                .Where(i => i.Id == id)
                .ExecuteDeleteAsync();

            return await _context.SaveChangesAsync(CancellationToken.None);
        }
    }
}
