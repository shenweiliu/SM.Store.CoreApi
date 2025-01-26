using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace SM.Store.Api.Common
{      
    public enum ErrorStatusCode
    {
        //Custom status code:
        GENERAL_ERROR = 500,
        AUTHENTICATION_ERROR = 451,
        DATABASE_ERROR = 452,
        DATA_VALIDATION_ERROR = 453,
        DATA_DUPLICATE_ERROR = 454        
    }
}
