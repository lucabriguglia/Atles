using Atles.Core.Commands;
using Atles.Domain.Models;
using Docs.Attributes;

namespace Atles.Domain.Commands.Categories
{
    /// <summary>
    /// Request that deletes a Forum Category.
    /// </summary>
    [DocRequest(typeof(Category))]
    public class DeleteCategory : CommandBase
    {
        public Guid CategoryId { get; set; } = Guid.NewGuid();
    }
}
