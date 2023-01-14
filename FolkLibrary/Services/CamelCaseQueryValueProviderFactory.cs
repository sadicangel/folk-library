using Humanizer;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using System.Globalization;

namespace FolkLibrary.Services;

public sealed class CamelCaseQueryValueProviderFactory : IValueProviderFactory
{
    public Task CreateValueProviderAsync(ValueProviderFactoryContext context)
    {
        if (context is null)
            throw new ArgumentNullException(nameof(context));

        var valueProvider = new ValueProvider(BindingSource.Query, context.ActionContext.HttpContext.Request.Query, CultureInfo.CurrentCulture);

        context.ValueProviders.Add(valueProvider);

        return Task.CompletedTask;
    }

    public sealed class ValueProvider : QueryStringValueProvider, IValueProvider
    {
        public ValueProvider(BindingSource bindingSource, IQueryCollection values, CultureInfo? culture)
            : base(bindingSource, values, culture)
        {
        }

        public override bool ContainsPrefix(string prefix) => base.ContainsPrefix(prefix.Camelize());

        public override ValueProviderResult GetValue(string key) => base.GetValue(key.Camelize());
    }
}
