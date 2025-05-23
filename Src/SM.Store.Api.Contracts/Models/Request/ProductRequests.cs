﻿using System;
using System.Collections.Generic;

namespace SM.Store.Api.Contracts.Models
{
    public enum ProductSearchField
    {
        CategoryId,
        CategoryName,
        ProductName,
        ProductId
    }

    public enum ProductStatusCode
    {
        Available = 1,
        OutOfStock = 2,
        BackOrdered = 3,
        Discontinued = 4,
        Undefined = 5
    }

    public class PagedProductListRequest
    {
        public ProductSearchFilter ProductSearchFilter { get; set; }
        public DateSearchFilter DateSearchFilter { get; set; }
        public PriceSearchFilter PriceSearchFilter { get; set; }
        public int? StatusCode { get; set; }
        public PaginationRequest2 PaginationRequest { get; set; }
        public List<int> NewProductIds { get; set; }
    }

    public class GetProductsBySearchRequest
    {
        public ProductSearchFilter ProductSearchFilter { get; set; }
        public DateSearchFilter DateSearchFilter { get; set; }
        public PriceSearchFilter PriceSearchFilter { get; set; }
        public int? StatusCode { get; set; }
        public PaginationRequest PaginationRequest { get; set; }
        public List<int> NewProductIds { get; set; }
    }

    public class GetProductsBySearchRequestTest
    {
        public ProductSearchFilter ProductSearchFilter { get; set; }
        public DateSearchFilter DateSearchFilter { get; set; }
        public PriceSearchFilter PriceSearchFilter { get; set; }
        public int? StatusCode { get; set; }
        public PaginationRequest PaginationRequest { get; set; }
        public List<int> NewProductIds { get; set; }
    }

    public class ProductSearchFilter
    {
        public ProductSearchField ProductSearchField { get; set; }        
        public string ProductSearchText { get; set; }
    }

    public class DateSearchFilter
    {
        public string SearchDateFrom { get; set; }
        public string SearchDateTo { get; set; }
    }
    
    public class PriceSearchFilter
    {
        public string SearchPriceLow { get; set; }
        public string SearchPriceHigh { get; set; }
    }
    
    public class SaveProductRequest
    {
        public Product Product { get; set; }
    }

    //public class TestArray
    //{
    //    public string[] Names { get; set; }
    //}
}