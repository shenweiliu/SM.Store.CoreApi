using System.Collections.Generic;
using System;
using System.Linq;
using SM.Store.Api.Contracts.Interfaces;
using Entities = SM.Store.Api.Contracts.Entities;
using Models = SM.Store.Api.Contracts.Models;
using SM.Store.Api.DAL;

namespace SM.Store.Api.BLL
{
    public class LookupBS : ILookupBS
    {
        //Instantiate from the IStoreLookupRepository 
        private IStoreLookupRepository<Entities.Category> _categoryRepository;
        private IStoreLookupRepository<Entities.ProductStatusType> _productStatusTypeRepository;
        
        public LookupBS(IStoreLookupRepository<Entities.Category> cateoryRepository,
                        IStoreLookupRepository<Entities.ProductStatusType> productStatusTypeRepository)
        {
            this._categoryRepository = cateoryRepository;
            this._productStatusTypeRepository = productStatusTypeRepository;            
        }

        public IList<Models.Category> LookupCategories()
        {
            var query = this._categoryRepository.GetIQueryable();
            var list = query.Select(a => new Models.Category
            {
                CategoryId = a.CategoryId,
                CategoryName = a.CategoryName
            });

            return list.ToList();
        }

        public IList<Models.ProductStatusType> LookupProductStatusTypes()
        {
            var query = this._productStatusTypeRepository.GetIQueryable();
            var list = query.Select(a => new Models.ProductStatusType
            {
                StatusCode = a.StatusCode,
                Description = a.Description
            });

            return list.ToList();
        }        
    }
}

