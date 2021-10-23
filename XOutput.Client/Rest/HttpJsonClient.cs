using System;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace XOutput.Rest
{
    public abstract class HttpJsonClient
    {
        protected readonly IHttpClientProvider clientProvider;
        protected readonly JsonSerializerOptions serializerOptions = new JsonSerializerOptions {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        };

        protected HttpJsonClient(IHttpClientProvider clientProvider)
        {
            this.clientProvider = clientProvider;
        }

        protected CancellationToken GetToken(int millis)
        {
            var source = new CancellationTokenSource();
            source.CancelAfter(millis);
            return source.Token;
        }

        protected HttpContent GetRequest<T>(T request)
        {
            if (request == null)
            {
                return new ByteArrayContent(Array.Empty<byte>());
            }
            else
            {
                string requestString = JsonSerializer.Serialize(request);
                return new StringContent(requestString, Encoding.UTF8);
            }
        }

        protected async Task<string> GetResponseBodyAsync(Func<Task<HttpResponseMessage>> responseGetter, CancellationToken token)
        {
            using (var response = await responseGetter())
            {
                response.EnsureSuccessStatusCode();
                return await response.Content.ReadAsStringAsync(token);
            }
        }

        protected async Task<T> GetResultAsync<T>(Func<Task<HttpResponseMessage>> responseGetter, CancellationToken token)
        {
            using (var response = await responseGetter())
            {
                response.EnsureSuccessStatusCode();
                var stream = await response.Content.ReadAsStreamAsync(token);
                return await JsonSerializer.DeserializeAsync<T>(stream, serializerOptions, token);
            }
        }

        protected Task<T> GetAsync<T>(string path, CancellationToken token)
        {
            return GetResultAsync<T>(() => clientProvider.Client.GetAsync(path, token), token);
        }

        protected Task<T> DeleteAsync<T>(string path, CancellationToken token)
        {
            return GetResultAsync<T>(() => clientProvider.Client.DeleteAsync(path, token), token);
        }

        protected Task<T> PostAsync<T, R>(string path, R request, CancellationToken token)
        {
            return GetResultAsync<T>(() => clientProvider.Client.PostAsync(path, GetRequest(request), token), token);
        }

        protected Task<T> PatchAsync<T, R>(string path, R request, CancellationToken token)
        {
            return GetResultAsync<T>(() => clientProvider.Client.PatchAsync(path, GetRequest(request), token), token);
        }

        protected Task<T> PutAsync<T, R>(string path, R request, CancellationToken token)
        {
            return GetResultAsync<T>(() => clientProvider.Client.PutAsync(path, GetRequest(request), token), token);
        }
    }
}
