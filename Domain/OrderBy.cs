using System.Diagnostics.CodeAnalysis;

namespace FolkLibrary;

public enum OrderDirection
{
    ASC,
    DESC
}

public readonly record struct OrderBy(string PropertyName, OrderDirection Direction = OrderDirection.ASC) : IParsable<OrderBy>
{
    public override string ToString() => $"{PropertyName} {Direction}";

    public static OrderBy Parse(string s, IFormatProvider? provider)
    {
        ArgumentNullException.ThrowIfNull(s);

        var parts = s.Split(' ', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);

        var propertyName = parts[0];
        var sortDirection = parts.Length >= 2 && Enum.TryParse<OrderDirection>(parts[1], ignoreCase: true, out var order)
            ? order
            : OrderDirection.ASC;

        return new OrderBy(propertyName, sortDirection);
    }

    public static bool TryParse([NotNullWhen(true)] string? s, IFormatProvider? provider, [MaybeNullWhen(false)] out OrderBy result)
    {
        result = default;

        if (String.IsNullOrEmpty(s))
            return false;

        var parts = s.Split(' ', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);

        var propertyName = parts[0];
        var sortDirection = parts.Length >= 2 && Enum.TryParse<OrderDirection>(parts[1], ignoreCase: true, out var order)
            ? order
            : OrderDirection.ASC;

        result = new OrderBy(propertyName, sortDirection);
        return true;
    }
}
