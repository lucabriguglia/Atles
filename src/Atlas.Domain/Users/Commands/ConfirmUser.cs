using System;
using Docs.Attributes;

namespace Atlas.Domain.Users.Commands
{
    [DocRequest(typeof(User))]
    public class ConfirmUser : CommandBase
    {
        public Guid Id { get; set; }
    }
}