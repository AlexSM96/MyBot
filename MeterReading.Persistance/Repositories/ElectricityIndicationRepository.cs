using MeterReading.Domain;
using MeterReading.Persistance.Interfaces;
using MeterReading.Persistance.Repositories.Base;
using Microsoft.EntityFrameworkCore;

namespace MeterReading.Persistance.Repositories
{
    public class ElectricityIndicationRepository(IMainDbContext context) : IBaseRepository<ElectricityIndication>
    {
        private readonly IMainDbContext _context = context;

        public async Task<IEnumerable<ElectricityIndication>> Get()
            => await _context.ElectricityIndications
            .AsNoTracking()
            .ToListAsync();

        public async Task<int> CreateAsync(ElectricityIndication? entity)
        {
            if (entity is null) return 0;

            await _context.ElectricityIndications.AddAsync(entity);
            return await _context.SaveChangesAsync(CancellationToken.None);
        }

        public async Task<int> UpdateAsync(ElectricityIndication? entity)
        {
            if (entity is null) return 0;

            await _context.ElectricityIndications
                .Where(e => e.Id == entity.Id)
                .ExecuteUpdateAsync(x => x
                    .SetProperty(e => e.AmountElectricity, entity.AmountElectricity)
                    .SetProperty(e => e.CreationDate, entity.CreationDate)
                    .SetProperty(e => e.CreationUser, entity.CreationUser));

            return await _context.SaveChangesAsync(CancellationToken.None);
        }

        public async Task<int> DeleteAsync(Guid? id)
        {
            if (id is null) return 0;

            await _context.ElectricityIndications
                .Where(e => e.Id == id)
                .ExecuteDeleteAsync();

            return await _context.SaveChangesAsync(CancellationToken.None);
        }
    }
}
