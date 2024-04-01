using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ImageBrowser.Application.Common.Interfaces;
using ImageBrowser.Application.Common.Models;

namespace ImageBrowser.Application.Accounts.Queries;
public class GetProfileQuery : IRequest<UserDto>
{
    public class Handler : IRequestHandler<GetProfileQuery, UserDto>
    {

        private readonly IApplicationDbContext dbContext;
        private readonly IFileProvider _fileProvider;
        private readonly IAppUserIdService appUser;
        

        public Handler(IAppUserIdService appUser, IApplicationDbContext dbContext, IFileProvider fileProvider)
        {
            this.dbContext = dbContext;
            _fileProvider = fileProvider;
            this.appUser = appUser;
        }

        public async Task<UserDto> Handle(GetProfileQuery request, CancellationToken cancellationToken)
        {
            var user = await dbContext.AppUsers.Include(a => a.Image).Where(a => a.Id == appUser.UserId)
                .Select(a => new UserDto
                {
                    Email = a.Email,
                    FirstName = a.FirstName,
                    LastName = a.LastName,
                    PhoneNumber = a.PhoneNumber,
                    ImageUrl = a.ImageId.HasValue ? _fileProvider.GeneratePreSignedURL(null, a.Image.Path, 2).Result.Url : null,
                    Id = a.Id,
                }).SingleOrDefaultAsync(cancellationToken);


            //var identityUser = await userManager.Users.Where(a => a.AppUserId == user.Id).SingleOrDefaultAsync(cancellationToken);
            //if (user != null)
            //{
            //    user.UserName = identityUser.UserName;
            //}

            //var roles = await userManager.GetRolesAsync(identityUser);
            //user.Roles = roles.ToList();

            return user;
        }
    }
}