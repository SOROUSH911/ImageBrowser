using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata;
using System.Text;
using System.Threading.Tasks;
using ImageBrowser.Application.Common.Interfaces;
using ImageBrowser.Application.Common.Models;

namespace ImageBrowser.Application.Common.Mappings;
public class ObjectMappingProfile : Profile
{

    const string bucketName = "simpleimagebrowser";
    private readonly IFileProvider fileProvider;
    private readonly IAppUserIdService appUser;

    public ObjectMappingProfile()
    {

    }
    public ObjectMappingProfile(IAppUserIdService appUser, IFileProvider fileProvider)
    {
        this.fileProvider = fileProvider;
        this.appUser = appUser;
        CreateMap<Domain.Entities.File, FileDto>()
            .ForMember(s => s.ObjectUrl, d => d.MapFrom(o => fileProvider.GeneratePreSignedURL(bucketName, o.Path, 2).Result));
    }
}
