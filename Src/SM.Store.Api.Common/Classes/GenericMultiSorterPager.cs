using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using SM.Store.Api.Contracts.Models;

namespace SM.Store.Api.Common
{
    public static class GenericMultiSorterPager
    {
        public static IList<T> GetSortedPagedList<T>(IQueryable<T> source, PaginationRequest2 paging, out int totalCount, ChildLoad childLoad = ChildLoad.None)
        {
            totalCount = 0;
            //If not need paging, pass the null PaginationRequest
            if (paging == null)
            {
                var list = source.ToList();
                totalCount = list.Count();
                return list;
            }            
            source = source.Distinct();
            
            //Call to build sorted paged query
            IQueryable<T> sortedPagedQuery = GetSortedPagedQuerable<T>(source, paging);

            //Call db one time to get data rows and count together
            //Core 3.0: No childLoad.Include is used.
            if (childLoad == ChildLoad.None)
            {
                //Build one-call query from created regular query
                var pagedGroup = from sel in sortedPagedQuery
                                 select new PagedResultSet<T>()
                                 {
                                     PagedData = sel,
                                     TotalCount = source.Count()
                                 };
                //Get the complete resultset from db.
                List<PagedResultSet<T>> pagedResultSet;
                try
                {
                    pagedResultSet = pagedGroup.AsParallel().ToList();
                }
                catch (NotSupportedException)
                {
                    //In case not supported with EF version, do two calls instead
                    totalCount = source.Count();
                    return sortedPagedQuery.ToList();
                }

                //Get data and total count from the resultset 
                IEnumerable<T> pagedList = new List<T>();
                if (pagedResultSet.Count() > 0)
                {
                    totalCount = pagedResultSet.First().TotalCount;
                    pagedList = pagedResultSet.Select(s => s.PagedData);
                }
                //Remove the wrapper reference 
                pagedResultSet = null;

                return pagedList.ToList();
            }
            //Call db twice when childLoad == Include or else
            //Core 3.0: this code is not used.
            else
            {
                totalCount = source.Count();
                return sortedPagedQuery.ToList();
            }
        }

        private static IQueryable<T> GetSortedPagedQuerable<T>(IQueryable<T> source, PaginationRequest2 paging)
        {
            IQueryable<T> pagedQuery = source;

            var tempPagedQuery = AsSortedQueryable<T>(source, paging.SortList);
            if (tempPagedQuery != null)
            {
                pagedQuery = tempPagedQuery;
            }
            
            //Construct paged query
            if (paging.PageSize > 0)
            {
                pagedQuery = AsPagedQueryable<T>(pagedQuery, paging.PageIndex, paging.PageSize);
            }
            else
            {
                //Passing PageSize 0 to get all rows but using sorting.
                paging.PageIndex = 0;
            }
            //Return sorted paged query
            return pagedQuery;
        }

        public static IOrderedQueryable<T> AsSortedQueryable<T>(this IQueryable<T> source, List<SortItem> sortList)
        {
            if (sortList.Count == 0)
            {
                return null;
            }
            //Create parameter expression node.
            var param = Expression.Parameter(typeof(T), string.Empty);
            
            //Create member expression for accessing properties for first sorted column.
            var property = Expression.PropertyOrField(param, sortList[0].SortBy);

            //Create lambda expression as delegation type for first sorted column.
            var sort = Expression.Lambda(property, param);

            //Call to create sorting expression for the first sorted column.
            MethodCallExpression orderByCall = Expression.Call(
                typeof(Queryable),
                "OrderBy" + (sortList[0].SortDirection == "desc" ? "Descending" : string.Empty),
                new[] { typeof(T), property.Type },
                source.Expression,
                Expression.Quote(sort));

            //Call to create multiple column sorting expressions if more than one sorted column passed.
            if (sortList.Count > 1)
            {
                for (int idx = 1; idx < sortList.Count; idx++)
                {
                    var item = sortList[idx].SortBy;

                    //The same logic used as for the first column sorting.
                    param = Expression.Parameter(typeof(T), string.Empty);
                    property = Expression.PropertyOrField(param, item);
                    sort = Expression.Lambda(property, param);

                    //Use "ThenBy" for more than one column sorting. 
                    orderByCall = Expression.Call(
                        typeof(Queryable),
                        "ThenBy" + (sortList[idx].SortDirection == "desc" ? "Descending" : string.Empty),
                        new[] { typeof(T), property.Type },
                        orderByCall,
                        Expression.Quote(sort));
                }
            }
            //Generate and return LINQ of IOrderedQueryable type.
            return (IOrderedQueryable<T>)source.Provider.CreateQuery<T>(orderByCall);
        }

        public static IQueryable<T> AsPagedQueryable<T>(IQueryable<T> source, int pageIndex, int pageSize)
        {
            return source.Skip(pageIndex * pageSize).Take(pageSize);
        }
    }
}