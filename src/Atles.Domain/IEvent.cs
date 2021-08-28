using System;

namespace Atles.Domain
{

    public interface IEvent
    {
        Guid Id { get; set; }
        Guid StoreId { get; set; }
        string UserId { get; set; }
        DateTime TimeStamp { get; set; }
    }
}
