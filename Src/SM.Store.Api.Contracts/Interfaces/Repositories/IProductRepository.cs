using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SM.Store.Api.Contracts.Interfaces
{
    public interface IProductRepository : IGenericRepository<Entities.Product>
    {
        Task<IList<Entities.Product>> GetProducts();
        Task<Entities.Product> GetProductById(int id);
                
        IList<Models.ProductCM> GetProductList(Models.ProductSearchField productSearchField, string productSearchText,
                            Decimal? priceLow, Decimal? priceHigh, DateTime? dateFrom, DateTime? dateTo, int? statusCode,
                            Models.PaginationRequest paging, out int totalCount, out int newPageIndex);

        IList<Models.ProductCM> GetProductList(Models.ProductSearchField productSearchField, string productSearchText,
                            Decimal? priceLow, Decimal? priceHigh, DateTime? dateFrom, DateTime? dateTo, int? statusCode,
                            Models.PaginationRequest2 paging, out int totalCount, out int newPageIndex);

        IList<Models.ProductCM> GetProductListSp(string filterString, string sortString, int pageIndex,
                                        int pageSize, out int totalCount, out int newPageIndex);

        Task<IList<Models.ProductCM>> GetProductListNew(List<int> newProductIds);
        
        //Not used.
        //IList<Models.ProductCM> GetProductByCategoryId(int categoryId, Models.PaginationRequest paging, out int totalCount, out int newPageIndex);
        
        IList<Models.ProductCM> GetFullProducts(int categoryId, Models.PaginationRequest paging, out int totalCount, out int newPageIndex);
        Task<IList<Models.ProductCM>> GetAllProductsByCategoryId(int categoryId);

        Task<int> AddProduct(Entities.Product inputEt);
        Task UpdateProduct(Entities.Product inputEt);
        Task DeleteProduct(int id);
    }
}
