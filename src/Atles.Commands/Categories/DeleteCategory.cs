using Atles.Core.Commands;

namespace Atles.Commands.Categories
{
    /// <summary>
    /// Request that deletes a Forum Category.
    /// </summary>
    public class DeleteCategory : CommandBase
    {
        public Guid CategoryId { get; set; } = Guid.NewGuid();
    }
}
