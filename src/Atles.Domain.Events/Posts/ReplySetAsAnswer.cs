using Atles.Core.Events;

namespace Atles.Events.Posts
{
    /// <summary>
    /// Event published when a reply is set as an answer.
    /// </summary>
    public class ReplySetAsAnswer : EventBase
    {
        /// <summary>
        /// Value indicating whether the reply needs to be set as answer (true) or not (false).
        /// </summary>
        public bool IsAnswer { get; set; }
    }
}