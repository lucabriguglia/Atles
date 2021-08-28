using System;

namespace Atles.Domain
{
    public interface ICommand
    {
        Guid Id { get; set; }
        Guid SiteId { get; set; }
        Guid UserId { get; set; }
    }
}
