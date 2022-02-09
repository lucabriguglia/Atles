namespace Atles.Domain.Models;

/// <summary>
/// Type of user level
/// </summary>
public enum UserLevelType
{
    /// <summary>
    /// Number of total posts (topics and replies).
    /// </summary>
    Posts = 1,

    /// <summary>
    /// Number of topics.
    /// </summary>
    Topics = 2,

    /// <summary>
    /// Number of replies.
    /// </summary>
    Replies = 3,

    /// <summary>
    /// Number of accepted answers.
    /// </summary>
    Answers = 4
}