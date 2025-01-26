using System;
using System.Collections.Generic;

namespace SM.Store.Api.Contracts.TestModels
{
    public enum SortDirection
    {
        Ascending,
        Descending
    }

    public class PagingRequest
    {        
        public int PageIndex { get; set; }
        public int PageSize { get; set; }
        public Sort Sort { get; set; }
        public PagingRequest()
        {
            this.Sort = new Sort();
        }
    }

    public class Sort
    {
        public string SortBy { get; set; }
        public SortDirection SortDirection { get; set; } 
    }

    public class PagingSortRequest
    {
        public int PageIndex { get; set; }
        public int PageSize { get; set; }
        public Sort[] Sort { get; set; }               
    }

    //For test of passing string list or array.
    public class Sort2
    {
        public string SortBy { get; set; }
        public SortDirection SortDirection { get; set; }
        public List<string> InStrings { get; set; }        
    }

    public class PagingSortRequest2
    {
        public int PageIndex { get; set; }
        public int PageSize { get; set; }
        public string[] RootStrings { get; set; }
        public Sort2[] Sort2 { get; set; }
    }
}
