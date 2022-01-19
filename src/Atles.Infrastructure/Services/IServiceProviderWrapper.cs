using System.Collections.Generic;

namespace Atles.Infrastructure.Services
{
    public interface IServiceProviderWrapper
    {
        T GetService<T>();
        IEnumerable<T> GetServices<T>();
    }
}
