using System;
using System.Net;
using System.Net.Http;

namespace XOutput.Rest
{
    public interface IHttpClientProvider
    {
        public HttpClient Client { get; }
    }

    public class StaticHttpClientProvider : IHttpClientProvider
    {
        public HttpClient Client { get; private set; }

        public StaticHttpClientProvider(Uri baseAddress)
        {
            Client = new HttpClient { BaseAddress = baseAddress };
        }

        public StaticHttpClientProvider(HttpClient client)
        {
            Client = client;
        }
    }

    public class DynamicHttpClientProvider : IHttpClientProvider
    {
        public HttpClient Client { get; set; }

        public void SetBaseAddress(Uri baseAddress)
        {
            if (Client == null)
            {
                Client = new HttpClient();
            }
            Client.BaseAddress = baseAddress;
        }
    }

    public class ClientHttpException : Exception {
        public HttpStatusCode StatusCode { get; private set; }
        public byte[] Content { get; private set; }

        public ClientHttpException(HttpStatusCode statusCode, byte[] content) {
            StatusCode = statusCode;
            Content = content;
        }
    }
}
