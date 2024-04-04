using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImageBrowser.Domain.SearchEngine;
public class SearchGroups
{
    public SearchGroups()
    {
        Fields = new List<string>();
    }
    public List<string> Fields { get; set; }
    public int Limit { get; set; }
}
