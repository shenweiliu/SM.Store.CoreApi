using System.Collections.Generic;

namespace SM.Store.Api.Contracts.Interfaces
{
    public interface ILookupBS 
    {
        IList<Models.Category> LookupCategories();
        IList<Models.ProductStatusType> LookupProductStatusTypes();        
    }
}
