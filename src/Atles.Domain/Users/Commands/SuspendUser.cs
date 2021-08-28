using System;
using Docs.Attributes;

namespace Atles.Domain.Users.Commands
{
    /// <summary>
    /// Request to suspend a user.
    /// </summary>
    [DocRequest(typeof(User))]
    public class SuspendUser : CommandBase
    {
    }
}