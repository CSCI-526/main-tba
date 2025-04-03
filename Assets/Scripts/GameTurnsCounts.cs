using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameTurnsCounts
{
    //for the analytics manager
    public int totalTurns;
    public int totalPassTurns;
    public string timestamp;

    public GameTurnsCounts(int totalTurns, int totalPassTurns)
    {
        this.totalTurns = totalTurns;
        this.totalPassTurns = totalPassTurns;
        this.timestamp = System.DateTime.UtcNow.ToString();
    }
}
