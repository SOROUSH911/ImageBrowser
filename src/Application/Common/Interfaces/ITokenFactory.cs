using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ImageBrowser.Application.Common.Models;

namespace ImageBrowser.Application.Common.Interfaces;
public interface ITokenFactory
{
    TokenResult TokenGenerator(string secretToken, string secreturl, string userName, List<string> roles, string IdentityUSerId, int AppUserId, int expireMiniutes, int accessTokenExpireMinutes);
}
