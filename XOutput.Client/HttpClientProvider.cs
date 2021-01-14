using System;
using System.Net.Http;

namespace XOutput.Client
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
}
