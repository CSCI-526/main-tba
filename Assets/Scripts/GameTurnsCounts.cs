using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameTurnsCounts
{
    //for the analytics manager
    public int totalTurns;
    public int totalPassTurns;
    public int winnerPlayerNum;
    public string timestamp;

    public GameTurnsCounts(int totalTurns, int totalPassTurns, int winnerPlayerNum)
    {
        this.totalTurns = totalTurns;
        this.totalPassTurns = totalPassTurns;
        this.winnerPlayerNum = winnerPlayerNum;
        this.timestamp = System.DateTime.UtcNow.ToString();
    }
}
