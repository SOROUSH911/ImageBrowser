using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImageBrowser.Application.Common.Models;
public class FilterGroup
{
    public FilterGroup()
    {
    }
  

    public string Label { get; set; }
    public int Min { get; set; }
    public int Max { get; set; }
    public DateTime DateMin { get; set; }
    public DateTime DateMax { get; set; }
    public string Icon { get; set; }
    public bool IsOpen { get; set; }
    public int FieldValueType { get; set; }
}
