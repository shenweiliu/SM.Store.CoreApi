using System;
using System.Linq;
using System.Collections.Generic;
using SM.Store.Api.DAL;
using SM.Store.Api.Common;
using Entities = SM.Store.Api.Contracts.Entities;
using Models = SM.Store.Api.Contracts.Models;
using SM.Store.Api.Contracts.Interfaces;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;

namespace SM.Store.Api.DAL
{
    public class ProductRepository : GenericRepository<Entities.Product>, IProductRepository
    {
        private StoreDataContext storeDBContext;
        public ProductRepository(StoreDataContext context)
            : base(context)
        {
            storeDBContext = context;
        }

        public async Task<IList<Entities.Product>> GetProducts()
        {
            return await this.GetAllAsync();
        }

        public async Task<Entities.Product> GetProductById(int id)
        {
            return await this.GetByIdAsync(id);
        }

        private IQueryable<Models.ProductCM> GetProductListQueryable()
        {
            //Core 3.0 Use from...select (outer join) instead of GroupJoin syntax.
            var query =
                from pr in storeDBContext.Products
                join ca in storeDBContext.Categories
                    on pr.CategoryId equals ca.CategoryId
                join ps in storeDBContext.ProductStatusTypes
                    on pr.StatusCode equals ps.StatusCode into tempJoin
                from t2 in tempJoin.DefaultIfEmpty()
                select new Models.ProductCM
                {
                    ProductId = pr.ProductId,
                    ProductName = pr.ProductName,
                    CategoryId = pr.CategoryId,
                    CategoryName = ca.CategoryName,
                    UnitPrice = pr.UnitPrice,
                    StatusCode = pr.StatusCode,
                    StatusDescription = t2.Description,
                    AvailableSince = pr.AvailableSince
                };

            //More readable code:
            //var query =
            //    from pr in storeDBContext.Products
            //    from ca in storeDBContext.Categories
            //       .Where(ca => ca.CategoryId == pr.CategoryId)
            //    from ps in storeDBContext.ProductStatusTypes
            //       .Where(ps => ps.StatusCode == pr.StatusCode).DefaultIfEmpty()
            //    select new Models.ProductCM
            //            {
            //                ProductId = pr.ProductId,
            //                ProductName = pr.ProductName,
            //                CategoryId = pr.CategoryId,
            //                CategoryName = ca.CategoryName,
            //                UnitPrice = pr.UnitPrice,
            //                StatusCode = pr.StatusCode,
            //                StatusDescription = ps.Description,
            //                AvailableSince = pr.AvailableSince
            //            };

            //Core 3.0: GroupJoin NOT work(Core 2.2 works)
            //var query = storeDBContext.Products
            //        .GroupJoin(storeDBContext.Categories,
            //        p => p.CategoryId, c => c.CategoryId,
            //        (p, c) => new { p, c })
            //        .GroupJoin(storeDBContext.ProductStatusTypes,
            //        p1 => p1.p.StatusCode, s => s.StatusCode,
            //        (p1, s) => new { p1, s })
            //        .SelectMany(p2 => p2.s.DefaultIfEmpty(), (p2, s2) => new { p2 = p2.p1, s2 = s2 })
            //        .Select(f => new Models.ProductCM
            //        {
            //            ProductId = f.p2.p.ProductId,
            //            ProductName = f.p2.p.ProductName,
            //            CategoryId = f.p2.p.CategoryId,
            //            CategoryName = f.p2.p.Category.CategoryName,
            //            UnitPrice = f.p2.p.UnitPrice,
            //            StatusCode = f.p2.p.StatusCode,
            //            StatusDescription = f.s2.Description,
            //            AvailableSince = f.p2.p.AvailableSince
            //        });

            return query;
        }


        //Cannot use async for a method with out or ref input argument. 
        public IList<Models.ProductCM> GetProductList(Models.ProductSearchField productSearchField, string productSearchText,
                            Decimal? priceLow, Decimal? priceHigh, DateTime? dateFrom, DateTime? dateTo, int? statusCode,
                            Models.PaginationRequest paging, out int totalCount, out int newPageIndex)
        {
            //Query to join parent and child entities (NO OUTTER JOIN) and return custom model
            //var query = storeDBContext.Products
            //      .Join(storeDBContext.Categories, p => p.CategoryId, c => c.CategoryId,
            //            (p, c) => new { p, c })
            //      .Join(storeDBContext.ProductStatusTypes, p2 => p2.p.StatusCode, ps => ps.StatusCode,
            //            (p2, ps) => new Models.ProductCM
            //            {
            //                ProductId = p2.p.ProductId,
            //                ProductName = p2.p.ProductName,
            //                CategoryId = p2.p.CategoryId,
            //                CategoryName = p2.c.CategoryName,
            //                UnitPrice = p2.p.UnitPrice,
            //                StatusCode = p2.p.StatusCode,
            //                StatusDescription = ps.Description,
            //                AvailableSince = p2.p.AvailableSince
            //            });

            //Test calling SP.
            //var rtnList = storeDBContext.Products.FromSql("EXECUTE GetAllCategorisAndProducts").ToList();

            var query = GetProductListQueryable();

            var predicate = PredicateBuilder.True<Models.ProductCM>();
            
            if (!string.IsNullOrEmpty(productSearchText))
            {
                if (productSearchField == Models.ProductSearchField.CategoryId && Util.IsNumeric(productSearchText))
                {
                    int categoryId = Convert.ToInt32(productSearchText);
                    predicate = predicate.And(p => p.CategoryId == categoryId);
                }
                if (productSearchField == Models.ProductSearchField.CategoryName)
                {
                    predicate = predicate.And(p => p.CategoryName.ToLower().Contains(productSearchText.ToLower()));
                }
                if (productSearchField == Models.ProductSearchField.ProductId && Util.IsNumeric(productSearchText))
                {
                    int productId = Convert.ToInt32(productSearchText);
                    predicate = predicate.And(p => p.ProductId == productId);
                }
                if (productSearchField == Models.ProductSearchField.ProductName)
                {
                    predicate = predicate.And(p => p.ProductName.ToLower().Contains(productSearchText.ToLower()));
                }
            }
            if (priceLow != null)
            {
                predicate = predicate.And(p => p.UnitPrice >= priceLow.Value);
            }
            if (priceHigh != null)
            {
                predicate = predicate.And(p => p.UnitPrice <= priceHigh.Value);
            }
            if (dateFrom != null)
            {
                predicate = predicate.And(p => p.AvailableSince >= dateFrom.Value);
            }
            if (dateTo != null)
            {
                predicate = predicate.And(p => p.AvailableSince <= dateTo.Value);
            }
            if (statusCode != null)
            {
                predicate = predicate.And(p => p.StatusCode == statusCode.Value);
            }                        
            query = query.Where(predicate);

            //Core 3.0: set default OrderBy for Queryable Distinct() function.
            if (paging != null && (paging.Sort == null || string.IsNullOrEmpty(paging.Sort.SortBy)))
            {
                paging.Sort = new Models.Sort() {SortBy = "ProductId"};
            }

            //IList<Models.ProductCM> resultList1 = query.ToList();
            IList<Models.ProductCM> resultList =
                GenericSorterPager.GetSortedPagedList<Models.ProductCM>(query, paging, out totalCount);
            
            //For issue when refreshing data after CRUD causes no row in the current page.
            newPageIndex = -1;
            while (paging.PageIndex > 0 && resultList.Count < 1)
            {
                paging.PageIndex -= 1;
                newPageIndex = paging.PageIndex;
                resultList =
                GenericSorterPager.GetSortedPagedList<Models.ProductCM>(query, paging, out totalCount);                
            }            
            return resultList;
        }

        //Multi-column sorting with EF/Linq.
        public IList<Models.ProductCM> GetProductList(Models.ProductSearchField productSearchField, string productSearchText,
                            Decimal? priceLow, Decimal? priceHigh, DateTime? dateFrom, DateTime? dateTo, int? statusCode,
                            Models.PaginationRequest2 paging, out int totalCount, out int newPageIndex)
        {
            var query = GetProductListQueryable();

            var predicate = PredicateBuilder.True<Models.ProductCM>();

            if (!string.IsNullOrEmpty(productSearchText))
            {
                if (productSearchField == Models.ProductSearchField.CategoryId && Util.IsNumeric(productSearchText))
                {
                    int categoryId = Convert.ToInt32(productSearchText);
                    predicate = predicate.And(p => p.CategoryId == categoryId);
                }
                if (productSearchField == Models.ProductSearchField.CategoryName)
                {
                    predicate = predicate.And(p => p.CategoryName.ToLower().Contains(productSearchText.ToLower()));
                }
                if (productSearchField == Models.ProductSearchField.ProductId && Util.IsNumeric(productSearchText))
                {
                    int productId = Convert.ToInt32(productSearchText);
                    predicate = predicate.And(p => p.ProductId == productId);
                }
                if (productSearchField == Models.ProductSearchField.ProductName)
                {
                    predicate = predicate.And(p => p.ProductName.ToLower().Contains(productSearchText.ToLower()));
                }
            }
            if (priceLow != null)
            {
                predicate = predicate.And(p => p.UnitPrice >= priceLow.Value);
            }
            if (priceHigh != null)
            {
                predicate = predicate.And(p => p.UnitPrice <= priceHigh.Value);
            }
            if (dateFrom != null)
            {
                predicate = predicate.And(p => p.AvailableSince >= dateFrom.Value);
            }
            if (dateTo != null)
            {
                predicate = predicate.And(p => p.AvailableSince <= dateTo.Value);
            }
            if (statusCode != null)
            {
                predicate = predicate.And(p => p.StatusCode == statusCode.Value);
            }
            query = query.Where(predicate); 

            //Core 3.0: set default OrderBy for Queryable Distinct() function.
            if (paging != null && paging.SortList.Count == 0)
            {                
                paging.SortList.Add(new Models.SortItem {SortBy = "ProductId"});
            }

            //IList<Models.ProductCM> resultList1 = query.ToList();
            IList<Models.ProductCM> resultList =
                GenericMultiSorterPager.GetSortedPagedList<Models.ProductCM>(query, paging, out totalCount);                

            //For issue when refreshing data after CRUD causes no row in the current page.
            newPageIndex = -1;
            while (paging.PageIndex > 0 && resultList.Count < 1)
            {
                paging.PageIndex -= 1;
                newPageIndex = paging.PageIndex;
                resultList =
                GenericMultiSorterPager.GetSortedPagedList<Models.ProductCM>(query, paging, out totalCount);
            }
            return resultList;
        }

        public IList<Models.ProductCM> GetProductListSp(string filterString, string sortString, int pageIndex,
                                        int pageSize, out int totalCount, out int newPageIndex)
        {             
            var resultList = storeDBContext.GetProductListSp(filterString, sortString, pageIndex + 1, pageSize, out totalCount);

            //For issue when refreshing data after CRUD causes no row in the current page.
            newPageIndex = -1;
            while (pageIndex > 0 && resultList.Count < 1)
            {
                pageIndex -= 1;
                newPageIndex = pageIndex;
                resultList = storeDBContext.GetProductListSp(filterString, sortString, pageIndex + 1, pageSize, out totalCount);
            }
            return resultList;
        }

        public async Task<IList<Models.ProductCM>> GetProductListNew(List<int> newProductIds)
        {
            var query = GetProductListQueryable();

            query = query.Where(a => newProductIds.Contains(a.ProductId));
            return await query.ToListAsync();            
        }

        public IList<Models.ProductCM> GetFullProducts(int categoryId, Models.PaginationRequest paging, out int totalCount, out int newPageIndex)
        {
            var query = GetProductListQueryable();

            //Add predicate for dynamic search
            var predicate = PredicateBuilder.True<Models.ProductCM>();
            if (categoryId > 0)
            {
                //predicate = predicate.And(p => p.CategoryId == categoryId);
                predicate = predicate.And(p => p.CategoryId == categoryId);
            }
            query = query.Where(predicate);

            //Core 3.0: set default OrderBy for Queryable Distinct() function.
            if (paging != null && (paging.Sort == null || string.IsNullOrEmpty(paging.Sort.SortBy)))
            {
                paging.Sort = new Models.Sort() { SortBy = "ProductId" };
            }

            IList<Models.ProductCM> resultList = GenericSorterPager.GetSortedPagedList<Models.ProductCM>(query, paging, out totalCount);

            //For issue when refreshing data after CRUD causes no row in the current page.
            newPageIndex = -1;
            while (paging.PageIndex > 0 && resultList.Count < 1)
            {
                paging.PageIndex -= 1;
                newPageIndex = paging.PageIndex;
                resultList =
                GenericSorterPager.GetSortedPagedList<Models.ProductCM>(query, paging, out totalCount);
            }    
            return resultList;
        }

        //Not used.
        public IList<Models.ProductCM> GetProductByCategoryId(int categoryId, Models.PaginationRequest paging, out int totalCount, out int newPageIndex)
        {
            //Core 3.0: Product entity is its table excluding any related entity, such as ProductStatusType.
            //IQueryable<Entities.Product> query = storeDBContext.Products
            //                             .Where(a => a.CategoryId == categoryId)
            //                            .Include(a => a.ProductStatusType)
            //                            ;

            var query = GetProductListQueryable();
            query = query.Where(a => a.CategoryId == categoryId); 

            //Core 3.0: set default OrderBy for Queryable Distinct() function.
            if (paging != null && (paging.Sort == null || string.IsNullOrEmpty(paging.Sort.SortBy)))
            {
                paging.Sort = new Models.Sort() { SortBy = "ProductId" };
            }

            var resultList = GenericSorterPager.GetSortedPagedList<Models.ProductCM>(query, paging, out totalCount);

            //For issue when refreshing data after CRUD causes no row in the current page.
            newPageIndex = -1;
            while (paging.PageIndex > 0 && resultList.Count < 1)
            {
                paging.PageIndex -= 1;
                newPageIndex = paging.PageIndex;
                resultList =
                GenericSorterPager.GetSortedPagedList<Models.ProductCM>(query, paging, out totalCount);
            }    
            return resultList;
        }

        public async Task<IList<Models.ProductCM>> GetAllProductsByCategoryId(int categoryId)
        {
            var query = GetProductListQueryable();

            //Add predicate for dynamic search
            var predicate = PredicateBuilder.True<Models.ProductCM>();
            if (categoryId > 0)
            {
                //predicate = predicate.And(p => p.CategoryId == categoryId);
                predicate = predicate.And(p => p.CategoryId == categoryId);
            }
            query = query.Where(predicate);
            return await query.ToListAsync();
        }

        public async Task<int> AddProduct(Entities.Product inputEt)        
        {
            //Due to using DatabaseGeneratedOption.None.
            int maxId = storeDBContext.Products.Max(a => (int?)a.ProductId) ?? 0;
            if (maxId == 0) throw new Exception("Error assign ProductId.");
            inputEt.ProductId = maxId + 1;

            inputEt.AuditTime = DateTime.Now;
            await this.InsertAsync(inputEt, true);            
            
            return inputEt.ProductId;
        }

        public async Task UpdateProduct(Entities.Product inputEt)
        {
            //Get entity to be updated
            Entities.Product updEt = GetProductById(inputEt.ProductId).Result;

            if (!string.IsNullOrEmpty(inputEt.ProductName)) updEt.ProductName = inputEt.ProductName;
            if (inputEt.CategoryId > 0) updEt.CategoryId = inputEt.CategoryId;
            if (inputEt.UnitPrice != null) updEt.UnitPrice = inputEt.UnitPrice;
            if (inputEt.StatusCode > 0)
            {
                updEt.StatusCode = inputEt.StatusCode;
            }
            else
            {
                updEt.StatusCode = null;
            }
            if (inputEt.AvailableSince != null && inputEt.AvailableSince > new DateTime())
            {
                updEt.AvailableSince = inputEt.AvailableSince;
            }
            updEt.AuditTime = DateTime.Now;
            await this.UpdateAsync(updEt, true);            
        }        
         
        public async Task DeleteProduct(int id)
        {
            await this.DeleteAsync(id, true);
        }
    }
}
