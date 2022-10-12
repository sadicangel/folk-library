using FolkLibrary.Models;
using Humanizer;
using Microsoft.EntityFrameworkCore;
using OfficeOpenXml;
using OfficeOpenXml.Style;
using System.Data;
using System.Reflection;

namespace FolkLibrary.Services;

internal sealed class FolkDataExporter
{
    private static IReadOnlyDictionary<PropertyInfo, Func<Artist, object>> ArtistGetters { get; } = FolkExtensions.CreatePropertyGetters<Artist>();
    private static IReadOnlyDictionary<PropertyInfo, Func<Album, object>> AlbumGetters { get; } = FolkExtensions.CreatePropertyGetters<Album>();
	private readonly FolkDbContext _dbContext;

    public FolkDataExporter(FolkDbContext dbContext)
	{
		_dbContext = dbContext;

    }

	public void WriteXlsx(string fileName, bool overwrite = false)
    {
        if (File.Exists(fileName))
        {
            if (!overwrite)
                throw new InvalidOperationException($"File '{fileName}' alread exists");
            File.Delete(fileName);
        }

        var artists = _dbContext.Artists.Include(a => a.Albums).OrderBy(a => a.Year).ToList();

        ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
        using var package = new ExcelPackage(new FileInfo(fileName));

        CreateStyles(package.Workbook);

        var artistsSheet = package.Workbook.Worksheets.Add(nameof(Artist).Pluralize());
        int row = 1, col = 1;
        foreach (var property in ArtistGetters.Keys)
            SetValue(artistsSheet.Cells[row, col++], property.Name.Humanize(), "header");
        foreach (var artist in artists)
        {
            ++row;
            col = 1;
            foreach (var property in ArtistGetters.Keys)
                SetValue(artistsSheet.Cells[row, col++], GetValue(property, ArtistGetters[property], artist));
        }
        FormatColumns(artistsSheet, ArtistGetters.Keys, firstRow: 2, firstColumn: 1);
        artistsSheet.Cells[artistsSheet.Dimension.Address].AutoFitColumns();

        var albumSheet = package.Workbook.Worksheets.Add(nameof(Album).Pluralize());
        row = 1; col = 1;
        foreach (var property in AlbumGetters.Keys)
            SetValue(albumSheet.Cells[row, col++], property.Name.Humanize(), "header");
        row = 2;
        foreach(var artist in artists)
        {
            SetValue(albumSheet.Cells[row, 1], $"{artist.ShortName} ({artist.Name})", "row-header");
            albumSheet.Cells[row, 1, row, AlbumGetters.Count].Merge = true;
            row++;
            foreach (var album in artist.Albums.OrderBy(e => e.Year).ThenBy(e => e.Name, NaturalSortComparer.Ordinal))
            {
                col = 1;
                foreach (var property in AlbumGetters.Keys)
                    SetValue(albumSheet.Cells[row, col++], GetValue(property, AlbumGetters[property], album));
                row++;
            }
        }
        FormatColumns(albumSheet, AlbumGetters.Keys, firstRow: 2, firstColumn: 1);
        albumSheet.Cells[albumSheet.Dimension.Address].AutoFitColumns();

        package.Save();
    }

    private static void FormatColumns(ExcelWorksheet sheet, IEnumerable<PropertyInfo> properties, int firstRow, int firstColumn)
    {
        var col = firstColumn;
        foreach (var property in properties)
        {
            var type = Nullable.GetUnderlyingType(property.PropertyType) ?? property.PropertyType;
            switch (type)
            {
                case Type when type == typeof(TimeSpan):
                    sheet.Column(col).Style.Numberformat.Format = "hh:mm:ss";
                    break;
                case Type when type == typeof(int):
                    sheet.Column(col).Style.Numberformat.Format = "0";
                    break;
                case Type when type == typeof(bool):
                    var value = property.Name[2..].Humanize();
                    for (int i = firstRow; i <= sheet.Dimension.Rows; ++i)
                    {
                        var cell = sheet.Cells[i, col];
                        var current = cell.GetValue<bool?>();
                        if (current is true)
                            SetValue(cell, value, "bool-cell");
                        else if (current is false)
                            cell.Value = null;
                    }
                    break;
            }
            col++;
        }
    }

    private static void CreateStyles(ExcelWorkbook workbook)
    {
        var header = workbook.Styles.CreateNamedStyle("header").Style;
        header.Font.Bold = true;
        header.Fill.PatternType = ExcelFillStyle.Solid;
        header.Fill.BackgroundColor.SetColor(OfficeOpenXml.Drawing.eThemeSchemeColor.Accent6);

        var rowHeader = workbook.Styles.CreateNamedStyle("row-header").Style;
        rowHeader.Font.Bold = true;
        rowHeader.Fill.PatternType = ExcelFillStyle.Solid;
        rowHeader.Fill.BackgroundColor.SetColor(OfficeOpenXml.Drawing.eThemeSchemeColor.Background2);

        var boolCell = workbook.Styles.CreateNamedStyle("bool-cell").Style;
        boolCell.Font.Italic = true;
    }

    private static void SetValue(ExcelRange cell, object? value, string? styleName = null)
    {
        cell.Value = value;
        if (!String.IsNullOrWhiteSpace(styleName))
            cell.StyleName = styleName;
    }

    private static object? GetValue<T>(PropertyInfo property, Func<T, object> getter, T @object)
    {
        var propertyType = Nullable.GetUnderlyingType(property.PropertyType) ?? property.PropertyType;
        var value = getter.Invoke(@object);
        return value is null ? value : Convert.ChangeType(value, propertyType);
    }

    /// <summary>
    /// A comparer of strings that preserves the order of numerical substrings.
    /// For example, <c>File2</c> comes before <c>File10</c>, which is the opposite of
    /// usual lexicographical ordering.
    /// </summary>
    /// <remarks>
    /// <see langword="null"/> values are sorted to the start of the collection.
    /// The comparison is case-sensitive.
    /// </remarks>
    private sealed class NaturalSortComparer : IComparer<string?>
    {
        /// <summary>
        /// Gets a singleton instance of this comparer for which non-integer characters are compared ordinally.
        /// </summary>
        public static NaturalSortComparer Ordinal { get; } = new();

        /// <inheritdoc />
        public int Compare(string? x, string? y)
        {
            // sort nulls to the start
            if (x == null)
                return y == null ? 0 : -1;
            if (y == null)
                return 1;

            var ix = 0;
            var iy = 0;

            while (true)
            {
                // sort shorter strings to the start
                if (ix >= x.Length)
                    return iy >= y.Length ? 0 : -1;
                if (iy >= y.Length)
                    return 1;

                var cx = x[ix];
                var cy = y[iy];

                int result;
                if (char.IsDigit(cx) && char.IsDigit(cy))
                    result = CompareInteger(x, y, ref ix, ref iy);
                else
                    result = cx.CompareTo(y[iy]);

                if (result != 0)
                    return result;

                ix++;
                iy++;
            }

            static int CompareInteger(string x, string y, ref int ix, ref int iy)
            {
                var lx = GetNumLength(x, ix);
                var ly = GetNumLength(y, iy);

                // shorter number first (note, doesn't handle leading zeroes)
                if (lx != ly)
                    return lx.CompareTo(ly);

                for (var i = 0; i < lx; i++)
                {
                    var result = x[ix++].CompareTo(y[iy++]);
                    if (result != 0)
                        return result;
                }

                return 0;

                static int GetNumLength(string s, int i)
                {
                    var length = 0;
                    while (i < s.Length && char.IsDigit(s[i++]))
                        length++;
                    return length;
                }
            }
        }
    }
}
