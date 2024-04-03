using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace ImageBrowser.Application.Common.Interfaces;
public interface IOCRService
{
    Task<string> ExtractText(string key);
}
