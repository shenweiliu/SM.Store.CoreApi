﻿
namespace SM.Store.Api.Contracts.Models
{
    using System;
        
    public class Status
    {
        public int StatusCode { get; set; }
        public string StatusMsg { get; set; }
        public string Details { get; set; }
    }

}
