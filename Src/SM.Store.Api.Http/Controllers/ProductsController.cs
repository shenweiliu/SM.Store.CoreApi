using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using SM.Store.Api.BLL;
using SM.Store.Api.Common;
using SM.Store.Api.DAL;
using SM.Store.Api.Contracts.Configurations;
using SM.Store.Api.Contracts.Interfaces;
using Entities = SM.Store.Api.Contracts.Entities;
using Models = SM.Store.Api.Contracts.Models;
using System.Threading.Tasks;

namespace SM.Store.Api.Http
{
    [CustomAuthorize]
    [Route("api")]
    public class ProductsController : ControllerBase
    {
        private IProductBS bs;
        private IAutoMapConverter<Entities.Product, Models.Product> mapEntityToModel;
        private IAutoMapConverter<Models.Product, Entities.Product> mapModelToEntity;
        private IOptions<AppConfig> config { get; set; }
        public ProductsController(IProductBS productBS,
                                  IAutoMapConverter<Entities.Product, Models.Product> convertEntityToModel,
                                  IAutoMapConverter<Models.Product, Entities.Product> convertModelToEntity,
                                  IOptions<AppConfig> appConfig)
        {
            bs = productBS;
            mapEntityToModel = convertEntityToModel;
            mapModelToEntity = convertModelToEntity;
            config = appConfig;
        }
                
        [HttpGet("getproductlist")]
        //Passing complex object type queryString causes error when using built-in FromQuery model binder.
        public Models.PagedProductListResponse GetProductList([ModelBinder(typeof(FieldValueModelBinder))] Models.PagedProductListRequest request)
        {
            var resp = new Models.PagedProductListResponse();
            if (request == null)
            {
                return resp;
            }

            Models.ProductSearchField searchField = 0;
            string searchText = null;
            Decimal? priceLow = null;
            Decimal? priceHigh = null;
            DateTime? dateFrom = null;
            DateTime? dateTo = null;
            if (request.ProductSearchFilter != null)
            {
                searchField = request.ProductSearchFilter.ProductSearchField;
                searchText = request.ProductSearchFilter.ProductSearchText;
            }
            if (request.PriceSearchFilter != null)
            {
                if (!String.IsNullOrEmpty(request.PriceSearchFilter.SearchPriceLow)) priceLow = Convert.ToDecimal(request.PriceSearchFilter.SearchPriceLow);
                if (!String.IsNullOrEmpty(request.PriceSearchFilter.SearchPriceHigh)) priceHigh = Convert.ToDecimal(request.PriceSearchFilter.SearchPriceHigh);
            }
            if (request.DateSearchFilter != null)
            {
                if (!String.IsNullOrEmpty(request.DateSearchFilter.SearchDateFrom)) dateFrom = Convert.ToDateTime(request.DateSearchFilter.SearchDateFrom);
                if (!String.IsNullOrEmpty(request.DateSearchFilter.SearchDateTo)) dateTo = Convert.ToDateTime(request.DateSearchFilter.SearchDateTo);
            }
            int totalCount = 0;
            int newPageIndex = -1;

            IEnumerable<Models.ProductCM> rtnList = bs.GetProductList(searchField, searchText,
                       priceLow, priceHigh, dateFrom, dateTo, request.StatusCode, request.PaginationRequest,
                       out totalCount, out newPageIndex);
            resp.Products.AddRange(rtnList);
            //Test get json:
            //var output = JsonConvert.SerializeObject(resp.Products);

            resp.TotalCount = totalCount;
            return resp;
        }

        //"getproductlist_p" is legacy method with one sort field.
        //Request object contains complex multi-level enum SortDirection which is not supported by Swagger.
        [ApiExplorerSettings(IgnoreApi = true)]
        [HttpPost("getproductlist_p")]
        public Models.PagedProductListResponse Post_GetProductList([FromBody] Models.GetProductsBySearchRequest request)
        {
            //Test getting config value.
            //var te = config.Value.TestConfig1;

            var resp = new Models.PagedProductListResponse();
            if (request == null)
            {
                return resp;
            }

            Models.ProductSearchField searchField = 0;
            string searchText = null;
            Decimal? priceLow = null;
            Decimal? priceHigh = null;
            DateTime? dateFrom = null;
            DateTime? dateTo = null;

            if (request.NewProductIds != null && request.NewProductIds.Count > 0)
            {
                //For refresh data with newly added products.
                IList<Models.ProductCM> rtnList = bs.GetProductListNew(request.NewProductIds).Result;
                resp.Products.AddRange(rtnList);
            }
            else
            {
                if (request.ProductSearchFilter != null)
                {
                    searchField = request.ProductSearchFilter.ProductSearchField;
                    searchText = request.ProductSearchFilter.ProductSearchText;
                }
                if (request.PriceSearchFilter != null)
                {
                    if (!String.IsNullOrEmpty(request.PriceSearchFilter.SearchPriceLow)) priceLow = Convert.ToDecimal(request.PriceSearchFilter.SearchPriceLow);
                    if (!String.IsNullOrEmpty(request.PriceSearchFilter.SearchPriceHigh)) priceHigh = Convert.ToDecimal(request.PriceSearchFilter.SearchPriceHigh);
                }
                if (request.DateSearchFilter != null)
                {
                    if (!String.IsNullOrEmpty(request.DateSearchFilter.SearchDateFrom)) dateFrom = Convert.ToDateTime(request.DateSearchFilter.SearchDateFrom);
                    if (!String.IsNullOrEmpty(request.DateSearchFilter.SearchDateTo)) dateTo = Convert.ToDateTime(request.DateSearchFilter.SearchDateTo);
                }

                int totalCount = 0;
                int newPageIndex = -1;

                IList<Models.ProductCM> rtnList = bs.GetProductList(searchField, searchText,
                                        priceLow, priceHigh, dateFrom, dateTo, request.StatusCode, request.PaginationRequest,
                                        out totalCount, out newPageIndex);
                resp.Products.AddRange(rtnList);
                resp.TotalCount = totalCount;
                resp.newPageIndex = newPageIndex;
            }

            //Test loader.
            //System.Threading.Thread.Sleep(3000);

            return resp;
        }

        [HttpPost("getpagedproductlist")]        
        public Models.PagedProductListResponse Post_GetPagedProductList([FromBody] Models.PagedProductListRequest request)
        {
            var resp = new Models.PagedProductListResponse();
            if (request == null)
            {
                return resp;
            }

            Models.ProductSearchField searchField = 0;
            string searchText = null;
            Decimal? priceLow = null;
            Decimal? priceHigh = null;
            DateTime? dateFrom = null;
            DateTime? dateTo = null;

            if (request.NewProductIds != null && request.NewProductIds.Count > 0)
            {
                //For refresh data with newly added products.
                IList<Models.ProductCM> rtnList = bs.GetProductListNew(request.NewProductIds).Result;
                resp.Products.AddRange(rtnList);
            }
            else
            {
                if (request.ProductSearchFilter != null)
                {
                    searchField = request.ProductSearchFilter.ProductSearchField;
                    searchText = request.ProductSearchFilter.ProductSearchText;
                }
                if (request.PriceSearchFilter != null)
                {
                    if (!String.IsNullOrEmpty(request.PriceSearchFilter.SearchPriceLow)) priceLow = Convert.ToDecimal(request.PriceSearchFilter.SearchPriceLow);
                    if (!String.IsNullOrEmpty(request.PriceSearchFilter.SearchPriceHigh)) priceHigh = Convert.ToDecimal(request.PriceSearchFilter.SearchPriceHigh);
                }
                if (request.DateSearchFilter != null)
                {
                    if (!String.IsNullOrEmpty(request.DateSearchFilter.SearchDateFrom)) dateFrom = Convert.ToDateTime(request.DateSearchFilter.SearchDateFrom);
                    if (!String.IsNullOrEmpty(request.DateSearchFilter.SearchDateTo)) dateTo = Convert.ToDateTime(request.DateSearchFilter.SearchDateTo);
                }

                int totalCount = 0;
                int newPageIndex = -1;

                IList<Models.ProductCM> rtnList = bs.GetProductList(searchField, searchText,
                                        priceLow, priceHigh, dateFrom, dateTo, request.StatusCode, request.PaginationRequest,
                                        out totalCount, out newPageIndex);
                resp.Products.AddRange(rtnList);
                resp.TotalCount = totalCount;
                resp.newPageIndex = newPageIndex;
            }

            //Test loader.
            //System.Threading.Thread.Sleep(3000);

            return resp;
        }
                
        [HttpPost("getpagedproductlistbysp")]        
        public Models.PagedProductListResponse Post_GetPagedProductListSp([FromBody] Models.PagedProductListRequest request)
        {
            var resp = new Models.PagedProductListResponse();
            if (request == null)
            {
                return resp;
            }

            if (request.NewProductIds != null && request.NewProductIds.Count > 0)
            {
                //For refresh data with newly added products.
                IList<Models.ProductCM> rtnList = bs.GetProductListNew(request.NewProductIds).Result;
                resp.Products.AddRange(rtnList);
            }
            else
            {
                var filterString = " ";
                var sortString = " ";

                //Build filter string with custom encoding.
                if (request.ProductSearchFilter != null && !string.IsNullOrEmpty(request.ProductSearchFilter.ProductSearchText))
                {
                    //If using Enum names: Enum.GetName(typeof(Models.ProductSearchField), request.ProductSearchFilter.ProductSearchField)
                    if (request.ProductSearchFilter.ProductSearchField == Models.ProductSearchField.CategoryId &&
                        Util.IsNumeric(request.ProductSearchFilter.ProductSearchText))
                    {
                        filterString += " AND " + "CategoryId = " + request.ProductSearchFilter.ProductSearchText;
                    }
                    else if (request.ProductSearchFilter.ProductSearchField == Models.ProductSearchField.CategoryName)
                    {
                        filterString += " AND " + "CategoryName LIKE &dapos;%" + request.ProductSearchFilter.ProductSearchText + "%&dapos;";
                    }
                    else if (request.ProductSearchFilter.ProductSearchField == Models.ProductSearchField.ProductId &&
                        Util.IsNumeric(request.ProductSearchFilter.ProductSearchText))
                    {
                        filterString += " AND " + "ProductId = " + request.ProductSearchFilter.ProductSearchText;
                    }
                    else if (request.ProductSearchFilter.ProductSearchField == Models.ProductSearchField.ProductName)
                    {
                        filterString += " AND " + "ProductName LIKE &dapos;%" + request.ProductSearchFilter.ProductSearchText + "%&dapos;";
                    }
                }
                if (request.PriceSearchFilter != null)
                {
                    if (!string.IsNullOrEmpty(request.PriceSearchFilter.SearchPriceLow) &&
                        Util.IsNumeric(request.PriceSearchFilter.SearchPriceLow))
                    {
                        filterString += " AND " + "UnitPrice >= " + request.PriceSearchFilter.SearchPriceLow;
                    }
                    if (!string.IsNullOrEmpty(request.PriceSearchFilter.SearchPriceHigh) &&
                        Util.IsNumeric(request.PriceSearchFilter.SearchPriceHigh))
                    {
                        filterString += " AND " + "UnitPrice <= " + request.PriceSearchFilter.SearchPriceHigh;
                    }
                }
                if (request.DateSearchFilter != null)
                {
                    if (!string.IsNullOrEmpty(request.DateSearchFilter.SearchDateFrom))
                    {
                        filterString += " AND " + "AvailableSince >= &dapos;" +
                            Convert.ToDateTime(request.DateSearchFilter.SearchDateFrom).ToString("yyyy-MM-dd") + "&dapos;";
                    }
                    if (!string.IsNullOrEmpty(request.DateSearchFilter.SearchDateTo))
                    {
                        filterString += " AND " + "AvailableSince <= &dapos;" +
                            Convert.ToDateTime(request.DateSearchFilter.SearchDateTo).ToString("yyyy-MM-dd") + "&dapos;";
                    }
                }
                if (request.StatusCode != null)
                {
                    filterString += " AND " + "StatusCode = " + request.StatusCode.Value;
                }
                //Multiple column sorting.
                if (request.PaginationRequest.SortList != null && request.PaginationRequest.SortList.Count > 0)
                {
                    foreach (var item in request.PaginationRequest.SortList)
                    {
                        if (sortString != " ") sortString += ", ";
                        sortString += item.SortBy + " " + item.SortDirection;
                    }
                }
                int totalCount = 0;
                int newPageIndex = -1;

                var rtnList = bs.GetProductListSp(filterString, sortString, request.PaginationRequest.PageIndex,
                                        request.PaginationRequest.PageSize, out totalCount, out newPageIndex);
                resp.Products.AddRange(rtnList);
                resp.TotalCount = totalCount;
                resp.newPageIndex = newPageIndex;
            }
            return resp;
        }

        [HttpGet("getproducts")]        
        public async Task<IList<Models.ProductCM>> GetAllProducts()
        {
            IList<Models.ProductCM> rtn = default(IList<Models.ProductCM>);
            try
            {
                rtn = await bs.GetAllProductsByCategoryId(0);
            }
            catch (Exception ex)
            {
                var msg = ex.Message;
            }
            return rtn;
        }

        //"getallproducts" is legacy method with one sort field.
        //Request object contains complex multi-level enum SortDirection which is not supported by Swagger.
        [ApiExplorerSettings(IgnoreApi = true)]
        [HttpGet("getallproducts")]
        public Models.PagedProductListResponse GetAllProducts([ModelBinder(typeof(FieldValueModelBinder))] Models.GetProductsBySearchRequest request)
        {
            var resp = new Models.PagedProductListResponse();
            if (request == null)
            {
                return resp;
            }

            int totalCount = 0;
            int newPageIndex = -1;

            IList<Models.ProductCM> rtnList = bs.GetFullProducts(0, request.PaginationRequest, out totalCount, out newPageIndex);
            resp.Products.AddRange(rtnList);
            resp.TotalCount = totalCount;
            return resp;
        }

        [HttpGet("getallproductsbycategoryid/{categoryId:int}")]
        public async Task<IList<Models.ProductCM>> GetAllProductsByCategoryId(int categoryId)
        {
            return await bs.GetAllProductsByCategoryId(categoryId);
        }

        [HttpGet("getproductbyid/{id:int}")]
        [HttpGet("products/{id:int}")] //for backward compatibility.
        public async Task<IActionResult> GetProductById(int id)
        {
            var eProduct = await bs.GetProductById(id);
            if (eProduct == null)
            {
                return NotFound();
            }
            else
            {
                Models.Product mProduct = mapEntityToModel.ConvertObject(eProduct);
                return Ok(mProduct);
            }
        }

        [HttpPost("addproduct")]
        public async Task<Models.AddProductResponse> Post_AddProduct([FromBody] Models.Product mProduct)
        {
            var eProduct = mapModelToEntity.ConvertObject(mProduct);
            await bs.AddProduct(eProduct);

            var addProductResponse = new Models.AddProductResponse()
            {
                ProductId = eProduct.ProductId
            };
            return addProductResponse;

            ////Generate a link to the new product and set the Location header in the response.
            ////For public HttpResponseMessage Post_AddProduct([FromBody] Models.Product mProduct)
            //var response = new HttpResponseMessage(HttpStatusCode.Created);
            //string uri = Url.Link("GetProductById", new { id = eProduct.ProductId });
            //response.Headers.Location = new Uri(uri);
            //return response;
        }

        [HttpPost("updateproduct")]
        public async Task Post_UpdateProduct([FromBody] Models.Product mProduct)
        {
            var eProduct = mapModelToEntity.ConvertObject(mProduct);
            await bs.UpdateProduct(eProduct);
        }

        [HttpPost("deleteproduct")]
        public async Task DeleteProduct([FromBody] int id)
        {
            await bs.DeleteProduct(id);
        }

        [HttpPost("deleteproducts")]
        public async Task Post_DeleteProduct([FromBody] List<int> ids)
        {
            if (ids.Count > 0)
            {
                //ids.ForEach(delegate (int id)
                //{
                //    await bs.DeleteProduct(id);
                //});
                foreach (int id in ids)
                {
                    await bs.DeleteProduct(id);
                };
            }
        }
    }
}
