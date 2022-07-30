namespace Atles.Domain;

public enum UserRankRuleType
{
    /// <summary>
    /// The number of total posts (topics and replies).
    /// </summary>
    Posts = 1,

    /// <summary>
    /// The number of topics.
    /// </summary>
    Topics = 2,

    /// <summary>
    /// The number of replies.
    /// </summary>
    Replies = 3,

    /// <summary>
    /// The number of accepted answers.
    /// </summary>
    Answers = 4
}