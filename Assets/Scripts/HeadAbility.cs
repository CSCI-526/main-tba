using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HeadAbility : IAbility
{
    // Handles the head ability
    // Copies n-1 cards of the opponent's bench that it "looks at" 

    public void Activate(int duplicateCount, Bank workBench)
    // This is just here to avoid the error when we don't inherit this apparently?
    {
        return;
    }

    public void Activate(int duplicateCount, Bank workBench, int bankNum)
    // Overriden Activate, since Head ability also needs an int variable to identify if it's left or right
    {
        Bank bankToCopy = null;

        // Based on activePlayer at the time of activation, determine the bank to copy from!
        if (GameplayManager.Instance.activePlayer.playerNum == 1)
        {
            GameplayManager.Instance.playerList[1].WB1.GetComponent<Bank>().enabled = true;
            GameplayManager.Instance.playerList[1].WB2.GetComponent<Bank>().enabled = true;

            Debug.Log("P2 Benches enabled? " + GameplayManager.Instance.playerList[1].WB1.GetComponent<Bank>().enabled + " and " + 
            GameplayManager.Instance.playerList[1].WB2.GetComponent<Bank>().enabled);

            if (bankNum == 1)
            {
                bankToCopy = GameplayManager.Instance.p2firstWB;
            }
            else if (bankNum == 2)
            {
                bankToCopy = GameplayManager.Instance.p2secondWB;
            }
        }
        else if (GameplayManager.Instance.activePlayer.playerNum == 2)
        {
            GameplayManager.Instance.playerList[0].WB1.GetComponent<Bank>().enabled = true;
            GameplayManager.Instance.playerList[0].WB2.GetComponent<Bank>().enabled = true;

            Debug.Log("P1 Benches enabled? " + GameplayManager.Instance.playerList[0].WB1.GetComponent<Bank>().enabled + " and " + 
            GameplayManager.Instance.playerList[0].WB2.GetComponent<Bank>().enabled);

            if (bankNum == 1)
            {
                bankToCopy = GameplayManager.Instance.p1firstWB;
            }
            else if (bankNum == 2)
            {
                bankToCopy = GameplayManager.Instance.p1secondWB;
            }
        }

        // workBench.cleanUpBench();
        workBench.bankData.Clear();
        workBench.headWeapon.SetActive(false);
        workBench.cleanupTakenParts();
        workBench.color = Card.CardSuit.empty;
        
        if (bankToCopy.enabled)
        {
            for (int i = 0; i < duplicateCount-1; i++)
            {
                
                if (i < bankToCopy.bankData.Count)
                {
                    Debug.Log("Copying card number " + (i+1));
                    Debug.Log("Own bench enabled? " + workBench.enabled);
                    workBench.AddToWB(bankToCopy.bankData[i]);
                }
            }
            GameplayManager.Instance.IncrementActivePlayer();
        }
        else
        {
            Debug.Log("BANK TO COPY NOT ENABLED!");
        }
        

        if (GameplayManager.Instance.activePlayer.playerNum == 1)
        {
            GameplayManager.Instance.playerList[1].WB1.GetComponent<Bank>().enabled = false;
            GameplayManager.Instance.playerList[1].WB2.GetComponent<Bank>().enabled = false;
        }

        else if (GameplayManager.Instance.activePlayer.playerNum == 2)
        {
            GameplayManager.Instance.playerList[0].WB1.GetComponent<Bank>().enabled = false;
            GameplayManager.Instance.playerList[0].WB2.GetComponent<Bank>().enabled = false;
        }

        if (workBench.bankData.Count >= 2)
        {
            if(workBench.bankData[0].cardValue == workBench.bankData[1].cardValue)
            {
                workBench.sellButtonText.text = "USE";
            }
            else if(workBench.bankData[0].cardSuit == workBench.bankData[1].cardSuit)
            {
                workBench.sellButtonText.text = "SELL";
            }
        }
    }
}
