
using Data.Contexts;
using Data.Entities;
using Data.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using System.Diagnostics;
using System.Linq.Expressions;

namespace Data.Repositories;

public abstract class BaseRepository<TEntity>(DataContext context) : IBaseRepository<TEntity> where TEntity : class
{
    private readonly DataContext _context = context;
    private readonly DbSet<TEntity> _dbSet = context.Set<TEntity>();
    private IDbContextTransaction _transaction = null!;

    #region Transaction Management

    public virtual async Task BeginTransactionAsync()
    {
        if (_transaction == null) // Prevent nested transactions
        {
            _transaction = await _context.Database.BeginTransactionAsync();
        }
    }

    public virtual async Task CommitTransactionAsync()
    {
        if (_transaction != null)
        {
            //await _context.SaveChangesAsync(); // Ensure changes are committed
            await _transaction.CommitAsync();
            await _transaction.DisposeAsync();
            _transaction = null!; // Reset transaction reference
        }
    }

    public virtual async Task RollbackTransactionAsync()
    {
        if (_transaction != null)
        {
            await _transaction.RollbackAsync();
            await _transaction.DisposeAsync();
            _transaction = null!; // Reset transaction reference
        }
    }

    #endregion



    #region Base CRUD Functionality
    public virtual async Task<TEntity> CreateAsync(TEntity entity)
    {
        if (entity == null)
        {
            return null!;
        }

        try
        {
            await _dbSet.AddAsync(entity);
            await _context.SaveChangesAsync();

            /* 
            Set ProjectNumber after SaveChanges (when Project_Id is assigned
            EITHER THIS IS TO BE DONE WITH A SQL SERVER TRIGGER OR HERE MANUALLY.

            Doing it here manually is not elegant, but it works. It will  work for all the situations I want it to work in and I can extend it to the cases 
            Where any sort of composites are being generated. It's being done automatically here, doing it in computed column DID NOT work. This drove me insane, but it works, the integration tests are working. Format is coming up correctly. 
            
            ProjectNumber has been set to be nullable so that the exception does not trigger (i.e. No Entity Created), ProjectNumber can be manually incremented or directly and uniquely mapped using Project_Id (which is the PK), the latter made more sense to me. This needs a save to it pulls it from the database then updates it. Normally this is not great, however Project Creation is not expected to be a task which is pinging the DB 1000 times per second and as such, this inefficiency (for the sake of convenience and remnants of my early morning sanity) shall be tolerated by me for now. It is automated and unique, as it should be. 
            */
            if (entity is ProjectEntity projectEntity)
            {
                projectEntity.ProjectNumber = $"P-{projectEntity.Project_Id}";
                _context.Update(entity);
            }


            return entity;
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"Error creating {nameof(TEntity)} entity ::{ex.Message}");
            return null!;
        }
    }

    public virtual async Task<IEnumerable<TEntity>> GetAllAsync(Expression<Func<TEntity, bool>>? expression = null)
    {
        return expression == null
            ? await _dbSet.ToListAsync()
            : await _dbSet.Where(expression).ToListAsync();
    }

    public virtual async Task<TEntity> GetAsync(Expression<Func<TEntity, bool>> expression)
    {
        if (expression == null)
        {
            return null!;
        }
        return await _dbSet.FirstOrDefaultAsync(expression) ?? null!;
    }

    public virtual async Task<TEntity> UpdateAsync(Expression<Func<TEntity, bool>> expression, TEntity updatedEntity)
    {
        if (updatedEntity == null)
        {
            return null!;
        }
        try
        {
            var existingEntity = await _dbSet.FirstOrDefaultAsync(expression) ?? null!;
            if (existingEntity == null)
            {
                return null!;
            }

            _context.Entry(existingEntity).CurrentValues.SetValues(updatedEntity);
            await _context.SaveChangesAsync();

            return existingEntity;
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"Error updating {nameof(TEntity)} entity :: {ex.Message}");
            return null!;
        }
    }

    public virtual async Task<bool> DeleteAsync(Expression<Func<TEntity, bool>> expression)
    {

        if (expression == null)
        {
            return false;
        }

        try
        {
            var existingEntity = await _dbSet.FirstOrDefaultAsync(expression);
            if (existingEntity == null)
                return false;

            _dbSet.Remove(existingEntity);
            await _context.SaveChangesAsync();
            return true;
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"Error deleting {nameof(TEntity)} entity :: {ex.Message}");
            return false;
        }
    }

    public virtual async Task<bool> ExistsAsync(Expression<Func<TEntity, bool>> expression)
    {
        return await _dbSet.AnyAsync(expression);
    }

    #endregion
}
