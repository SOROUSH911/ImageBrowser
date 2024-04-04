using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SolrNet;

namespace ImageBrowser.Domain.SearchEngine;
 
public class SearchResponse
{

    //Expose properties that will be returned to from the search library
    public List<IBDataSchema> Results { get; set; }
    public Dictionary<string, List<IBDataSchema>> GroupResults { get; set; }
    public List<Pivot> PivotResults { get; set; }

    public int TotalHits { get; set; }
    public int QueryTime { get; set; }
    public int Status { get; set; }
    public SearchQuery OriginalQuery { get; set; }

    public List<string> DidYouMean { get; set; }
    public List<KeyValuePair<string, int>> TopicsFacet { get; set; }
    public List<KeyValuePair<string, int>> FieldValueFacet { get; set; }
    public List<KeyValuePair<string, int>> CollectionDatesFacets { get; set; }
    public List<KeyValuePair<string, int>> GeoFacets { get; set; }
    public List<KeyValuePair<string, int>> DataTypesFacets { get; set; }

    public string Error { get; set; }

}
