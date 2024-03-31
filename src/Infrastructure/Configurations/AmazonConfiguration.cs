using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImageBrowser.Infrastructure.Configurations;
public class AmazonConfiguration
{
    public string SecretKey { get; set; }
    public string AccessKey { get; set; }
    public string EndpointUrl { get; set; }
    public string DefaultBucket { get; set; }
    //public string TempPath { get; set; }
    public string Host { get; set; }
}
