# Clean Architecture and CQRS

## Overview

**ImageBrowser** is a .NET Core REST API application that demonstrates the implementation of **Clean Architecture**, **CQRS**, and **Domain-Driven Design (DDD)** principles. It allows users to browse and manage images efficiently using extracting features and indexing it into a Search engine (solr).

## Key Concepts

### Clean Architecture

Clean Architecture emphasizes separation of concerns, maintainability, and testability. It organizes the application into distinct layers, each with specific responsibilities:

1. **Presentation Layer (Web Project)**: Handles HTTP requests and responses. Contains controllers.
2. **Application Layer**: Contains application-specific logic. Implements use cases and orchestrates interactions between other layers.
3. **Domain Layer**: Represents the core business logic. Contains entities, value objects, and domain services.
4. **Infrastructure Layer**: Deals with external dependencies (e.g., databases, APIs, file systems). Includes data access, caching, and external services.

### CQRS (Command Query Responsibility Segregation)

CQRS separates the read and write responsibilities of an application. It introduces two distinct models:

1. **Write Model (Commands)**:
    - Handles commands (e.g., create, update, delete).
    - Enforces business rules and updates the domain model.
    - Utilizes the **MediatR** library for command handling.
2. **Read Model (Queries)**:
    - Executes queries (e.g., fetching data for display).
    - Optimized for read performance.
    - Uses Entity Framework and Linq to query to database.

## Code Snippets

Let’s explore some relevant code snippets from your **ImageBrowser** project:

### Command Handling (Write Model)

Application/Files/Commands/CreateFileCommand.cs

```csharp
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
```

AI-generated code. Review and use carefully. [More info on FAQ](https://www.bing.com/new#faq).

### Query Execution (Read Model)

src/Application/Accounts/Queries/GetProfileQuery.cs

```csharp
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
```

AI-generated code. Review and use carefully. [More info on FAQ](https://www.bing.com/new#faq).

## Getting Started

1. Create an empty database.
2. Make sure **`InitializeDatabase`** is running in Program.cs.
3. Set the connection string `ProductionConneciton` in **`appsettings.json`** or use the user secrets mechanism.
4. Run the application!

## Additional Resources

- Simple CQRS Implementation with Raw SQL and DDD
- Domain Model Encapsulation and Persistence Ignorance with Entity Framework 2.2

If you find this project helpful, please consider giving it a star on GitHub. Happy coding! 🚀

---

ImageBrowser GitHub Repository

**Learn more**

[GitHub - kgrzybek/sample-dotnet-core-cqrs-api: Sample .NET Core REST API CQRS implementation with raw SQL and DDD using Clean Architecture.](https://github.com/kgrzybek/sample-dotnet-core-cqrs-api)

[GitHub - DigitalCaesar/CleanArchitectureSample: A sample for best practices and experimenting with Clean Architecture and CQRS](https://github.com/DigitalCaesar/CleanArchitectureSample)

[GitHub - danielmackay/Clean-Architecture-CQRS-Templates: Clean Architecture Command and Query templates](https://github.com/danielmackay/Clean-Architecture-CQRS-Templates)

[GitHub - Rezakazemi890/Clean-Architecture-CQRS: Clean Architecture with CQRS Pattern .NET 8](https://github.com/Rezakazemi890/Clean-Architecture-CQRS)