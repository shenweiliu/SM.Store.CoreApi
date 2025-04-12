using System;
using System.Collections.Generic;

namespace SM.Store.Api.Contracts.Models
{
    //This enum request doesn't work with Swagger. 
    //https://stackoverflow.com/questions/36452468/swagger-ui-web-api-documentation-present-enums-as-strings
    //1. Add these to starup ConfigurationServices (NOT Working):
    //services.AddMvc()
    //...
    //.AddNewtonsoftJson(opts =>
    //{
    //    opts.SerializerSettings.Converters.Add(new StringEnumConverter());
    //});
    //services.AddSwaggerGen(...);
    //services.AddSwaggerGenNewtonsoftSupport(); 

    //Or return emun string by using (NOT Working):
    //using System.Text.Json.Serialization;
    //[JsonConverter(typeof(JsonStringEnumConverter))]
    public enum SortDirection
    {
        Ascending,
        Descending
    }

    public class PaginationRequest
    {        
        public int PageIndex { get; set; }
        public int PageSize { get; set; }
        //Change to SortItem for using Swagger.
        public Sort Sort { get; set; }
        //public SortItem Sort { get; set; }
        //public PaginationRequest()
        //{
        //    this.Sort = new SortItem();
        //}
    }

    public class Sort
    {
        public string SortBy { get; set; }
        public SortDirection SortDirection { get; set; }  
    }

    public class PaginationRequest2
    {
        public int PageIndex { get; set; }
        public int PageSize { get; set; }
        public List<SortItem> SortList { get; set; }
        public PaginationRequest2()
        {
            this.SortList = new List<SortItem>();
        }
    }

    public class SortItem
    {
        public string SortBy { get; set; }
        public string SortDirection { get; set; } //"asc", "desc", "".
    }
}
