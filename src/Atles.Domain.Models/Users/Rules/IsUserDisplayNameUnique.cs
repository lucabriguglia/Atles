using System;
using Atles.Infrastructure.Queries;

namespace Atles.Domain.Models.Users.Rules
{
    public class IsUserDisplayNameUnique : QueryBase<bool>
    {
        public string DisplayName { get; set; }
        public Guid? Id { get; set; }
    }
}
