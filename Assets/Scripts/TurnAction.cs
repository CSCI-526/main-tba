using UnityEngine;

public class TurnAction
{
    //For the AI agent
    //You can do 3 things
    //Take a card from the river and place it in your workbench if possible
    //Sell a workbench
    //Pass the turn

    public int riverCardToTake;
    public int workBenchToPlaceCard;
    public bool sellLeftWorkbench;
    public bool sellRightWorkbench;
    public bool passTurn;

    public TurnAction(int riverCardToTake, int workBenchToPlaceCard, bool sellLeftWorkbench, 
        bool sellRightWorkbench, bool passTurn)
    {
        this.riverCardToTake = riverCardToTake;
        this.workBenchToPlaceCard = workBenchToPlaceCard;
        this.sellLeftWorkbench = sellLeftWorkbench;
        this.sellRightWorkbench = sellRightWorkbench;
        this.passTurn = passTurn;
    }
}
