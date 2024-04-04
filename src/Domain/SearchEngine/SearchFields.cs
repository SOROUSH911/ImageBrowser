using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImageBrowser.Domain.SearchEngine;
public class SearchFields
{
    public List<string> Ids { get; set; }
    public List<string> TextContents { get; set; }
    public List<string> Categories { get; set; }

    public List<string> Names { get; set; }
    public List<string> Paths { get; set; }
    public List<long> Sizes { get; set; }
    public List<int> OwnerIds { get; set; }

public SearchFields()
    {
        Ids = new List<string>();
        TextContents = new List<string>();
        Categories = new List<string>();    
        Names = new List<string>();
        Paths = new List<string>();
        Sizes = new List<long>();   
        OwnerIds = new List<int>();
    }

    public override string ToString()
    {
        var fields = new List<string>
         {
             GetListString(IBDataSchema.IdField, Ids),
             GetListString(IBDataSchema.catField, Categories),
             GetListString(IBDataSchema.textContentField, TextContents),
             GetListString(IBDataSchema.nameField, Names),
             GetListString(IBDataSchema.pathField, Paths),
             GetListString(IBDataSchema.sizeField, Sizes),
             GetListString(IBDataSchema.ownerIdField, OwnerIds),
         };

        var notEmpties = fields.Where(f => !string.IsNullOrEmpty(f)).ToList();
        return string.Join(" ", notEmpties);
    }

    private string GetListString<T>(string key, List<T> list)
    {
        if (list == null || list.Count == 0)
        {
            return "";
        }

        var strList = list.Distinct().Select(i => typeof(T) == typeof(string) ? $"\"{i}\"" : i.ToString()).ToList();

        return $"{key}:({string.Join(" ", strList)})";
    }
}
