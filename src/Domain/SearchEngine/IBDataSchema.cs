using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata.Ecma335;
using System.Text;
using System.Threading.Tasks;
using SolrNet.Attributes;

namespace ImageBrowser.Domain.SearchEngine;
 public class IBDataSchema
{

    public const string IdField = "id";
    public const string textContentField = "text_content";
    public const string catField = "cat";
    public const string nameField = "name";
    public const string sizeField = "size";
    public const string pathField = "path";
    public const string ownerIdField = "owner_id";





    [SolrUniqueKey("id")]
    public string Id { get; set; }

    [SolrField(textContentField)]
    public string TextContent { get; set; }

    [SolrField(catField)]
    public ICollection<string> Categories { get; set; }

    [SolrField(nameField)]
    public string Name { get; set; }
    [SolrField(pathField)]
    public string Path { get; set; }
    [SolrField(sizeField)]
    public long Size { get; set; }
    [SolrField(ownerIdField)]
    public int OwnerId { get; set; }
}
