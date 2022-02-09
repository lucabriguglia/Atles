namespace Atles.Domain.Models;

/// <summary>
/// Type of user level
/// </summary>
public enum UserLevelCountType
{
    /// <summary>
    /// Number of topics.
    /// </summary>
    Topics = 1,

    /// <summary>
    /// Number of replies.
    /// </summary>
    Replies = 2,

    /// <summary>
    /// Number of accepted answers.
    /// </summary>
    Answers = 3,

    /// <summary>
    /// Number of total posts (topics and replies).
    /// </summary>
    Posts = 4
}