using System.Collections.Generic;
using System;
using UnityEngine;

public abstract class BaseAIAgent
{
    public string botName;

    public BaseAIAgent(string botName)
    {
        this.botName = botName;
    }

    //TakeTurn would return a TurnAction object that the gameplayManager could interpret as an action
    public abstract TurnAction TakeTurn(GameState gameState);

    //Method where given the gamestate info and a specific workbench, is there a valid river card to add?
    public virtual int FindValidBenchAddition(GameState gameState, Boolean leftBench)
    {
        List<CardData> bench;

        if (leftBench) {
            bench = gameState.aiLeftBank;
        } else
        {
            bench = gameState.aiRightBank;
        }

        foreach(CardData riverCard in gameState.riverCards)
        {
            //Could the card be added to the bench?
        }

        return -1;
    }
}
