﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ImageBrowser.Application.Common.Interfaces;
using ImageBrowser.Application.Common.Models;
using Microsoft.AspNetCore.Identity.UI.Services;

namespace ImageBrowser.Infrastructure.Services;
public class EmailSender : IEmailSender
{
    private readonly IEmailService _emailService;

    public EmailSender(IEmailService emailService)
    {
        _emailService = emailService;
    }
    public async Task SendEmailAsync(string email, string subject, string htmlMessage)
    {
        EmailMessageModel emailMessage = new(email,
        subject,
        htmlMessage);

        await _emailService.Send(emailMessage);
    }
}
