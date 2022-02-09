﻿using System;
using System.Collections.Generic;

namespace Atles.Domain.Models;

/// <summary>
/// User Rank
/// </summary>
public class UserRank
{
    public Guid Id { get; private set; }
    public string Name { get; private set; }
    public int Order { get; private set; }
    public string Badge { get; private set; }

    public IReadOnlyCollection<UserLevel> UserLevels => _UserLevels;
    private readonly List<UserLevel> _UserLevels = new();

    public UserRank(string name, int order, string badge = null)
    {
        Id = Guid.NewGuid();
        Name = name;
        Order = order;
        Badge = badge;
    }
}