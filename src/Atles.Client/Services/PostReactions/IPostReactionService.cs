using System;
using System.Threading.Tasks;
using Atles.Client.Models;
using Atles.Models.Public;

namespace Atles.Client.Services.PostReactions
{
    public interface IPostReactionService
    {
        Task AddReaction(Guid forumId, Guid topicId, ReactionCommandModel command);
        Task RemoveReaction(Guid forumId, Guid topicId, ReactionCommandModel command);
        Task<UserTopicReactionsModel> GetReactions(Guid topicId);
    }
}
