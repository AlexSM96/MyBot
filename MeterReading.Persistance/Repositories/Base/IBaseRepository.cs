namespace MeterReading.Persistance.Repositories.Base
{
    public interface IBaseRepository<T>
    {
        public Task<IEnumerable<T>> Get();

        public Task<int> CreateAsync(T? entity);

        public Task<int> UpdateAsync(T? entity);

        public Task<int> DeleteAsync(Guid? id);
    }
}
