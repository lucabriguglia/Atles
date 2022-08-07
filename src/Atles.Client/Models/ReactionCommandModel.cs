using Atles.Domain;

namespace Atles.Client.Models;

public class ReactionCommandModel
{
    public PostReactionType PostReactionType { get; set; }
    public Guid PostId { get; set; }
}
