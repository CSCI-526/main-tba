using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Bank : MonoBehaviour
{
    //card data for all cards in bank
    public List<CardData> bankData = new List<CardData>();

    public Card.CardSuit color = Card.CardSuit.empty;
    public Card.CardValue partType = Card.CardValue.empty;

    //text listing all of the cards
    public TMP_Text bankText;

    public bool AddToBank(CardData cd)
    {
        if (isValidAddition(cd)) {
            bankData.Add(cd);
            UpdateBankText();
            return true;
        }
        else
        {
            Debug.Log("Card is invalid!");
            return false;
        }
    }

    public void UpdateBankText()
    {
        string t = "Workbench:\n";
        for(int i = 0; i < bankData.Count; i++)
        {
            t += bankData[i].getCardString() + "\n";
        }
        bankText.text = t;
    }

    public bool isValidAddition(CardData cd)
    {
        //if empty then it has to be valid
        if (bankData.Count == 0)
        {
            partType = cd.cardValue;
            color = cd.cardSuit;
            return true;
        }
        //if there's one or more parts
        else
        {
            //if it doesn't match parttype or color it's invalid
            if (cd.cardSuit != color && cd.cardValue != partType)
            {
                return false;
            }
            //if it doesn't match the color, then we're not building based on color
            if (cd.cardSuit != color)
            {
                color = Card.CardSuit.empty;
            }
            if (cd.cardValue != partType)
            {
                partType = Card.CardValue.empty;
            }
            return true;

        }
    }

    public void OnMouseDown()
    {
        sellWorkBench(bankData);
    }

    public void sellWorkBench(List<CardData> sellData)
    {
        // Score tables for selling robots and heaps 
        //   - key is the number of parts
        //   - value is the points to be awarded 
        Dictionary<int, int> robotScoreTable = new()
        {
            {2, 2},
            {3, 4},
            {4, 8},
            {5, 14}
        };
        Dictionary<int, int> heapScoreTable = new()
        {
            {2, 2}, 
            {3, 2},
            {4, 4},
            {5, 4},
            {6, 8},
            {7, 8},
            {8, 11},
            {9, 11},
            {10, 15}
        };

        if (sellData.Count > 1)
        {
            // The checks here only check from the first two cards inside the workbench; 
            // This is (probably?) sufficient considering validity checks were done by the workbench class 
            // already when cards were being added
            if (sellData[0].cardSuit == sellData[1].cardSuit)
            {
                // this workbench contains a robot 
                Debug.Log("Selling robot...");
                Debug.Log("Awarding " + robotScoreTable[sellData.Count] + " points to Player " + GameplayManager.Instance.activePlayer.playerNum);
                GameplayManager.Instance.AwardPoints(robotScoreTable[sellData.Count]);
                bankData.Clear();
                UpdateBankText();
                GameplayManager.Instance.UpdatePointsDisplay();
            }
            else if (sellData[0].cardValue == sellData[1].cardValue)
            {
                // this workbench contains a heap 
                Debug.Log("Selling heap...");
                Debug.Log("Awarding " + heapScoreTable[sellData.Count] + " points to Player " + GameplayManager.Instance.activePlayer.playerNum);
                GameplayManager.Instance.AwardPoints(heapScoreTable[sellData.Count]);
                bankData.Clear();
                UpdateBankText();
                GameplayManager.Instance.UpdatePointsDisplay();
            }
        }
        else
        {
            Debug.Log("This workbench can't be sold yet");
        }
    }
}
