using System.Diagnostics.CodeAnalysis;

namespace FolkLibrary.Infrastructure;

internal readonly struct Param<T> : IParsable<Param<T>> where T : struct, IParsable<T>
{
    private readonly bool _hasValue;
    public static readonly Param<T> None = new();

    public Param(T value)
    {
        _hasValue = true;
        Value = value;
    }

    public T Value { get; }

    public bool IsEmpty => !_hasValue;

    public static Param<T> Parse(string s, IFormatProvider? provider) =>
        String.IsNullOrEmpty(s) ? Param<T>.None : new Param<T>(T.Parse(s, provider));

    public static bool TryParse([NotNullWhen(true)] string? s, IFormatProvider? provider, [MaybeNullWhen(false)] out Param<T> result)
    {
        result = Param<T>.None;
        if (!String.IsNullOrEmpty(s))
        {
            if (!T.TryParse(s, provider, out var value))
                return false;
            result = value;
        }
        return true;
    }

    public static implicit operator T?(Param<T> value) => value.IsEmpty ? null : value.Value;
    public static implicit operator Param<T>(T? value) => value is not null ? new(value.Value) : Param<T>.None;
}
