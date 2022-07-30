using System.Collections.Generic;
using Atles.Core.Events;

namespace Atles.Core.Results;

public record Success(IEnumerable<IEvent> Events);
