using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImageBrowser.Infrastructure.Configurations;
public class TokenConfiguration
{
        public string EncRefPassword { get; set; }
        public string SecretToken { get; set; }
        public string VarifiedSecretUrl { get; set; }
}
