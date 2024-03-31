using FluentValidation;
using ImageBrowser.Application.Common.Interfaces;
using ImageBrowser.Application.Common.Models;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;



namespace ImageBrowser.Application.Files.Commands;
public class AddFileCommand : IRequest<ServiceResult>
{
    public IFormFile Upload { get; set; }
    public string Path { get; set; }

    public class Handler : IRequestHandler<AddFileCommand, ServiceResult>
    {

        private readonly IIdentityService service;
        private readonly IFileProvider _fileServices;
        private readonly IApplicationDbContext dbContext;
        private readonly IAppUserIdService appUser;

        public Handler(IIdentityService service, IFileProvider fileServices, IAppUserIdService appUser, IApplicationDbContext dbContext)
        {
            this.service = service;
            _fileServices = fileServices;
            this.dbContext = dbContext;
            this.appUser = appUser;
        }

        public async Task<ServiceResult> Handle(AddFileCommand request, CancellationToken cancellationToken)
        {
            if (request.Upload == null)
            {
                return ServiceResult.Failure("upload_file_necessary");
            }

            long fileSizeInBytes = request.Upload.Length;
            double fileSizeInKB = fileSizeInBytes / 1024.0; // Convert bytes to kilobytes
            double fileSizeInMB = fileSizeInKB / 1024.0;
            if (fileSizeInMB > 5)
            {
                return ServiceResult.Failure("Uplaoded file size is bigger than 5 MB");
            }

            // var res = await fileManager.Upload(request.Path, request.Upload);
            //return res;

            // Wrap the file upload operation in Task.Run
            var uploadTask = Task.Run(() => _fileServices.UploadFileAsync(request.Path, request.Upload), cancellationToken);

            // Define a timeout duration (adjust as needed)
            var timeoutMilliseconds = 90000; // 30 seconds

            // Use Task.WhenAny to await the first task to complete or timeout
            var completedTask = await Task.WhenAny(uploadTask, Task.Delay(timeoutMilliseconds, cancellationToken));

            if (completedTask == uploadTask)
            {
                // The uploadTask completed successfully
                var res = await uploadTask;
                return res;
            }
            else
            {
                // Handle the timeout case
                return ServiceResult.Failure("File upload timed out.");
            }
        }
    }
}
