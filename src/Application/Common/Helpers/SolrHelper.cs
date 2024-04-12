using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImageBrowser.Application.Common.Helpers;
public static class SolrHelper
{
    public static string GetEntitySolrId(int id)
    {
     
      
            return "file_" + id;
      
    }

    public static int GetEntityId(string Id)
    {
        return int.Parse(Id.Split("_", StringSplitOptions.None)[1]);
    }
}
