using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace ProjectSystemWPF.ViewModel
{
    public class REST
    {
        public HttpClient client = new HttpClient();
        public JsonSerializerOptions options = new JsonSerializerOptions();


        public REST()
        {
            client.BaseAddress = new Uri("http://localhost:5281/api/");
            options.ReferenceHandler = System.Text.Json.Serialization.ReferenceHandler.Preserve;
            options.PropertyNameCaseInsensitive = true;
        }

        static REST instance = new REST();
        public static REST Instance
        {
            get
            {
                if (instance == null)
                    instance = new REST();
                return instance;
            }
        }

        public void SetToken(string token)
        {
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
        }
    }
}
