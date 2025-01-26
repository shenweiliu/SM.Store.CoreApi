using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace SM.Store.Api.Contracts.Models
{
    public class Contact
    {       
        public int ContactId { get; set; }
        public string ContactName { get; set; }
        public string Phone { get; set; }
        public string Email { get; set; }
        public int PrimaryType { get; set; } // 1: Phone; 2: Email.        
    }
}
