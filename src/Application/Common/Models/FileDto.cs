using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ImageBrowser.Domain.Entities;

namespace ImageBrowser.Application.Common.Models;
public class FileDto
{
    public string Name { get; set; }
    public string Path { get; set; }
    public long Size { get; set; }
    public int OwnerId { get; set; }
    public string ObjectUrl { get; set; }
    public DateTimeOffset Created { get; set; }
}
