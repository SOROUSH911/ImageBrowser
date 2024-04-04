using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ImageBrowser.Application.Common.Interfaces;
using ImageBrowser.Domain.SearchEngine;
using SolrNet.Commands.Parameters;
using SolrNet;

namespace ImageBrowser.Infrastructure.Services;
internal class IBSearch : ISearchService
{
    private readonly ISolrOperations<IBDataSchema> _solrOperations;

    public IBSearch(ISolrOperations<IBDataSchema> solrOperations)
    {
        _solrOperations = solrOperations;
    }
    public async Task<SearchResponse> DoSearch(SearchQuery query)
    {
        ////Create an object to hold results
        //FiltersFacets filtersFacets = new FiltersFacets();
        ////Highlights highlights = new Highlights();
        //ExtraParameters extraParameters = new ExtraParameters();
        //var highlights = new Highlights();

        SolrQueryResults<IBDataSchema> solrResults;
        SearchResponse queryResponse = new SearchResponse
        {
            //Echo back the original query 
            OriginalQuery = query
        };

        //Get a connection
        QueryOptions queryOptions = new QueryOptions
        {
            Rows = query.Rows,
            StartOrCursor = new StartOrCursor.Start(query.Start),
            //FilterQueries = filtersFacets.BuildFilterQueries(query),
            //Facet = filtersFacets.BuildFacets(query.Pivots),
            //Stats = filtersFacets.BuildStats(),
            //Highlight = highlights.BuildHighlightParameters(),
            //ExtraParams = extraParameters.BuildExtraParameters(query),
            Fields = { "*", "score" },
            Grouping = query.Grouping.Fields.Count == 0 ? null : new GroupingParameters() { Fields = query.Grouping.Fields, Limit = query.Grouping.Limit, Format = GroupingFormat.Grouped, },
        };

        if (!string.IsNullOrEmpty(query.Sort))
        {
            queryOptions.OrderBy = new[] { new SortOrder(query.Sort, query.IsAscending ? Order.ASC : Order.DESC) };
        }
        //Execute the query
        ISolrQuery solrQuery = new SolrQuery(string.IsNullOrEmpty(query.Query) ? "*:*" : query.Query);
        try
        {
            solrResults = await _solrOperations.QueryAsync(solrQuery, queryOptions);
        }
        catch (Exception e)
        {
            queryResponse.Status = -1;
            queryResponse.Error = e.Message;
            return queryResponse;
        }

        //Set response
        //var extractResponse = new ResponseExtraction();

        //if (solrResults.Count > 0)
        //{
        //    extractResponse.SetHeader(queryResponse, solrResults);
        //}
        //extractResponse.SetBody(queryResponse, solrResults);
        //if (solrResults.Grouping.Count == 0 && solrResults.FacetPivots.Count == 0)
        //{
        //    extractResponse.SetSpellcheck(queryResponse, solrResults);
        //    extractResponse.SetFacets(queryResponse, solrResults);
        //}
        return queryResponse;

    }
}
