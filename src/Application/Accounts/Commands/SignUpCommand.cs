using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ImageBrowser.Application.Common.Interfaces;
using ImageBrowser.Application.Common.Models;
using ImageBrowser.Domain.Entities;

namespace ImageBrowser.Application.Accounts.Commands;
public class SignUpCommand : IRequest<ServiceResult>
{
    public CreateUserDto User { get; set; }

    public class Validator : AbstractValidator<SignUpCommand>
    {
        public Validator()
        {

            RuleFor(c => c.User.Email).NotEmpty()
                             .WithMessage("email_necessary")
                             .EmailAddress().WithMessage("invalid_email");

            RuleFor(c => c.User.Password).NotEmpty()
                             .WithMessage("password_necessary");

            RuleFor(c => c.User.FirstName).NotEmpty()
                             .WithMessage("first_name_necessary");

            RuleFor(c => c.User.LastName).NotEmpty()
                             .WithMessage("last_name_necessary");
        }
    }

    public class Handler : IRequestHandler<SignUpCommand, ServiceResult>
    {

        private readonly IIdentityService identityService;
        private readonly IApplicationDbContext dbContext;
        private readonly IAppUserIdService appUserService;

        public Handler(IIdentityService identityService, IApplicationDbContext dbContext, IAppUserIdService appUserService)
        {
            this.identityService = identityService;
            this.dbContext = dbContext;
            this.appUserService = appUserService;
        }

        public async Task<ServiceResult> Handle(SignUpCommand request, CancellationToken cancellationToken)
        {
            var duplicateUser = await dbContext.AppUsers.AnyAsync(a => a.Email.Equals(request.User.Email), cancellationToken);
            if (duplicateUser)
            {
                return ServiceResult.Failure("email_duplicate");
            }
            //if (await identityService.UsernameExists(request.User.UserName, 0))
            //{
            //    return ServiceResult.Failure("username_duplicate");

            //}
            if (request.User.ImageId.HasValue && !await dbContext.Files.AnyAsync(f => f.Id == request.User.ImageId.Value))
            {
                return ServiceResult.Failure("file_notfound");

            }

            if (request.User.ImageId.HasValue && !await dbContext.Files.AnyAsync(f => f.Id == request.User.ImageId))
            {
                return ServiceResult.Failure("image_file_notfound");

            }
            await dbContext.BeginTransactionAsync();
            var appUser = new User
            {
                FirstName = request.User.FirstName,
                LastName = request.User.LastName,
                Email = request.User.Email,
                PhoneNumber = request.User.PhoneNumber,
                ImageId = request.User.ImageId,
            };
            dbContext.AppUsers.Add(appUser);
            await dbContext.SaveChangesAsync(cancellationToken);

            try
            {
                var createUserRes = await identityService.CreateUserAsync(request.User.Password, request.User.FirstName, request.User.LastName, request.User.Email, request.User.PhoneNumber, appUser.Id);
                //if (appUserService.IsAdmin && request.User.IsSupervisor)
                //{
                //    var userId = createUserRes.UserId;
                //    await identityService.AddToRole(userId, "Supervisor");
                //}

                //multiobjectfiles
                if (!createUserRes.Result.Succeeded)
                {
                  throw new Exception(String.Join(", ", createUserRes.Result.Errors));
                }
                    await dbContext.CommitTransactionAsync();
            }
            catch(Exception ex)
            {
                //dbContext.AppUsers.Remove(appUser);
                //await dbContext.SaveChangesAsync(cancellationToken);
                dbContext.RollbackTransaction();
                return ServiceResult.Failure("error_createuser" + "\n " + ex.Message);
            }


            return ServiceResult.Success(appUser.Id);
        }
    }
}
