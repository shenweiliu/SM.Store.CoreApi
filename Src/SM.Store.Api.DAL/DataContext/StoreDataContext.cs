using Microsoft.EntityFrameworkCore;
using SM.Store.Api.Contracts.Entities;
using Models = SM.Store.Api.Contracts.Models;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Data.SqlClient;
using System.Data;

namespace SM.Store.Api.DAL
{
    public class StoreDataContext : DbContext
    {
        //public string ConnectionString { get; set; }

        public StoreDataContext(DbContextOptions<StoreDataContext> options) : base(options)
        {
        }

        public DbSet<Models.ProductCM> ProductCMs { get; set; }
        public DbSet<Product> Products { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<ProductStatusType> ProductStatusTypes { get; set; }
        public DbSet<Contact> Contacts { get; set; }


        protected override void OnModelCreating(ModelBuilder builder)
        {
            //base.OnModelCreating(builder);
            builder.Entity<Product>().ToTable("Product");
            builder.Entity<Category>().ToTable("Category");
            builder.Entity<ProductStatusType>().ToTable("ProductStatusType");
            builder.Entity<Contact>().ToTable("Contact");

            //For GetProductListSp.
            //builder.Query<Models.ProductCM>();
            //Core 3.0:
            builder.Entity<Models.ProductCM>().HasNoKey();
        }

        public virtual List<Models.ProductCM> GetProductListSp(string filterString, string sortString, int pageNumber, int pageSize, out int totalCount)
        {
            filterString = filterString ?? string.Empty;
            var _filterString = new SqlParameter("@FilterString", filterString);

            sortString = sortString ?? string.Empty;
            var _sortString = new SqlParameter("@SortString", sortString);

            if (pageNumber == 0) pageNumber = 1;
            var _pageNumber = new SqlParameter("@PageNumber", pageNumber);

            var _pageSize = new SqlParameter("@PageSize", pageSize);

            var _totalCount = new SqlParameter("@TotalCount", SqlDbType.Int);
            _totalCount.Direction = ParameterDirection.Output;

            var result = this.ProductCMs.FromSqlRaw("dbo.GetPagedProductList " +
                    "@FilterString, @SortString, @PageNumber, @PageSize, @TotalCount OUT",
                    _filterString, _sortString, _pageNumber, _pageSize, _totalCount).ToList();

            //FromSqlInterpolated doesn't support OUTPUT parameter.
            //var result = this.ProductCMs.FromSqlInterpolated($"EXEC dbo.GetPagedProductList @FilterString={filterString}, @SortString={sortString}, @PageNumber={pageNumber}, @PageSize={pageSize}, @TotalCount OUTPUT={ _totalCount}")
            //    .ToList();

            totalCount = (int)_totalCount.Value;

            return result;
        }

        //protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        //{
        //    //base.OnConfiguring(optionsBuilder);

        //    //if (optionsBuilder.IsConfigured)
        //    //    return;

        //    //ConnectionString = Configuration.GetValue<string>("ConnectionStrings:StoreDbConnection");
        //    ConnectionString = "Server=(localdb)\\SQLLocal2016; Integrated Security = true; Initial Catalog = StoreCF8;";
        //    optionsBuilder.UseSqlServer(ConnectionString);
        //}
    }
}