using System.Collections.Generic;

namespace Atles.Core.Services;

public interface IServiceProviderWrapper
{
    T GetService<T>();
    IEnumerable<T> GetServices<T>();
}