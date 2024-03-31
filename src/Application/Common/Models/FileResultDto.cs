using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace ImageBrowser.Application.Common.Models
{
    public class FileResultDto
    {
        public Stream Stream { get; set; }
        public string ETag { get; set; }
        public string ContentRange { get; set; }    
        public long ContentLength { get; set; }
        public HttpStatusCode HttpStatusCode { get; set; }
        public string FileName { get; set; }
    }

    public class S3FileDto
    {
        public string Name { get; set; }
        public int FileId { get; set; }
        public double Size { get; set; }
        public DateTime? LastModified { get; set; }
        public DateTime Created { get; set; }
        public string Url { get; set; }
        public string BucketName { get; set; }
        public int Total { get; set; }
        public string UserName { get; set; }
    }
}
