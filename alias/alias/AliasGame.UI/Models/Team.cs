namespace Alias.Models;

using System;
using System.Collections.Generic;

public class Team
{
    public string Id { get; set; } = Guid.NewGuid().ToString();
    public List<Player> Players { get; set; } = new List<Player>();
    public int Score { get; set; }
    public string Name { get; set; }
} 