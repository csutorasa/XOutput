using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using XOutput.Api.Serialization;

namespace XOutput.Client
{
    public class HttpJsonClient
    {
        protected readonly HttpClient client;

        protected HttpJsonClient()
        {
            client = new HttpClient();
        }

        protected HttpJsonClient(HttpClient client)
        {
            this.client = client;
        }

        protected async Task<HttpResponseMessage> GetResponse(Uri uri)
        {
            return await client.GetAsync(uri);
        }

        protected async Task<string> GetResponseBody(Uri uri)
        {
            using (var response = await GetResponse(uri))
            {
                response.EnsureSuccessStatusCode();
                return await response.Content.ReadAsStringAsync();
            }
        }

        protected async Task<T> GetResult<T>(Uri uri)
        {
            using (var response = await GetResponse(uri))
            {
                response.EnsureSuccessStatusCode();
                var stream = await response.Content.ReadAsStreamAsync();
                return await JsonSerializer.DeserializeAsync<T>(stream);
            }
        }
    }
}
