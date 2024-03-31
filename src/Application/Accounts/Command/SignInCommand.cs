using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using ImageBrowser.Application.Common.Interfaces;
using ImageBrowser.Application.Common.Models;

namespace ImageBrowser.Application.Accounts.Command;
public class SignInCommand : IRequest<TokenDto>
{
    public TokenRequest LoginRequest { get; set; }
    [JsonIgnore]
    public bool IsAdminPanel { get; set; }
    public string ReturnUrl { get; set; }

    public class Validator : AbstractValidator<SignInCommand>
    {
        public Validator()
        {
            RuleFor(c => c.LoginRequest.Client_id).NotEmpty()
                              .WithMessage("clientId_necessary");

            RuleFor(c => c.LoginRequest.Client_secret).NotEmpty()
                             .WithMessage("clientSecret_necessary");

            RuleFor(c => c.LoginRequest.grant_type).NotEmpty()
                                .WithMessage("grant_type_is_necessary");

            When(a => a.LoginRequest.grant_type.ToLower() == "password", () =>
            {
                RuleFor(c => c.LoginRequest.Password)
                .NotEmpty().WithMessage("password_necessary");

                RuleFor(c => c.LoginRequest.UserName)
                    .NotEmpty().WithMessage("user_name_necessary");

            }).Otherwise(() =>
            {
                RuleFor(c => c.LoginRequest.Refresh_Token).NotEmpty()
                             .WithMessage("grant_type_isNot_valid");
            });

        }
    }

    public class Handler : IRequestHandler<SignInCommand, TokenDto>
    {

        private readonly IIdentityService service;
        private readonly IApplicationDbContext dbContext;

        public Handler(IIdentityService service, IApplicationDbContext dbContext)
        {
            this.service = service;
            this.dbContext = dbContext;
        }

        public async Task<TokenDto> Handle(SignInCommand request, CancellationToken cancellationToken)
        {
            var result = await service.SignInUser(request.LoginRequest, request.IsAdminPanel, cancellationToken);

            if (!result.IsSuccess)
            {
                return new TokenDto
                {
                    Error = result.Error,
                    IsSuccess = false
                };
            }
            result.IsSuccess = true;
            return result;
        }
    }
}
