using System;

namespace Atles.Core.Commands
{
    public interface ICommand
    {
        Guid Id { get; set; }
        Guid SiteId { get; set; }
        Guid UserId { get; set; }
    }
}
