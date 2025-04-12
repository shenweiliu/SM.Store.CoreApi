using System;
using System.Collections.Generic;

namespace SM.Store.Api.Contracts.Models
{ 
    public class ContactListResponse
    {
        public Contacts Contacts { get; set; }
    }
    
    public class Contacts : List<Contact> { }

    public class AddContactResponse
    {
        public int ContactId { get; set; }
    }

    public class AddContactsResponse
    {
        public List<int> ContactIdList { get; set; }
    }
    
}
