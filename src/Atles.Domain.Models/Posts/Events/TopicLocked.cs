﻿using Atles.Infrastructure.Events;

namespace Atles.Domain.Models.Posts.Events
{
    /// <summary>
    /// Event published when a topic is locked.
    /// </summary>
    public class TopicLocked : EventBase
    {
        /// <summary>
        /// Value indicating whether the topic is locked (true) or unlocked (false).
        /// </summary>
        public bool Locked { get; set; }
    }
}