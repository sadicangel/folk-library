using System.Reflection.Emit;
using System.Reflection;
using FolkLibrary.Interfaces;

namespace FolkLibrary;
public static class FolkExtensions
{
    public static IEnumerable<Type> GetAllIdTypes()
    {
        return typeof(IId<>).Assembly.GetExportedTypes().Where(t => t.IsValueType && t.IsAssignableTo(typeof(IId<>).MakeGenericType(t)));
    }

    public static Dictionary<PropertyInfo, Func<T, object>> CreatePropertyGetters<T>()
    {
        var getMethods = new Dictionary<PropertyInfo, Func<T, object>>();
        foreach (var property in GetProperties().Where(FilterProperty))
        {
            Func<T, object> getter;
            if (property.PropertyType.IsValueType)
            {
                var dynMethod = new DynamicMethod(string.Format("Dynamic_Get_{0}_{1}", typeof(T).Name, property.Name), typeof(object), new[] { typeof(T) }, typeof(T).Module);
                var ilGen = dynMethod.GetILGenerator();
                ilGen.Emit(OpCodes.Ldarg_0);
                ilGen.Emit(OpCodes.Callvirt, property.GetGetMethod()!);
                ilGen.Emit(OpCodes.Box, property.PropertyType);
                ilGen.Emit(OpCodes.Ret);

                getter = (Func<T, object>)dynMethod.CreateDelegate(typeof(Func<T, object>));
            }
            else
            {
                getter = (Func<T, object>)Delegate.CreateDelegate(typeof(Func<T, object>), null, property.GetGetMethod()!);
            }

            getMethods[property] = getter;
        }

        return getMethods;

        static IEnumerable<PropertyInfo> GetProperties()
        {
            var type = typeof(T);
            var list = new List<Type>();
            while (type is not null)
            {
                list.Add(type);
                type = type.BaseType;
            }
            list.Reverse();
            return list.SelectMany(t => t.GetProperties(BindingFlags.Instance | BindingFlags.Public | BindingFlags.DeclaredOnly));
        }

        static bool FilterProperty(PropertyInfo property)
        {
            return property.CanRead
                && (property.PropertyType.IsValueType || !property.PropertyType.IsGenericType)
                && property.Name != nameof(IIdObject.Id);
        }
    }
}
