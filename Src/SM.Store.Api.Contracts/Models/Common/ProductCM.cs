using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace SM.Store.Api.Contracts.Models
{    
    public partial class ProductCM
    {
        public int ProductId { get; set; }
        public string ProductName { get; set; }
        public int CategoryId { get; set; }
        public string CategoryName { get; set; } 
        public Nullable<decimal> UnitPrice { get; set; }
        public Nullable<int> StatusCode { get; set; }
        public string StatusDescription { get; set; }
        public Nullable<DateTime> AvailableSince { get; set; }      
    }
    
}
