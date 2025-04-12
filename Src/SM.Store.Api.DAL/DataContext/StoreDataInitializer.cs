using System;
using System.Collections.Generic;
using System.Linq;
using SM.Store.Api.Contracts.Entities;
using Microsoft.EntityFrameworkCore;
using SM.Store.Api.DAL;
using SM.Store.Api.Common;

namespace SM.Store.Api.DAL
{
    public class StoreDataInitializer
    {
        public static void Initialize(StoreDataContext context)
        {
            if (StaticConfigs.GetConfig("UseInMemoryDatabase") != "true")
            {
                context.Database.EnsureCreated();
            }

            if (context.Categories.Any())
            {
                return;
            }

            var statusTypes = new ProductStatusType[]
            {
                new ProductStatusType { StatusCode = 1, Description = "Available", AuditTime = Convert.ToDateTime("2022-08-26")},
                new ProductStatusType { StatusCode = 2, Description = "Out of Stock", AuditTime = Convert.ToDateTime("2022-09-26")},
                new ProductStatusType { StatusCode = 3, Description = "Back Ordered", AuditTime = Convert.ToDateTime("2022-09-26")},
                new ProductStatusType { StatusCode = 4, Description = "Discontinued", AuditTime = Convert.ToDateTime("2022-10-26")},
                new ProductStatusType { StatusCode = 5, Description = "Undefined", AuditTime = Convert.ToDateTime("2022-10-26")}
            };
            context.ProductStatusTypes.AddRange(statusTypes);
            //context.SaveChanges();
            
            var categories = new Category[]
            {
                new Category { CategoryId = 1, CategoryName = "Bath", AuditTime = DateTime.Now },
                new Category { CategoryId = 2, CategoryName = "Bedding", AuditTime = DateTime.Now },
                new Category { CategoryId = 3, CategoryName = "kitchen", AuditTime = DateTime.Now },
                new Category { CategoryId = 4, CategoryName = "Undefined", AuditTime = DateTime.Now }
            };
            context.Categories.AddRange(categories);            
            
            var products = new Product[] {
                new Product { ProductId = 11, ProductName = "Bath Rug", CategoryId = 1, UnitPrice = 24.5m, StatusCode = 1, AvailableSince = Convert.ToDateTime("2023-07-10"), AuditTime = DateTime.Now},
                new Product { ProductId = 12, ProductName = "Shower Curtain", CategoryId = 1, UnitPrice = 30.99m, StatusCode = 4, AvailableSince = Convert.ToDateTime("2022-07-13"), AuditTime = DateTime.Now},
                new Product { ProductId = 13, ProductName = "Soap Dispenser", CategoryId = 1, UnitPrice = 12.4m, StatusCode = 2, AvailableSince = Convert.ToDateTime("2023-08-05"), AuditTime = DateTime.Now},
                new Product { ProductId = 14, ProductName = "Toilet Tissue", CategoryId = 1, UnitPrice = 15, StatusCode = 3, AvailableSince = Convert.ToDateTime("2023-05-16"), AuditTime = DateTime.Now},
                new Product { ProductId = 15, ProductName = "Bath Rug 2", CategoryId = 1, UnitPrice = 26.2m, StatusCode = 1, AvailableSince = Convert.ToDateTime("2022-07-10"), AuditTime = DateTime.Now},
                new Product { ProductId = 16, ProductName = "Shower Curtain 2", CategoryId = 1, UnitPrice = 32.99m, StatusCode = 1, AvailableSince = Convert.ToDateTime("2022-07-13"), AuditTime = DateTime.Now},
                new Product { ProductId = 17, ProductName = "Soap Dispenser 2", CategoryId = 1, UnitPrice = 13.4m, StatusCode = 2, AvailableSince = Convert.ToDateTime("2022-08-05"), AuditTime = DateTime.Now},
                new Product { ProductId = 18, ProductName = "Toilet Tissue 2", CategoryId = 1, UnitPrice = 16, StatusCode = 3, AvailableSince = Convert.ToDateTime("2022-05-16"), AuditTime = DateTime.Now},
                new Product { ProductId = 20, ProductName = "Bath Rug 3", CategoryId = 1, UnitPrice = 27.1m, StatusCode = 1, AvailableSince = Convert.ToDateTime("2023-07-10"), AuditTime = DateTime.Now},
                new Product { ProductId = 21, ProductName = "Shower Curtain 3", CategoryId = 1, UnitPrice = 35.99m, StatusCode = 1, AvailableSince = Convert.ToDateTime("2023-07-13"), AuditTime = DateTime.Now},
                new Product { ProductId = 22, ProductName = "Soap Dispenser 3", CategoryId = 1, UnitPrice = 19.4m, StatusCode = 2, AvailableSince = Convert.ToDateTime("2023-08-05"), AuditTime = DateTime.Now},
                new Product { ProductId = 23, ProductName = "Toilet Tissue 3", CategoryId = 1, UnitPrice = 20, StatusCode = 3, AvailableSince = Convert.ToDateTime("2023-05-16"), AuditTime = DateTime.Now},
                new Product { ProductId = 24, ProductName = "Bath Rug 4", CategoryId = 1, UnitPrice = 28.8m, StatusCode = 1, AvailableSince = Convert.ToDateTime("2023-08-10"), AuditTime = DateTime.Now},
                new Product { ProductId = 25, ProductName = "Shower Curtain 4", CategoryId = 1, UnitPrice = 37.99m, StatusCode = 1, AvailableSince = Convert.ToDateTime("2023-09-13"), AuditTime = DateTime.Now},
                
                new Product { ProductId = 110, ProductName = "Branket", CategoryId = 2, UnitPrice = 60, StatusCode = 1, AvailableSince = Convert.ToDateTime("2022-08-22"), AuditTime = DateTime.Now},
                new Product { ProductId = 112, ProductName = "Mattress Protector", CategoryId = 2, UnitPrice = 30.4m, StatusCode = 2, AvailableSince = Convert.ToDateTime("2022-08-22"), AuditTime = DateTime.Now },
                new Product { ProductId = 113, ProductName = "Sheet Set", CategoryId = 2, UnitPrice = 40.69m, StatusCode = 1, AvailableSince = Convert.ToDateTime("2023-07-26"), AuditTime = DateTime.Now},
                new Product { ProductId = 114, ProductName = "Pillow", CategoryId = 2, UnitPrice = 10.2m, StatusCode = 1, AvailableSince = Convert.ToDateTime("2023-10-04"), AuditTime = DateTime.Now},
                new Product { ProductId = 115, ProductName = "Branket 2", CategoryId = 2, UnitPrice = 62, StatusCode = 3, AvailableSince = Convert.ToDateTime("2023-08-22"), AuditTime = DateTime.Now},
                new Product { ProductId = 116, ProductName = "Mattress Protector 2", CategoryId = 2, UnitPrice = 31.4m, StatusCode = 2, AvailableSince = Convert.ToDateTime("2023-08-22"), AuditTime = DateTime.Now },
                new Product { ProductId = 117, ProductName = "Sheet Set 2", CategoryId = 2, UnitPrice = 43.79m, StatusCode = 1, AvailableSince = Convert.ToDateTime("2022-07-26"), AuditTime = DateTime.Now},
                new Product { ProductId = 118, ProductName = "Pillow 2", CategoryId = 2, UnitPrice = 16.2m, StatusCode = 1, AvailableSince = Convert.ToDateTime("2022-06-04"), AuditTime = DateTime.Now},
                new Product { ProductId = 119, ProductName = "Bed Curtain", CategoryId = 2, UnitPrice = 18.6m, StatusCode = 4, AvailableSince = Convert.ToDateTime("2022-07-11"), AuditTime = DateTime.Now},
                new Product { ProductId = 120, ProductName = "Bed Curtain 2", CategoryId = 2, UnitPrice = 19.8m, StatusCode = 1, AvailableSince = Convert.ToDateTime("2023-06-11"), AuditTime = DateTime.Now},
                new Product { ProductId = 125, ProductName = "Branket 3", CategoryId = 2, UnitPrice = 65, StatusCode = 1, AvailableSince = Convert.ToDateTime("2023-08-22"), AuditTime = DateTime.Now},
                new Product { ProductId = 126, ProductName = "Mattress Protector 3", CategoryId = 2, UnitPrice = 34.8m, StatusCode = 3, AvailableSince = Convert.ToDateTime("2023-08-22"), AuditTime = DateTime.Now },
                new Product { ProductId = 127, ProductName = "Sheet Set 3", CategoryId = 2, UnitPrice = 50.69m, StatusCode = 1, AvailableSince = Convert.ToDateTime("2023-07-26"), AuditTime = DateTime.Now},
                new Product { ProductId = 128, ProductName = "Pillow 3", CategoryId = 2, UnitPrice = 17.7m, StatusCode = 1, AvailableSince = Convert.ToDateTime("2023-06-04"), AuditTime = DateTime.Now},
                new Product { ProductId = 129, ProductName = "Bed Curtain 3", CategoryId = 2, UnitPrice = 21.5m, StatusCode = 2, AvailableSince = Convert.ToDateTime("2023-08-22"), AuditTime = DateTime.Now},
                  
                new Product { ProductId = 210, ProductName = "Baking Pan", CategoryId = 3, UnitPrice = 10.99m, StatusCode = 1, AvailableSince = Convert.ToDateTime("2022-10-26"), AuditTime = DateTime.Now},
                new Product { ProductId = 212, ProductName = "Can Opener", CategoryId = 3, UnitPrice = 7.99m, StatusCode = 4, AvailableSince = Convert.ToDateTime("2022-09-18"), AuditTime = DateTime.Now},
                new Product { ProductId = 213, ProductName = "Coffee Maker", CategoryId = 3, UnitPrice = 49.39m, StatusCode = 4, AvailableSince = null, AuditTime = DateTime.Now},
                new Product { ProductId = 214, ProductName = "Knife Set", CategoryId = 3, UnitPrice = 70, StatusCode = 1, AvailableSince = Convert.ToDateTime("2022-10-10"), AuditTime = DateTime.Now},
                new Product { ProductId = 215, ProductName = "Pressure Cooker", CategoryId = 3, UnitPrice = 90.5m, StatusCode = 2, AvailableSince = Convert.ToDateTime("2023-10-26"), AuditTime = DateTime.Now },
                new Product { ProductId = 216, ProductName = "Water Pitcher", CategoryId = 3, UnitPrice = 29.99m, StatusCode = 3, AvailableSince = null, AuditTime = DateTime.Now},
                new Product { ProductId = 218, ProductName = "Baking Pan 2", CategoryId = 3, UnitPrice = 12.99m, StatusCode = 1, AvailableSince = Convert.ToDateTime("2023-10-26"), AuditTime = DateTime.Now},
                new Product { ProductId = 219, ProductName = "Can Opener 2", CategoryId = 3, UnitPrice = 8.99m, StatusCode = 1, AvailableSince = Convert.ToDateTime("2022-09-18"), AuditTime = DateTime.Now},
                new Product { ProductId = 220, ProductName = "Coffee Maker 2", CategoryId = 3, UnitPrice = 50.39m, StatusCode = 2, AvailableSince = null, AuditTime = DateTime.Now},
                new Product { ProductId = 221, ProductName = "Knife Set 2", CategoryId = 3, UnitPrice = 76, StatusCode = 1, AvailableSince = Convert.ToDateTime("2022-10-10"), AuditTime = DateTime.Now},
                new Product { ProductId = 222, ProductName = "Pressure Cooker 2", CategoryId = 3, UnitPrice = 91.5m, StatusCode = 3, AvailableSince = Convert.ToDateTime("2022-10-26"), AuditTime = DateTime.Now },
                new Product { ProductId = 223, ProductName = "Water Pitcher 2", CategoryId = 3, UnitPrice = 39.99m, StatusCode = 1, AvailableSince = null, AuditTime = DateTime.Now},
                new Product { ProductId = 240, ProductName = "Baking Pan 3", CategoryId = 3, UnitPrice = 14.99m, StatusCode = 1, AvailableSince = Convert.ToDateTime("2023-10-26"), AuditTime = DateTime.Now},
                new Product { ProductId = 242, ProductName = "Can Opener 3", CategoryId = 3, UnitPrice = 9.99m, StatusCode = 5, AvailableSince = Convert.ToDateTime("2023-09-18"), AuditTime = DateTime.Now},
                new Product { ProductId = 243, ProductName = "Coffee Maker 3", CategoryId = 3, UnitPrice = 49.39m, StatusCode = 2, AvailableSince = null, AuditTime = DateTime.Now},
                new Product { ProductId = 244, ProductName = "Knife Set 3", CategoryId = 3, UnitPrice = 70, StatusCode = 1, AvailableSince = Convert.ToDateTime("2022-10-10"), AuditTime = DateTime.Now},
                new Product { ProductId = 245, ProductName = "Pressure Cooker 3", CategoryId = 3, UnitPrice = 90.5m, StatusCode = 1, AvailableSince = Convert.ToDateTime("2022-10-26"), AuditTime = DateTime.Now },
                new Product { ProductId = 246, ProductName = "Water Pitcher 3", CategoryId = 3, UnitPrice = 29.99m, StatusCode = 2, AvailableSince = null, AuditTime = DateTime.Now},
                new Product { ProductId = 248, ProductName = "Baking Pan 4", CategoryId = 3, UnitPrice = 10.99m, StatusCode = 1, AvailableSince = Convert.ToDateTime("2023-10-26"), AuditTime = DateTime.Now},
                new Product { ProductId = 249, ProductName = "Can Opener 4", CategoryId = 3, UnitPrice = 7.99m, StatusCode = 3, AvailableSince = Convert.ToDateTime("2022-09-18"), AuditTime = DateTime.Now},
                new Product { ProductId = 250, ProductName = "Coffee Maker 4", CategoryId = 3, UnitPrice = 49.39m, StatusCode = 4, AvailableSince = null, AuditTime = DateTime.Now},
                new Product { ProductId = 251, ProductName = "Knife Set 4", CategoryId = 3, UnitPrice = 70, StatusCode = 1, AvailableSince = Convert.ToDateTime("2023-10-10"), AuditTime = DateTime.Now},
                new Product { ProductId = 252, ProductName = "Pressure Cooker 4", CategoryId = 3, UnitPrice = 90.5m, StatusCode = 1, AvailableSince = Convert.ToDateTime("2022-10-26"), AuditTime = DateTime.Now },
                new Product { ProductId = 253, ProductName = "Water Pitcher 4", CategoryId = 3, UnitPrice = 29.99m, StatusCode = 5, AvailableSince = null, AuditTime = DateTime.Now}
            };
            context.Products.AddRange(products);
            
            var contacts = new Contact[]
            {
                new Contact { ContactName = "My Contact", Phone = "222-333-4444", Email = "demo@smstore.com", PrimaryType = 2, AuditTime = Convert.ToDateTime("2023-05-28")},
                new Contact { ContactName = "John Wood", Phone = "410-565-3879", Email = "johnwoo@retob.com", PrimaryType = 1, AuditTime = Convert.ToDateTime("2023-05-14")},
                new Contact { ContactName = "Dean Sapez", Phone = "647-596-2143", Email = "deans@fashep.com", PrimaryType = 2, AuditTime = Convert.ToDateTime("2023-06-27")}
            };
            context.Contacts.AddRange(contacts);
            context.SaveChanges();            

            //Memory db doesn't support stored procedures.
            if (StaticConfigs.GetConfig("UseInMemoryDatabase") != "true")
            {
                context.Database.ExecuteSqlRaw(
                   @"IF OBJECT_ID ( 'dbo.GetAllCategoriesAndProducts', 'P' ) IS NOT NULL 
    DROP PROCEDURE dbo.GetAllCategoriesAndProducts "
                 );

                context.Database.ExecuteSqlRaw(
                @"CREATE PROCEDURE dbo.GetAllCategoriesAndProducts 
AS 
BEGIN
SET NOCOUNT ON 
SELECT c.CategoryId,  
    c.CategoryName, 
    p.ProductCount
FROM dbo.Category c 
JOIN (SELECT count(ProductId) AS ProductCount, CategoryId
FROM Product			 
GROUP BY CategoryId) p
ON p.CategoryId = c.CategoryId 
SELECT p.ProductId, 
        p.ProductName, 
        p.CategoryId,
        c.CategoryName,
        p.UnitPrice,
        p.StatusCode, 
        s.Description AS StatusDescription,                          
        p.AvailableSince 
FROM dbo.Product p
join dbo.Category c on c.CategoryId = p.CategoryId 
join dbo.ProductStatusType s on s.StatusCode = p.StatusCode 
END "
                );

                context.Database.ExecuteSqlRaw(
                   @"IF OBJECT_ID ( 'dbo.GetProductCM', 'P' ) IS NOT NULL 
    DROP PROCEDURE dbo.GetProductCM "
                 );
                context.Database.ExecuteSqlRaw(
                   @"CREATE PROCEDURE dbo.GetProductCM 
AS 
BEGIN 
SET NOCOUNT ON 
SELECT p.ProductId, 
        p.ProductName, 
        p.CategoryId,
        c.CategoryName, 
        p.UnitPrice, 
        p.StatusCode, 
        s.Description AS StatusDescription,                          
        p.AvailableSince 
FROM dbo.Product p
join dbo.Category c on c.CategoryId = p.CategoryId 
join dbo.ProductStatusType s on s.StatusCode = p.StatusCode 
END "
                );

                context.Database.ExecuteSqlRaw(
                   @"IF OBJECT_ID ( 'dbo.GetPagedProductList', 'P' ) IS NOT NULL 
    DROP PROCEDURE dbo.GetPagedProductList; "
                 );
                context.Database.ExecuteSqlRaw(
                   @"CREATE PROCEDURE dbo.GetPagedProductList	 
	@FilterString varchar(max) = '',
	@SortString varchar(max) = '',	
    @PageNumber int = null,
	@PageSize int = null,
	@TotalCount int OUTPUT
AS
BEGIN
  /* 
	 '&dapos;': encode for dual single quotes for dynamic query. 
  */
  SET NOCOUNT ON;
	If @PageNumber is NULL OR @PageNumber = 0 SET @PageNumber = 1;
	If @PageSize is NULL OR @PageSize = 0 SET @PageSize = 2147483647; -- max int size by default
    
	DECLARE @ProductTable table 
	(
		RowNumber int identity(1, 1),
		ProductId int,
		ProductName varchar(50),
		CategoryId int,
		CategoryName varchar(50),
		UnitPrice decimal (18, 2),	
		StatusCode int,
		StatusDescription varchar(20),
		AvailableSince datetime2		
	)
	DECLARE @Query VARCHAR(MAX)
	SET @Query = '
	SELECT X.* FROM (
	  SELECT DISTINCT 
	     p.ProductId, 
         p.ProductName, 
         p.CategoryId,
         c.CategoryName, 
		 p.UnitPrice,
         p.StatusCode, 
         s.Description AS StatusDescription,          
         p.AvailableSince 
      FROM dbo.Product p
      INNER JOIN dbo.Category c on c.CategoryId = p.CategoryId 
      INNER JOIN dbo.ProductStatusType s on s.StatusCode = p.StatusCode 
	) AS X'
	IF @FilterString != ''
	BEGIN
	    --Replace '&dapos;' with dual single quotes.
		SET @FilterString = REPLACE(@FilterString, '&dapos;', '''')
		SET @Query = @Query + ' WHERE 1=1' + @FilterString
	END
	IF @SortString != ''
	SET @Query = @Query + ' ORDER BY ' + @SortString 
 
  INSERT INTO @ProductTable  
  EXEC (@Query)	
  
  select @TotalCount = count(*) from @ProductTable 
   
  --Output paged list.
  SELECT ProductId, 
         ProductName, 
         CategoryId,
         CategoryName,
         UnitPrice,
         StatusCode, 
         StatusDescription,
         AvailableSince
  FROM @ProductTable
  ORDER BY RowNumber
  OFFSET  @PageSize * (@PageNumber - 1) ROWS
  FETCH NEXT @PageSize ROWS ONLY;
 
  SET NOCOUNT OFF;     
END "
                );
            }
        }
    }    
}