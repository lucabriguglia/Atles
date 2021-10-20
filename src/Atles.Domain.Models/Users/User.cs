﻿using System;
using System.Collections.Generic;
using Atles.Domain.PostReactions;
using Atles.Domain.Posts;
using Docs.Attributes;

namespace Atles.Domain.Users
{
    /// <summary>
    /// User
    /// </summary>
    [DocTarget(Consts.DocsContextForum)]
    public class User
    {
        /// <summary>
        /// The unique identifier of the user.
        /// </summary>
        public Guid Id { get; private set; }

        /// <summary>
        /// The unique identifier of the user in the membership database.
        /// </summary>
        public string IdentityUserId { get; private set; }

        /// <summary>
        /// The email of the user in the membership database.
        /// </summary>
        public string Email { get; private set; }

        /// <summary>
        /// The display name of the user.
        /// It is used as the public name of the user.
        /// </summary>
        public string DisplayName { get; private set; }

        /// <summary>
        /// The number of the topics published by the user.
        /// </summary>
        public int TopicsCount { get; private set; }

        /// <summary>
        /// The number of replies created by the user.
        /// </summary>
        public int RepliesCount { get; private set; }

        /// <summary>
        /// The status of the user.
        /// <list type="bullet">
        /// <item>
        /// <description>Pending: User is registered but the email is not confirmed.</description>
        /// </item>
        /// <item>
        /// <description>Active: User is registered and the email is confirmed.</description>
        /// </item>
        /// <item>
        /// <description>Suspended: User cannot create new posts (topics or replies).</description>
        /// </item>
        /// </list>
        /// </summary>
        public UserStatusType Status { get; private set; }

        /// <summary>
        /// The time stamp of when the user has been created.
        /// </summary>
        public DateTime TimeStamp { get; set; }

        /// <summary>
        /// The list of posts created by the user.
        /// </summary>
        public virtual ICollection<Post> Posts { get; set; }

        /// <summary>
        /// Reference to post likes.
        /// </summary>
        public virtual ICollection<PostReaction> PostReactions { get; set; }

        /// <summary>
        /// The list of events generated by the user.
        /// </summary>
        public virtual ICollection<Event> Events { get; set; }

        /// <summary>
        /// Creates an empty user.
        /// </summary>
        public User()
        {
        }

        /// <summary>
        /// Creates a new user with the given values.
        /// </summary>
        /// <param name="id"></param>
        /// <param name="identityUserId"></param>
        /// <param name="email"></param>
        /// <param name="displayName"></param>
        public User(Guid id, string identityUserId, string email, string displayName)
        {
            New(id, identityUserId, email, displayName);
        }

        /// <summary>
        /// Creates a new user with the given values including the unique identifier.
        /// </summary>
        /// <param name="identityUserId"></param>
        /// <param name="email"></param>
        /// <param name="displayName"></param>
        public User(string identityUserId, string email, string displayName)
        {
            New(Guid.NewGuid(), identityUserId, email, displayName);
        }

        private void New(Guid id, string identityUserId, string email, string displayName)
        {
            Id = id;
            IdentityUserId = identityUserId;
            Email = email;
            DisplayName = displayName;
            Status = UserStatusType.Pending;
            TimeStamp = DateTime.UtcNow;
        }

        /// <summary>
        /// Set the status to active when the user confirms the email address.
        /// </summary>
        public void Confirm()
        {
            Status = UserStatusType.Active;
        }

        /// <summary>
        /// Updates user's details.
        /// </summary>
        /// <param name="displayName"></param>
        public void UpdateDetails(string displayName)
        {
            DisplayName = displayName;
        }

        /// <summary>
        /// Increases the number of topics by 1.
        /// </summary>
        public void IncreaseTopicsCount()
        {
            TopicsCount += 1;
        }

        /// <summary>
        /// Increases the number of replies by 1.
        /// </summary>
        public void IncreaseRepliesCount()
        {
            RepliesCount += 1;
        }

        /// <summary>
        /// Decrease the number of topics by the given value.
        /// If no value is given it will decrease by 1.
        /// If the resulting number is less than zero, the value will be set to zero.
        /// </summary>
        /// <param name="count"></param>
        public void DecreaseTopicsCount(int count = 1)
        {
            TopicsCount -= count;

            if (TopicsCount < 0)
            {
                TopicsCount = 0;
            }
        }

        /// <summary>
        /// Decrease the number of replies by the given value.
        /// If no value is given it will decrease by 1.
        /// If the resulting number is less than zero, the value will be set to zero.
        /// </summary>
        /// <param name="count"></param>
        public void DecreaseRepliesCount(int count = 1)
        {
            RepliesCount -= count;

            if (RepliesCount < 0)
            {
                RepliesCount = 0;
            }
        }

        /// <summary>
        /// Sets the status of the user as suspended.
        /// The user will no longer be able to write new posts.
        /// </summary>
        public void Suspend()
        {
            Status = UserStatusType.Suspended;
        }

        /// <summary>
        /// Re-activates the user if suspended.
        /// </summary>
        public void Reinstate()
        {
            Status = UserStatusType.Active;
        }

        /// <summary>
        /// Sets the status of the user as deleted.
        /// </summary>
        public void Delete()
        {
            Status = UserStatusType.Deleted;
        }
    }
}