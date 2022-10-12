using Humanizer;
using System.Reflection;
using System.Web;

namespace FolkLibrary.Interfaces;

internal interface IQueryParams<T>
{
    private static IDictionary<PropertyInfo, Func<T, object>> Getters { get; } = FolkExtensions.CreatePropertyGetters<T>();

    public string ToQueryParams()
    {
        var @this = (T)this;
        var query = HttpUtility.ParseQueryString("");
        foreach (var (property, getter) in Getters)
        {
            var value = getter.Invoke(@this).ToString();
            if (value is not null)
                query[property.Name.Camelize()] = value;
        }
        return query.ToString()!;
    }

}

internal static class QueryParamsExtensions
{
    public static string ToQueryParams<T>(this IQueryParams<T> @this) => @this.ToQueryParams();
}
