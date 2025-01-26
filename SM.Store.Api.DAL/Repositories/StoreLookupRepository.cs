using System;
using System.Linq;
using System.Collections.Generic;
using SM.Store.Api.Common;
using Entities = SM.Store.Api.Contracts.Entities;
using Models = SM.Store.Api.Contracts.Models;
using SM.Store.Api.Contracts.Interfaces;
using Microsoft.EntityFrameworkCore;


namespace SM.Store.Api.DAL
{
    public class StoreLookupRepository<TEntity> : GenericRepository<TEntity>, IStoreLookupRepository<TEntity> where TEntity : class
    {
        //Just need to pass db context to GenericRepository. 
        public StoreLookupRepository(StoreDataContext context)
            : base(context)
        {            
        }
        
    }
}
