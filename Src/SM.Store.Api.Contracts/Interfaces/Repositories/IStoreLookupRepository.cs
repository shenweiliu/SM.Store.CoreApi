using System;
using System.Collections.Generic;

namespace SM.Store.Api.Contracts.Interfaces
{
    public interface IStoreLookupRepository<TEntity> : IGenericRepository<TEntity>  where TEntity : class
    {        
    }
}
