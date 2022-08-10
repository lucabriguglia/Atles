using Atles.Core.Events;

namespace Atles.Core.Results.Types;

public record Success(IEnumerable<IEvent> Events, object Result = null);
