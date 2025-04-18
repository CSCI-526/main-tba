using System.Collections.Generic;
using UnityEngine;

public class GameState
{
    //To be used entirely by the AI
    //Represents all game state info the AI needs to make a decision

    public List<CardData> riverCards;
    public List<CardData> aiLeftBank;
    public List<CardData> aiRightBank;
    public List<CardData> playerLeftBank;
    public List<CardData> playerRightBank;
    //remaining turns in round
    public int remainingTurns;
    public int aiScore;
    public int playerScore;

    public GameState(List<CardData> riverCards, List<CardData> aiLeftBank, List<CardData> aiRightBank, 
        List<CardData> playerLeftBank, List<CardData> playerRightBank, int remainingTurns, int aiScore, int playerScore)
    {
        this.riverCards = riverCards;
        this.aiLeftBank = aiLeftBank;
        this.aiRightBank = aiRightBank;
        this.playerLeftBank = playerLeftBank;
        this.playerRightBank = playerRightBank;
        this.remainingTurns = remainingTurns;
        this.aiScore = aiScore;
        this.playerScore = playerScore;
    }
}
