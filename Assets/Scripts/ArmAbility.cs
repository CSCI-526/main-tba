using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class ArmAbility : IAbility
{
    private bool isLeft = false;
    //Handles the arm ability
    //If it's a left arm stack take from the left workbench (duplicateCount - 1 cards)
    public void Activate(int duplicateCount, Bank workBench)
    {
        //Need to get the other players workbench
        //Workbench1 for both players is left
        //Workbench2 for both players is right

        //Workbench parameter is the calling workbench
        //workbench has a playerNumber
        //gameplaymanager has an activeplayer check if player num == activeplayer
        if (GameplayManager.Instance.activePlayer.playerNum == workBench.playerNumber)
        {
            //Get the other player
            //This assumes 2 players only
            //player 1 playerNum is 1 p2 is 2
            int otherPlayerNumber = 3 - workBench.playerNumber;

            //But in the playerlist p1 is at the 0th index and p2 is at the 1st index :(
            GameplayManager.GamePlayer otherPlayer = GameplayManager.Instance.playerList[otherPlayerNumber - 1];

            int numCardsToDelete = duplicateCount - 1;
            //We have the other player, now delete some cards from their workbench
            if (isLeft)
            {
                //delete from workbench1
                for (int i = 0; i < numCardsToDelete; i++) {
                    //Remove from bank takes care of the nonsense, updating bank ui, taken list, etc
                    otherPlayer.WB1.RemoveFromBank();
                }

                if (otherPlayer.WB1.bankData.Count == 1)
                {
                    otherPlayer.WB1.drawRobot(otherPlayer.WB1.bankData[0]);
                }
            } else
            {
                //delete from workbench2
                for (int i = 0; i < numCardsToDelete; i++)
                {
                    //Remove from bank takes care of the nonsense, updating bank ui, taken list, etc
                    otherPlayer.WB2.RemoveFromBank();
                }
                
                if (otherPlayer.WB2.bankData.Count == 1)
                {
                    otherPlayer.WB2.drawRobot(otherPlayer.WB2.bankData[0]);
                }
            }
        }
    }

    //Sets isLeft variable for the ability
    public void setLeft(bool left)
    {
        this.isLeft = left;
    }
}
