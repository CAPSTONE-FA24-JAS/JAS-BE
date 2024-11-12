using Application.Commons;
using Domain.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Application.Repositories
{
    public interface IGenericRepository<TEntity> where TEntity : BaseEntity
    {
        Task<List<TEntity>> GetAllAsync(Expression<Func<TEntity, bool>>? condition = null,
                                        Expression<Func<TEntity, object>>? sort = null,
                                        bool ascending = true,
                                        params Expression<Func<TEntity, object>>[] includes);
        Task<TEntity?> GetByIdAsync(int? id, 
                                    Expression<Func<TEntity,bool>>? condition = null,
                                    Expression<Func<TEntity, object>>? sort = null,
                                    bool ascending = true,
                                    params Expression<Func<TEntity, object>>[] includes);
        Task AddAsync(TEntity entity);
        void Update(TEntity entity);
        void UpdateRange(List<TEntity> entities);
        void SoftRemove(TEntity entity);
        void Remove(TEntity entity);
        void RemoveRange(List<TEntity> entities);
        Task AddRangeAsync(List<TEntity> entities);
        void SoftRemoveRange(List<TEntity> entities);
        void SetPropertyModified(TEntity entity, string propertyName);
        Task<Pagination<TEntity>> ToPagination(int pageNumber = 0, int pageSize = 10);

        Task<(IEnumerable<TEntity> data, int totalItems)> GetAllPaging(
        Expression<Func<TEntity, bool>>? filter = null,
        Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>>? orderBy = null,
        string includeProperties = "",
        int? pageIndex = null,
        int? pageSize = null);

    }
}
