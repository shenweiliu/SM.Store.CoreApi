using System;
using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json;

namespace SM.Store.Api.Common
{
    public class ErrorModel
    {
        public ErrorStatusCode StatusCode { get; set; }
        public string Description { get; set; }
        public string Details { get; set; }
        public string StackTraces { get; set; }
        public string AppName { get; set; }
    }

    //Return response for client call.
    public class ErrorDetails
    {
        public int StatusCode { get; set; }
        public string Message { get; set; }
        public override string ToString()
        {
            return JsonConvert.SerializeObject(this);
        }
    }

}
