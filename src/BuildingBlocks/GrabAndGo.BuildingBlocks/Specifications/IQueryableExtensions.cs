using System.Linq.Expressions;
using System.Reflection;

namespace GrabAndGo.BuildingBlocks.Specifications;

public static class IQueryableExtensions
{
    public static IQueryable<T> ApplySort<T>(this IQueryable<T> source, string? orderByQueryString)
    {
        if (string.IsNullOrWhiteSpace(orderByQueryString))
            return source;

        var orderParams = orderByQueryString.Trim().Split(',');
        var propertyInfos = typeof(T).GetProperties(BindingFlags.Public | BindingFlags.Instance);
        var first = true;

        foreach (var param in orderParams)
        {
            if (string.IsNullOrWhiteSpace(param)) continue;

            var propertyFromQueryName = param.Trim().Split(" ")[0];
            var objectProperty = propertyInfos.FirstOrDefault(pi => pi.Name.Equals(propertyFromQueryName, StringComparison.InvariantCultureIgnoreCase));

            if (objectProperty == null) continue;

            var direction = param.EndsWith(" desc", StringComparison.InvariantCultureIgnoreCase) ? "Descending" : "Ascending";
            var methodName = first ? (direction == "Ascending" ? "OrderBy" : "OrderByDescending") : (direction == "Ascending" ? "ThenBy" : "ThenByDescending");

            var parameter = Expression.Parameter(typeof(T), "x");
            var property = Expression.Property(parameter, objectProperty);
            var lambda = Expression.Lambda(property, parameter);

            source = (IQueryable<T>)typeof(Queryable).GetMethods()
                .Single(method => method.Name == methodName && method.IsGenericMethodDefinition && method.GetGenericArguments().Length == 2 && method.GetParameters().Length == 2)
                .MakeGenericMethod(typeof(T), objectProperty.PropertyType)
                .Invoke(null, new object[] { source, lambda })!;

            first = false;
        }

        return source;
    }

    public static IQueryable<T> ApplyFilters<T>(this IQueryable<T> source, Dictionary<string, string> filters)
    {
        if (filters == null || !filters.Any())
            return source;

        foreach (var filter in filters)
        {
            var propertyInfo = typeof(T).GetProperties().FirstOrDefault(p => p.Name.Equals(filter.Key, StringComparison.InvariantCultureIgnoreCase));
            if (propertyInfo == null || string.IsNullOrWhiteSpace(filter.Value)) continue;

            var parameter = Expression.Parameter(typeof(T), "x");
            var property = Expression.Property(parameter, propertyInfo);
            
            Expression comparison;
            
            try 
            {
                if (propertyInfo.PropertyType == typeof(string))
                {
                    var containsMethod = typeof(string).GetMethod("Contains", new[] { typeof(string) });
                    comparison = Expression.Call(property, containsMethod!, Expression.Constant(filter.Value));
                }
                else if (propertyInfo.PropertyType == typeof(Guid) || propertyInfo.PropertyType == typeof(Guid?))
                {
                    comparison = Expression.Equal(property, Expression.Constant(Guid.Parse(filter.Value), propertyInfo.PropertyType));
                }
                else if (propertyInfo.PropertyType == typeof(bool) || propertyInfo.PropertyType == typeof(bool?))
                {
                    comparison = Expression.Equal(property, Expression.Constant(bool.Parse(filter.Value), propertyInfo.PropertyType));
                }
                else if (propertyInfo.PropertyType == typeof(int) || propertyInfo.PropertyType == typeof(int?))
                {
                    comparison = Expression.Equal(property, Expression.Constant(int.Parse(filter.Value), propertyInfo.PropertyType));
                }
                else
                {
                    continue;
                }

                var lambda = Expression.Lambda<Func<T, bool>>(comparison, parameter);
                source = source.Where(lambda);
            }
            catch
            {
                // Skip filter if parsing fails
                continue;
            }
        }

        return source;
    }

    public static IQueryable<T> Search<T>(this IQueryable<T> source, string? searchTerm)
    {
        if (string.IsNullOrWhiteSpace(searchTerm))
            return source;

        var propertyInfo = typeof(T).GetProperties().FirstOrDefault(p => p.Name.Equals("Name", StringComparison.InvariantCultureIgnoreCase));
        if (propertyInfo == null || propertyInfo.PropertyType != typeof(string))
            return source;

        var parameter = Expression.Parameter(typeof(T), "x");
        var property = Expression.Property(parameter, propertyInfo);
        var containsMethod = typeof(string).GetMethod("Contains", new[] { typeof(string) });
        var comparison = Expression.Call(property, containsMethod!, Expression.Constant(searchTerm));
        
        var lambda = Expression.Lambda<Func<T, bool>>(comparison, parameter);
        return source.Where(lambda);
    }
}
