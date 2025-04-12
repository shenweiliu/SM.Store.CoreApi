using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SM.Store.Api.Contracts.Entities
{
    public class Contact
    {
        [Key]
        public int ContactId { get; set; }
        public string ContactName { get; set; }
        public string Phone { get; set; }
        public string Email { get; set; }
        public int PrimaryType { get; set; } // 1: Phone; 2: Email.
        public DateTime? AuditTime { get; set; }
    }
}
