using System;
using System.Net.Http;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace XOutput.Client
{
    public abstract class HttpJsonClient
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

        protected CancellationToken GetToken(int millis)
        {
            var source = new CancellationTokenSource();
            source.CancelAfter(millis);
            return source.Token;
        }

        protected async Task<HttpResponseMessage> GetResponse(HttpRequestMessage request, CancellationToken token = default)
        {
            return await client.SendAsync(request, token);
        }

        protected async Task<string> GetResponseBody(HttpRequestMessage request, int timeoutMillis = 1000)
        {
            var token = GetToken(timeoutMillis);
            using (var response = await GetResponse(request, token))
            {
                response.EnsureSuccessStatusCode();
                return await response.Content.ReadAsStringAsync(token);
            }
        }

        protected async Task<T> GetResult<T>(HttpRequestMessage request, int timeoutMillis = 1000)
        {
            var token = GetToken(timeoutMillis);
            using (var response = await GetResponse(request, token))
            {
                response.EnsureSuccessStatusCode();
                var stream = await response.Content.ReadAsStreamAsync(token);
                return await JsonSerializer.DeserializeAsync<T>(stream, null, token);
            }
        }
    }
}
