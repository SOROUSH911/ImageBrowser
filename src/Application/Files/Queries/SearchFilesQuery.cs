using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ImageBrowser.Application.Common.Helpers;
using ImageBrowser.Application.Common.Interfaces;
using ImageBrowser.Application.Common.Models;
using ImageBrowser.Domain.SearchEngine;
using MassTransit.Saga;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace ImageBrowser.Application.Files.Queries;
public class SearchFilesQuery : PageQuery, IRequest<QueryResult<FileDto>>
{
    internal class Handler : IRequestHandler<SearchFilesQuery, QueryResult<FileDto>>
    {
        private readonly IApplicationDbContext _dbContext;
        private readonly IAppUserIdService _appUser;
        private readonly IMapper _mapper;
        private readonly ISearchService _searchService;

        public Handler(IApplicationDbContext dbContext, IAppUserIdService appUser, IMapper mapper, ISearchService searchService)
        {
            _dbContext = dbContext;
            _appUser = appUser;
            _mapper = mapper;
            _searchService = searchService;
        }
        public async Task<QueryResult<FileDto>> Handle(SearchFilesQuery request, CancellationToken cancellationToken)
        {

            var PageNumber = request.PageNumber;
            var PageSize = request.PageSize;

            var searchQuery = new SearchQuery
            {
                Query = request.Search,
                Start = (PageNumber - 1) * PageSize,
                Rows =  PageSize,
                IsAscending = true
            };



            var result = await _searchService.DoSearch(searchQuery, _appUser.UserId);
            if (result.Status < 0)
            {
                throw new SearchEngineException();
            }

            var files = await _mapper.ProjectTo<FileDto>(_dbContext.Files.Where(f => result.Results.Select(r => SolrHelper.GetEntityId(r.Id)).ToList().Contains(f.Id) && f.OwnerId == _appUser.UserId)).ToListAsync(cancellationToken);


            return new QueryResult<FileDto>
            {
                Data = files,
                Total = result.TotalHits
            };





        }
    }
}
