using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ImageBrowser.Application.Common.Models;
using Microsoft.AspNetCore.Http;

namespace ImageBrowser.Application.Common.Interfaces;
public interface IFileProvider
{
    Task<ServiceResult> DeleteObjectFromS3Async(string keyName);
    Task<S3FileDto> GeneratePreSignedURL(string bucketName, string objectKey, double duration);
    Task<FileResultDto> GetObjectFromS3Async(string keyName);
    Task<ServiceResult> UploadFileAsync(string path, IFormFile file);
}
