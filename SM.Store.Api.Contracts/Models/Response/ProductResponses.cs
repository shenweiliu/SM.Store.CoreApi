﻿using System;
using System.Collections.Generic;

namespace SM.Store.Api.Contracts.Models
{
    public class ProductResponse 
    {
        public Product Product { get; set; }
    }
    public class ProductCMResponse 
    {
        public ProductCM ProductCM { get; set; }
    }
     
    public class PagedProductListResponse 
    {
        public PagedProductListResponse() {
            Products = new List<ProductCM>();
        }
        public List<ProductCM> Products { get; set; }
        public int TotalCount { get; set; }
        public int newPageIndex { get; set; }
    }

    //public class Products : List<ProductCM> { }

    public class AddProductResponse
    {
        public int ProductId { get; set; }
    }
}
