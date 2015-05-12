using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Windows.Web.Http;


namespace NFCTrust.Writer.Service
{
    public class ServiceClientBase
    {
        protected T DeserializeJson<T>(string json) where T : class
        {
            return JsonConvert.DeserializeObject<T>(json);

        }
    }
}
