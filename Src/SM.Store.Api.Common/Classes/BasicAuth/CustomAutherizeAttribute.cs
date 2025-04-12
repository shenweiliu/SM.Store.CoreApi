using Microsoft.AspNetCore.Authorization;
using System;
using System.Collections.Generic;
using System.Configuration;
 
namespace SM.Store.Api.Common
{
    [AttributeUsageAttribute(AttributeTargets.Class | AttributeTargets.Method, Inherited = true, AllowMultiple = true)]
    public class CustomAuthorizeAttribute : AuthorizeAttribute
    {
        public CustomAuthorizeAttribute(string policy = "RoleBasedBasicAuth")
        {            
            this.Policy = policy;
        }        
    }
}
