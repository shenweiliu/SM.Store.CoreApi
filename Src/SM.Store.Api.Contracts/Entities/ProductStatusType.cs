using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SM.Store.Api.Contracts.Entities
{
    public class ProductStatusType
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int StatusCode { get; set; }
        public string Description { get; set; }
        public System.DateTime? AuditTime { get; set; }

        public virtual ICollection<Product> Products { get; set; }
    }
}
