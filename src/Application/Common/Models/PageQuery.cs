using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImageBrowser.Application.Common.Models;
public class PageQuery
{
    public string Lang { get; set; }
    public int PageNumber { get; set; }
    public int PageSize { get; set; }
    public string SortField { get; set; }
    public int DataProfileSort { get; set; }
    public string Search { get; set; }
    public bool? IsIndex { get; set; }
    public bool IsExport { get; set; }
    public List<FilterGroup> FilterGroups { get; set; }
    public List<FilterGroup> FilterFacets { get; set; }

    public PageQuery()
    {
        FilterGroups = new List<FilterGroup>();
        FilterFacets = new List<FilterGroup>();
    }
}
