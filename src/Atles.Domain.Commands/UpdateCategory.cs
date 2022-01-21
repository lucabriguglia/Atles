using System;
using Atles.Domain.Models.Categories;
using Atles.Infrastructure.Commands;
using Docs.Attributes;

namespace Atles.Domain.Commands
{
    /// <summary>
    /// Request that updates a forum category.
    /// </summary>
    [DocRequest(typeof(Category))]
    public class UpdateCategory : CommandBase
    {
        /// <summary>
        /// The new name of the forum category.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// The unique identifier of the new permission set for the forum category.
        /// </summary>
        public Guid PermissionSetId { get; set; }
    }
}
