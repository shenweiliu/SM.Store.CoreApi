using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using Microsoft.AspNetCore.Mvc;
using SM.Store.Api.Common;
using SM.Store.Api.Contracts.TestModels;

namespace SM.Store.Api.Http
{
    [Route("api")]
    public class TestModelBinderController : ControllerBase
    {
        [HttpGet("nvpstonestget")]
        public NestSearchRequest Get_NvpsToNest([ModelBinder(typeof(FieldValueModelBinder))] NestSearchRequest request)
        {
            return request;
        }

        //[HttpGet("~/api/nvpstonestget")]
        //public NestSearchRequest Get_NvpsToNest([FromUri] NestSearchRequest request)
        //{
        //    return request;
        //}

        [HttpGet("nvpstonestpost")]
        public NestSearchRequest Post_NvpsToNest([ModelBinder(typeof(FieldValueModelBinder))] NestSearchRequest request)
        {
            return request;
        }

        [HttpGet("nvpstonestcollectionget")]
        public ComplexSearchRequest Get_NvpsToNestCollection([ModelBinder(typeof(FieldValueModelBinder))] ComplexSearchRequest request)
        {
            return request;
        }

        [HttpGet("stringlistorarrayget")]
        public PagingSortRequest2 Get_StringListOrArray([ModelBinder(typeof(FieldValueModelBinder))] PagingSortRequest2 request)
        {
            return request;
        }

        //Used for testing passing json object.
        [HttpGet("stringlistorarraypost")]
        public PagingSortRequest2 Post_StringListOrArray([ModelBinder(typeof(FieldValueModelBinder))] PagingSortRequest2 request)
        {
            return request;
        } 
    }
}
