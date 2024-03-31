using System;
using System.Collections.Generic;
using System.IO.Compression;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Amazon.S3;
using Amazon.S3.Model;
using Amazon.S3.Util;
using Amazon.S3.Transfer;
using Humanizer.Bytes;
using ImageBrowser.Application.Common.Interfaces;
using ImageBrowser.Infrastructure.Configurations;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using ImageBrowser.Application.Common.Models;
using System.Threading;

namespace ImageBrowser.Infrastructure.Services;
public class FileProvider : IFileProvider
{
    protected string bucketName;
    //static IAmazonS3 client;
    //static ListObjectsResponse response;
    //static ListObjectsResponse childResponse;
    public string RootName;
    //long sizeValue = 0;
    //private string eKey = "Ramoon@^%0#%$";
    //List<FileManagerDirectoryContent> s3ObjectFiles = new List<FileManagerDirectoryContent>();
    TransferUtility fileTransferUtility;
    private readonly AmazonConfiguration amazonConfiguration;
    private readonly IAppUserIdService appUser;
    private readonly IAmazonS3 client;
    private readonly IApplicationDbContext dbContext;

    public FileProvider(IAmazonS3 client, IApplicationDbContext dbContext, IAppUserIdService appUser, IOptions<AmazonConfiguration> amazonConfiguration)
    {
        this.amazonConfiguration = amazonConfiguration.Value;
        this.appUser = appUser;
        this.client = client;
        this.dbContext = dbContext;
        RegisterAmazonS3();
    }

    // Register the amazon client details
    public void RegisterAmazonS3()
    {
        bucketName = amazonConfiguration.DefaultBucket;
        //var config = new AmazonS3Config
        //{
        //    //ForcePathStyle = true,
        //    ServiceURL = amazonConfiguration.EndpointUrl
        //};
        //client = new AmazonS3Client(
        //        //amazonConfiguration.AccessKey,
        //        //amazonConfiguration.SecretKey,
        //        config
        //        );
        fileTransferUtility = new TransferUtility(client);
    }

    public async Task<ServiceResult> UploadFileAsync(string path, IFormFile file)
    {


        string TempPath = Path.GetTempPath();
        string fileName = Path.GetFileName(file.FileName);
        string fullName = Path.Combine(TempPath, fileName + Guid.NewGuid().ToString("N"));
        try
        {
            //var fileTransferUtility = new TransferUtility(_awsS3Client);

            // Option 1 (Upload an existing file in your computer to the S3)
            //await fileTransferUtility.UploadAsync(FilePathToUpload, bucketName);

            // Option2 (Upload and create the file in the process)
            //await fileTransferUtility.UploadAsync(FilePath, bucketName, UploadWithKeyName);

            // Option 3 (Upload and create the file in the process)



            //if (!File.Exists(fullName))
            //{

            //}
            using (FileStream fsSource = new FileStream(fullName, FileMode.Create))
            {
                file.CopyTo(fsSource);
                fsSource.Close();
            }

            using (var fileToUpload = new FileStream(fullName, FileMode.Open, FileAccess.Read))
            {
                await fileTransferUtility.UploadAsync(fileToUpload, bucketName, fullName);
            }


            // Option 4 (Upload and create the file in the process)
            //var fileTransferUtilityRequest = new TransferUtilityUploadRequest
            //{
            //    BucketName = bucketName,
            //    FilePath = FilePath,
            //    StorageClass = S3StorageClass.Standard,
            //    PartSize = 6291456,
            //    Key = AdvancedUpload,
            //    CannedACL = S3CannedACL.NoACL
            //};
            //fileTransferUtilityRequest.Metadata.Add("param1", "Value1");
            //fileTransferUtilityRequest.Metadata.Add("param2", "Value2");

            //await fileTransferUtility.UploadAsync(fileTransferUtilityRequest);
        }
        catch (AmazonS3Exception e)
        {
            return new ServiceResult { Error = e.Message, IsSuccess = false, StatusCode = e.StatusCode };
        }
        catch (Exception e)
        {
            return new ServiceResult { Error = e.Message, IsSuccess = false, StatusCode = HttpStatusCode.InternalServerError };
        }

        //await fileServices.AddNewFile(fullName, file.Length);
        var newFile = new Domain.Entities.File
        {
            Name = file.FileName,
            Path = fullName,
            Size = file.Length,
            OwnerId = appUser.UserId
        };
        dbContext.Files.Add(newFile);

        await dbContext.SaveChangesAsync(new CancellationToken());

        return new ServiceResult { Error = "File uploaded Successfully", StatusCode = HttpStatusCode.OK };
    }

    /// <summary>
    ///     Get a file from S3
    /// </summary>
    /// <param name="bucketName">Bucket where the file is stored</param>
    /// <param name="keyName">Key name of the file (file name including extension)</param>
    /// <returns></returns>
    public async Task<FileResultDto> GetObjectFromS3Async(string keyName)
    {


        var request = new GetObjectRequest
        {
            BucketName = bucketName,
            Key = keyName
        };
        string responseBody;

        using (var response = await client.GetObjectAsync(request))
        using (var responseStream = response.ResponseStream)
            //using (var reader = new StreamReader(responseStream))
            //{
            //    var title = response.Metadata["x-amz-meta-title"];
            //    var contentType = response.Headers["Content-Type"];
            //    responseBody = reader.ReadToEnd();
            //}

            //var createText = responseBody;
            //File.WriteAllText(FilePathToDownload + keyName, createText);
            return new FileResultDto
            {
                Stream = response.ResponseStream,
                ETag = response.ETag,
                ContentLength = response.ContentLength,
                ContentRange = response.ContentRange,
                HttpStatusCode = response.HttpStatusCode,
                FileName = keyName
            };


    }

    /// <summary>
    ///     Delete a file from an S3 bucket
    /// </summary>
    /// <param name="bucketName">Bucket where file is stored</param>
    /// <param name="keyName">Key name of the file (file name including extension)</param>
    /// <returns></returns>
    public async Task<ServiceResult> DeleteObjectFromS3Async(string keyName)
    {
        if (string.IsNullOrEmpty(keyName))
            keyName = "test.txt";

        try
        {
            var request = new GetObjectRequest { BucketName = bucketName, Key = keyName };

            var response = await client.GetObjectAsync(request);

            if (response == null || response.HttpStatusCode != HttpStatusCode.OK)
                return new ServiceResult { Error = "Error getting the object from the bucket", StatusCode = HttpStatusCode.NotFound };

            await client.DeleteObjectAsync(bucketName, keyName);

            return new ServiceResult {  Error = "The file was successfully deleted", StatusCode = HttpStatusCode.OK };
        }
        catch (AmazonS3Exception e)
        {
            return new ServiceResult { Error = e.Message, StatusCode = e.StatusCode };
        }
        catch (Exception e)
        {
            return new ServiceResult { Error = e.Message, StatusCode = HttpStatusCode.InternalServerError };
        }
    }

    public async Task<S3FileDto> GeneratePreSignedURL(string bucketName, string objectKey, double duration)
    {
        S3FileDto fileDto = new S3FileDto();
        try
        {
            //var s3Obj = await client.GetObjectAsync(new GetObjectRequest
            //{
            //    BucketName = bucketName,
            //    Key = objectKey
            //});
            //if (s3Obj == null)
            //{
            //    return null;
            //}
            fileDto = new S3FileDto
            {
                Name = objectKey,
                Size = 0,//s3Obj.ContentLength
            };

            GetPreSignedUrlRequest request1 = new GetPreSignedUrlRequest
            {
                BucketName = this.bucketName,
                Key = objectKey,
                Expires = DateTime.UtcNow.AddHours(duration)
            };
            fileDto.Url = client.GetPreSignedURL(request1);
        }
        catch (AmazonS3Exception e)
        {
            Console.WriteLine("Error encountered on server. Message:'{0}' when writing an object", e.Message);
            return new S3FileDto();
        }
        catch (Exception e)
        {
            Console.WriteLine("Unknown encountered on server. Message:'{0}' when writing an object", e.Message);
            return new S3FileDto();
        }

        return fileDto;
    }

}
public class DownloadData
{
    public DateTime ExpireDate { get; set; }
    public string FilePath { get; set; }

}