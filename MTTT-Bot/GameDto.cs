using System;
using System.Collections.Generic;

namespace MTTT;

public class GameDto
{
    public List<Guid> PlayerIds { get; set; } = new();
    public Guid CurrentPlayerId { get; set; }
    public Guid GameId { get; set; }
    public List<string> PlayField { get; set; } = new();
    public char[] WinningField { get; set; } = new char[9];
    public int NextFieldIndicator { get; set; }
    public bool FreeMove { get; set; }
}