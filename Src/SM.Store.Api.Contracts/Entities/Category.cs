using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SM.Store.Api.Contracts.Entities
{
    public class Category
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int CategoryId { get; set; }
        public string CategoryName { get; set; }
        public DateTime? AuditTime { get; set; }
                
        public virtual ICollection<Product> Products { get; set; }
    }
}
