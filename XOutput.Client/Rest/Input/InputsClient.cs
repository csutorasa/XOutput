namespace XOutput.Rest.Input
{
    public class InputsClient : HttpJsonClient
    {
        public InputsClient(IHttpClientProvider clientProvider) : base(clientProvider)
        {

        }
    }
}
