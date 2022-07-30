using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using Atles.Models;

namespace Atles.Data.Extensions
{
    /// <summary>
    /// https://stackoverflow.com/questions/34906437/how-to-construct-order-by-expression-dynamically-in-entity-framework
    /// </summary>
    public static class QueryExtensions
    {
        private static readonly MethodInfo OrderByMethod =
            typeof(Queryable).GetMethods().Single(method =>
           method.Name == "OrderBy" && method.GetParameters().Length == 2);

        private static readonly MethodInfo OrderByDescendingMethod =
            typeof(Queryable).GetMethods().Single(method =>
           method.Name == "OrderByDescending" && method.GetParameters().Length == 2);

        private static bool PropertyExists<T>(this IQueryable<T> source, string propertyName)
        {
            return typeof(T).GetProperty(propertyName, 
                       BindingFlags.IgnoreCase |  
                       BindingFlags.Public | 
                       BindingFlags.Instance) != null;
        }

        private static IQueryable<T> OrderByProperty<T>(this IQueryable<T> source, string propertyName)
        {
            if (typeof(T).GetProperty(propertyName, 
                    BindingFlags.IgnoreCase | 
                    BindingFlags.Public | 
                    BindingFlags.Instance) == null)
            {
                return null;
            }

            ParameterExpression parameterExpression = Expression.Parameter(typeof(T));
            Expression orderByProperty = Expression.Property(parameterExpression, propertyName);
            LambdaExpression lambda = Expression.Lambda(orderByProperty, parameterExpression);
            MethodInfo genericMethod = OrderByMethod.MakeGenericMethod(typeof(T), orderByProperty.Type);
            object ret = genericMethod.Invoke(null, new object[] { source, lambda });
            return (IQueryable<T>)ret;
        }

        private static IQueryable<T> OrderByPropertyDescending<T>(this IQueryable<T> source, string propertyName)
        {
            if (typeof(T).GetProperty(propertyName, 
                    BindingFlags.IgnoreCase | 
                    BindingFlags.Public | 
                    BindingFlags.Instance) == null)
            {
                return null;
            }

            ParameterExpression parameterExpression = Expression.Parameter(typeof(T));
            Expression orderByProperty = Expression.Property(parameterExpression, propertyName);
            LambdaExpression lambda = Expression.Lambda(orderByProperty, parameterExpression);
            MethodInfo genericMethod = OrderByDescendingMethod.MakeGenericMethod(typeof(T), orderByProperty.Type);
            object ret = genericMethod.Invoke(null, new object[] { source, lambda });
            return (IQueryable<T>)ret;
        }

        public static IQueryable<T> OrderBy<T>(this IQueryable<T> source, QueryOptions options)
        {
            var propertyName = options.OrderByField;
            var direction = options.OrderByDirection;

            if (!string.IsNullOrWhiteSpace(propertyName) && direction != null && source.PropertyExists(propertyName))
            {
                source = direction == OrderByDirectionType.Asc
                    ? source.OrderByProperty(propertyName)
                    : source.OrderByPropertyDescending(propertyName);
            };

            return source;
        }
    }
}
