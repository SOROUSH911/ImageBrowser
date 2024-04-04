using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImageBrowser.Domain.SearchEngine;
public class SearchQuery
{
    public SearchQuery()
    {
        Query = "*:*";
        Start = 0;
        Rows = 10;
        FilterFields = new SearchFields();
        FacetFields = new SearchFields();
        BoostFields = new SearchFields();
        Grouping = new SearchGroups();
        Pivots = new SearchPivots();
    }

    public string Query { get; set; }
    public int Start { get; set; }
    public int Rows { get; set; }
    public string Sort { get; set; }
    public bool IsAscending { get; set; }
    public SearchFields FilterFields { get; set; }
    public SearchFields FacetFields { get; set; }
    public SearchFields BoostFields { get; set; }
    public SearchGroups Grouping { get; set; }
    public SearchPivots Pivots { get; set; }


}
