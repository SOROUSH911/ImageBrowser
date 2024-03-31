using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ImageBrowser.Application.Common.Models;

namespace ImageBrowser.Application.Common.Interfaces;
public interface IEmailService
{
    Task Send(EmailMessageModel emailMessage);
}
