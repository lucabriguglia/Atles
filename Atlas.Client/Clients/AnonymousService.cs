using System.Net.Http;

namespace Atlas.Client.Clients
{
    public class AnonymousService : ServiceBase
    {
        public AnonymousService(HttpClient httpClient) 
            : base(httpClient)
        {
        }
    }
}
