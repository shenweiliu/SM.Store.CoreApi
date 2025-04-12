using System;
using System.Collections.Generic;

namespace SM.Store.Api.Contracts.TestModels
{   
    public class NestSearchRequest
    {
        public int CategoryId { get; set; }
        public PagingRequest PagingRequest { get; set; }        
    }
        
    public class ComplexSearchRequest
    {
        public int CategoryId { get; set; }
        public List<PagingSortRequest> PagingRequest { get; set; }
        //public PagingSortRequests PagingRequest { get; set; }
        //public PagingSortRequest[] PagingRequest { get; set; }
        public string Test { get; set; }
    }

    public class PagingSortRequests : List<PagingSortRequest> {}
}

