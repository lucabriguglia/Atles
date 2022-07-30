namespace Atles.Domain;

/// <summary>
/// Type of subscription
/// </summary>
public enum SubscriptionType
{
    /// <summary>
    /// User will be notified for every post in the category.
    /// </summary>
    Category = 1,

    /// <summary>
    /// User will be notified for every post in the forum.
    /// </summary>
    Forum = 2,

    /// <summary>
    /// User will be notified for every reply in the topic.
    /// </summary>
    Topic = 3
}