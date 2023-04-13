namespace FolkLibrary;

public static class EntityExtensions
{
    public static string GetYearString(this Entity entity)
    {
        if (!entity.Year.HasValue)
            return "";

        if (entity.IsYearUncertain)
            return FormattableString.Invariant($"{entity.Year}?");

        return FormattableString.Invariant($"{entity.Year}");
    }
}
