using System.Collections.Generic;
using System;
using System.Linq;
using SM.Store.Api.Common;
using System.Threading.Tasks;
using SM.Store.Api.Contracts.Interfaces;
using Entities = SM.Store.Api.Contracts.Entities;
using Models = SM.Store.Api.Contracts.Models;

namespace SM.Store.Api.BLL
{
    public class ProductBS : IProductBS
    {
        private IProductRepository _productRepository;
        
        public ProductBS(IProductRepository productRepository)
        {
            if (productRepository != null)
                this._productRepository = productRepository;            
        }
        
        public async Task<IList<Entities.Product>> GetProducts()
        {
            return await this._productRepository.GetProducts();
        }

        public async Task<Entities.Product> GetProductById(int id)
        {
            return await this._productRepository.GetProductById(id);
        }

        //Not used.
        //public IList<Models.ProductCM> GetProductByCategoryId(int categoryId, Models.PaginationRequest paging, out int totalCount, out int newPageIndex)
        //{
        //    return this._productRepository.GetProductByCategoryId(categoryId, paging, out totalCount, out newPageIndex);
        //}

        public IList<Models.ProductCM> GetFullProducts(int categoryId, Models.PaginationRequest paging, out int totalCount, out int newPageIndex)
        {
            return this._productRepository.GetFullProducts(categoryId, paging, out totalCount, out newPageIndex);
        }

        public IList<Models.ProductCM> GetProductList(Models.ProductSearchField productSearchField, string productSearchText,
                            Decimal? priceLow, Decimal? priceHigh, DateTime? dateFrom, DateTime? dateTo, int? statusCode,
                            Models.PaginationRequest paging, out int totalCount, out int newPageIndex)
        {
            return this._productRepository.GetProductList(productSearchField, productSearchText,
                   priceLow, priceHigh, dateFrom, dateTo, statusCode, paging, out totalCount, out newPageIndex);
        }

        public IList<Models.ProductCM> GetProductList(Models.ProductSearchField productSearchField, string productSearchText,
                            Decimal? priceLow, Decimal? priceHigh, DateTime? dateFrom, DateTime? dateTo, int? statusCode,
                            Models.PaginationRequest2 paging, out int totalCount, out int newPageIndex)
        {
            return this._productRepository.GetProductList(productSearchField, productSearchText,
                   priceLow, priceHigh, dateFrom, dateTo, statusCode, paging, out totalCount, out newPageIndex);
        }

        public IList<Models.ProductCM> GetProductListSp(string filterString, string sortString, int pageIndex,
                                        int pageSize, out int totalCount, out int newPageIndex)
        {
            return this._productRepository.GetProductListSp(filterString, sortString, pageIndex,
                                        pageSize, out totalCount, out newPageIndex);
        }

        public async Task<IList<Models.ProductCM>> GetProductListNew(List<int> newProductIds)
        {
            return await this._productRepository.GetProductListNew(newProductIds);
        }

        public async Task<IList<Models.ProductCM>> GetAllProductsByCategoryId(int categoryId)
        {
            return await this._productRepository.GetAllProductsByCategoryId(categoryId);
        }
        
        public async Task<int> AddProduct(Entities.Product inputEt)
        {
            return await this._productRepository.AddProduct(inputEt);
        }
        public async Task UpdateProduct(Entities.Product inputEt)
        {
            await this._productRepository.UpdateProduct(inputEt);
        }
        public async Task DeleteProduct(int id)
        {
            await this._productRepository.DeleteProduct(id);            
        }
    }
}
