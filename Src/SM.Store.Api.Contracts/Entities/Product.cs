using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SM.Store.Api.Contracts.Entities
{
    public class Product
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int ProductId { get; set; }
        public string ProductName { get; set; }
        public int CategoryId { get; set; }
        public Decimal? UnitPrice { get; set; }
        public int? StatusCode { get; set; }
        public System.DateTime? AvailableSince { get; set; }
        public System.DateTime? AuditTime { get; set; }

        public virtual Category Category { get; set; }
        public virtual ProductStatusType ProductStatusType { get; set; }
    }
}
