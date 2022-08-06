using Atles.Core.Commands;

namespace Atles.Commands.Forums;

/// <summary>
/// Request that creates a forum section.
/// </summary>
public class CreateForum : CommandBase
{
    public Guid ForumId { get; set; } = Guid.NewGuid();

    /// <summary>
    /// The unique identifier of the forum category which the new forum belongs to.
    /// </summary>
    public Guid CategoryId { get; set; }

    /// <summary>
    /// The name of the new forum.
    /// </summary>
    public string Name { get; set; }

    /// <summary>
    /// The slug for the new forum.
    /// </summary>
    public string Slug { get; set; }

    /// <summary>
    /// The description of the new forum.
    /// </summary>
    public string? Description { get; set; }

    /// <summary>
    /// The unique identifier of the permission set.
    /// If not assigned, the new forum will inherit the permission set from the forum category.
    /// </summary>
    public Guid? PermissionSetId { get; set; }
}
