using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImageBrowser.Domain.Entities;
public class File : BaseAuditableEntity
{
    public string Name { get; set; }
    public string Path { get; set; }
    public long Size { get; set; }
    public int OwnerId { get; set; }
    public User Owner { get; set; }
}
