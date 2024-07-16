using System.Linq.Expressions;

namespace AirportBudget.Server.Interfaces.Repositorys
{
    public interface IGenericRepository<T> where T : class
    {
        T? GetById(int id);
        IQueryable<T> GetByCondition(Expression<Func<T, bool>> condition);
        IQueryable<T> GetAll();
        void Add(T entity);
        void AddRange(IEnumerable<T> entities);
        void Update(T entityToUpdate, T? updatedEntity = null);
        void Delete(int id);
        void DeleteAll();

        Task<T?> GetByIdAsync(int id);
        Task<IQueryable<T>> GetByConditionAsync(Expression<Func<T, bool>> condition);
        Task<IQueryable<T>> GetAllAsync();
        Task AddAsync(T entity);
        Task AddRangeAsync(IEnumerable<T> entities);
        Task UpdateAsync(T entityToUpdate, T? updatedEntity = null);
        Task DeleteAsync(int id);
        Task DeleteAllAsync();
    }
}
