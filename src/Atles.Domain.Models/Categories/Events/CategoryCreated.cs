using System;
using Atles.Infrastructure.Events;

namespace Atles.Domain.Models.Categories.Events
{
    public class CategoryCreated : EventBase
    {
        public string Name { get; set; }
        public Guid PermissionSetId { get; set; }
        public int SortOrder { get; set; }
    }
}
