using System;

namespace Atles.Core;

public interface IDateTimeProvider
{
    public DateTimeOffset UtcNow { get; }
}
