using Azure.Core;
using Microsoft.EntityFrameworkCore;
using AirportBudget.Server.Datas;
using AirportBudget.Server.Interfaces.Repositorys;
using System.Linq.Expressions;

namespace AirportBudget.Server.Repositorys
{
    public class GenericRepository<T>(AirportBudgetDbContext context) : IGenericRepository<T> where T : class
    {
        private readonly AirportBudgetDbContext _context = context;

        public T? GetById(int id)
        {
            return _context.Set<T>().Find(id);
        }

        public async Task<T?> GetByIdAsync(int id)
        {
            return await _context.Set<T>().FindAsync(id);
        }

        public IQueryable<T> GetByCondition(Expression<Func<T, bool>> condition)
        {
            return _context.Set<T>().Where(condition);
        }

        public async Task<IQueryable<T>> GetByConditionAsync(Expression<Func<T, bool>> condition)
        {
            return await Task.Run(() => _context.Set<T>().Where(condition));
        }

        public IQueryable<T> GetAll()
        {
            return _context.Set<T>();
        }

        public async Task<IQueryable<T>> GetAllAsync()
        {
            return await Task.Run(_context.Set<T>);
        }

        public void Add(T entity)
        {
            _context.Set<T>().Add(entity);
            _context.SaveChanges();
        }

        public async Task AddAsync(T entity)
        {
            await _context.Set<T>().AddAsync(entity);
            await _context.SaveChangesAsync();
        }

        public void AddRange(IEnumerable<T> entities)
        {
            _context.Set<T>().AddRange(entities);
            _context.SaveChanges();
        }
        
        public async Task AddRangeAsync(IEnumerable<T> entities)
        {
            await _context.Set<T>().AddRangeAsync(entities);
            await _context.SaveChangesAsync();
        }

        public void Update(T entityToUpdate, T? updatedEntity = null)
        {
            if (updatedEntity != null)
            {
                _context.Entry(entityToUpdate).CurrentValues.SetValues(updatedEntity);
            }
            _context.Entry(entityToUpdate).State = EntityState.Modified;
            _context.SaveChanges();
        }

        public async Task UpdateAsync(T entityToUpdate, T? updatedEntity = null)
        {
            if (updatedEntity != null)
            {
                _context.Entry(entityToUpdate).CurrentValues.SetValues(updatedEntity);
            }
            _context.Entry(entityToUpdate).State = EntityState.Modified;
            await _context.SaveChangesAsync();
        }

        public void Delete(int id)
        {
            var entity = _context.Set<T>().Find(id);
            if (entity != null)
            {
                _context.Set<T>().Remove(entity);
                _context.SaveChanges();
            }
        }

        public async Task DeleteAsync(int id)
        {
            var entity = await _context.Set<T>().FindAsync(id);
            if (entity != null)
            {
                _context.Set<T>().Remove(entity);
                await _context.SaveChangesAsync();
            }
        }

        public void DeleteAll()
        {
            var entities = _context.Set<T>().ToList();
            if (entities != null && entities.Count != 0)
            {
                _context.Set<T>().RemoveRange(entities);
                _context.SaveChanges();
            }
        }

        public async Task DeleteAllAsync()
        {
            var entities = await _context.Set<T>().ToListAsync();
            if (entities != null && entities.Count != 0)
            {
                _context.Set<T>().RemoveRange(entities);
                await _context.SaveChangesAsync();
            }
        }
    }
}
