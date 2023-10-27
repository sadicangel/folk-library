using DotNext;
using System.Text;

namespace FolkLibrary;

internal struct QueryBuilder
{
    private List<string>? _wheres;
    private List<object>? _params;
    private List<string>? _sorts;

    private List<string> Wheres { get => _wheres ??= new List<string>(); }

    private List<object> Params { get => _params ??= new List<object>(); }

    private List<string> OrderBys { get => _sorts ??= new List<string>(); }

    public QueryBuilder Where(string? value, string name, string condition)
    {
        if (!string.IsNullOrEmpty(value))
        {
            Wheres.Add($"data ->> '{name}' {condition} ?");
            if ("ilike".Equals(condition) || "like".Equals(condition))
                value = $"%{value}%";
            Params.Add(value);
        }
        return this;
    }

    public QueryBuilder Where(string? value, string name1, string name2, string condition)
    {
        if (!string.IsNullOrEmpty(value))
        {
            Wheres.Add($"data -> '{name1}' ->> '{name2}' {condition} ?");
            if ("ilike".Equals(condition) || "like".Equals(condition))
                value = $"%{value}%";
            Params.Add(value);
        }
        return this;
    }

    public QueryBuilder Where(int? value, string name, string condition)
    {
        if (value is not null)
        {
            Wheres.Add($"(data -> '{name}')::int {condition} ?");
            Params.Add(value);
        }
        return this;
    }

    public QueryBuilder Where(TimeSpan? value, string name, string condition)
    {
        if (value is not null)
        {
            Wheres.Add($"(data ->> '{name}')::time {condition} ?");
            Params.Add(value);
        }
        return this;
    }

    public QueryBuilder OrderBy(OrderBy? orderBy)
    {
        if (orderBy is not null)
        {
            OrderBys.Add($" order by data -> '{orderBy.Value.PropertyName}' {orderBy.Value.Direction}");
        }
        return this;
    }

    public readonly Optional<(string Where, object[] Params)> Build()
    {
        var builder = default(StringBuilder);
        if (_wheres is not null)
        {
            builder ??= new StringBuilder();
            builder.Append($" where {_wheres[0]}");
            foreach (var where in _wheres.Skip(1))
                builder.Append($" and {where}");
        }
        if (_sorts is not null)
        {
            builder ??= new StringBuilder();
            foreach (var sort in _sorts)
                builder.Append(sort);
        }

        return builder is { Length: > 0 }
            ? (builder.ToString(), _params?.ToArray() ?? Array.Empty<string>())
            : Optional<(string, object[])>.None;

    }
}