using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ImageBrowser.Domain.SearchEngine;

namespace ImageBrowser.Application.Common.Interfaces;
public interface ISearchService
{
    Task<SearchResponse> DoSearch(SearchQuery query);
}
