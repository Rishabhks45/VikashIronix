using System.Linq;

namespace SharedKernel.DTOs;

public class GridRequestDto
{
    public int Page { get; set; }
    public int PageSize { get; set; }
    public string SortColumn { get; set; } = "CreatedAt";
    public bool SortDescending { get; set; }
    public string Filter { get; set; } = string.Empty;

    public string GetSortColumnForDatabase()
    {
        if (!string.IsNullOrEmpty(SortColumn))
        {
            SortColumn = SortColumn == "FullName" ? "FirstName" : SortColumn;
            return string.Concat(SortColumn.Select((x, i) => i > 0 && char.IsUpper(x) ? "_" + x : x.ToString())).ToLower();
        }
        else
        {
            return SortColumn;
        }
    }
}