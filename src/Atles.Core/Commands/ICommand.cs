using System;

namespace Atles.Core.Commands;

public interface ICommand
{
    Guid SiteId { get; set; }
    Guid UserId { get; set; }
}