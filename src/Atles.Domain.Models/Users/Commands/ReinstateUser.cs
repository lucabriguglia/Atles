using Atles.Infrastructure.Commands;
using System;
using Docs.Attributes;

namespace Atles.Domain.Users.Commands
{
    /// <summary>
    /// Request to re-active a suspended user.
    /// </summary>
    [DocRequest(typeof(User))]
    public class ReinstateUser : CommandBase
    {
    }
}