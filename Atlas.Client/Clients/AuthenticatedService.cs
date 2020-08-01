using System.Net.Http;

namespace Atlas.Client.Clients
{
    public class AuthenticatedService : ServiceBase
    {
        public AuthenticatedService(HttpClient httpClient) 
            : base(httpClient)
        {
        }
    }
}
