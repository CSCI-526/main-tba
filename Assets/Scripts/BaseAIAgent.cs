using UnityEngine;

public abstract class BaseAIAgent
{
    public string botName;
    public abstract TurnAction TakeTurn(GameState gameState);
}
