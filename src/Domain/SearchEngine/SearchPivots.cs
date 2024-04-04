using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImageBrowser.Domain.SearchEngine;
public class SearchPivots
{
    public SearchPivots()
    {
        Fields = new List<PivotFieldsQuery>();
    }
    public List<PivotFieldsQuery> Fields { get; set; }
    public int Limit { get; set; }
}
