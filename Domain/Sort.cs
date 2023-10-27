using System.Diagnostics.CodeAnalysis;

namespace FolkLibrary;

public enum SortDirection
{
    ASC,
    DESC
}

public readonly record struct Sort(string PropertyName, SortDirection SortDirection = SortDirection.ASC) : IParsable<Sort>
{
    public override string ToString() => $"{PropertyName} {SortDirection}";

    public static Sort Parse(string s, IFormatProvider? provider)
    {
        ArgumentNullException.ThrowIfNull(s);

        var parts = s.Split(' ', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);

        var propertyName = parts[0];
        var sortDirection = parts.Length >= 2 && Enum.TryParse<SortDirection>(parts[1], ignoreCase: true, out var order)
            ? order
            : SortDirection.ASC;

        return new Sort(propertyName, sortDirection);
    }

    public static bool TryParse([NotNullWhen(true)] string? s, IFormatProvider? provider, [MaybeNullWhen(false)] out Sort result)
    {
        result = default;

        if (String.IsNullOrEmpty(s))
            return false;

        var parts = s.Split(' ', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);

        var propertyName = parts[0];
        var sortDirection = parts.Length >= 2 && Enum.TryParse<SortDirection>(parts[1], ignoreCase: true, out var order)
            ? order
            : SortDirection.ASC;

        result = new Sort(propertyName, sortDirection);
        return true;
    }
}
