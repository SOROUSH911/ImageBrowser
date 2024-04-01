using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImageBrowser.Application.Common.Event;

public record ExtractImageAttributesEvent()
{
    public int FileId { get; init; }
}
