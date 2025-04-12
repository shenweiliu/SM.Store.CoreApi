using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using SM.Store.Api.BLL;
using SM.Store.Api.Common;
using SM.Store.Api.DAL;
using SM.Store.Api.Contracts.Configurations;
using SM.Store.Api.Contracts.Interfaces;
using Entities = SM.Store.Api.Contracts.Entities;
using Models = SM.Store.Api.Contracts.Models;

namespace SM.Store.Api.Web
{    
    [CustomAuthorize]
    [ApiController]
    [Route("api")]
    public class LookupController : ControllerBase
    {
        private ILookupBS bs;
        private IOptions<AppConfig> config { get; set; }

        public LookupController(ILookupBS lookupBS, IOptions<AppConfig> appConfig)
        {
            bs = lookupBS;
            config = appConfig;
        }

        //For tutorial and test only. 
        [HttpGet("lookupcategorieserror")]
        public List<Models.Category> Get_LookupCategoriesError()
        {
            if (StaticConfigs.GetConfig("Environment") != "PROD")
            {
                //Test global error handler and report.
                throw new Exception("Test error for lookup categories.");
            }
            return new List<Models.Category>();
        }

        [HttpGet("lookupcategories")]
        public IList<Models.Category> Get_LookupCategories()
        {
            //For test global error handler and report.
            //throw new Exception("Test error for lookup categories.");
            
            IList<Models.Category> rtnList = bs.LookupCategories();
            return rtnList;
        }

        [HttpGet("lookupproductstatustypes")]
        public IList<Models.ProductStatusType> Get_LookupProductStatusTypes()
        {
            IList<Models.ProductStatusType> rtnList = bs.LookupProductStatusTypes();
            return rtnList;
        }
    }
}
