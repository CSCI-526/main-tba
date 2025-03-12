using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class Bank : MonoBehaviour
{
    //card data for all cards in bank
    public List<CardData> bankData = new List<CardData>();

    public Card.CardSuit color = Card.CardSuit.empty;
    
    //partType no longer in use, deletable?
    public Card.CardValue partType = Card.CardValue.empty;
    //init an array of taken parts, this assumes only 5 distinct parts
    private bool[] takenParts = { false, false, false, false, false };

    //text listing all of the cards
    public TMP_Text bankText;

    // button to sell this workbench
    [SerializeField]
    private Button sellButton;

    void Start()
    {
        sellButton.onClick.AddListener(() => sellWorkBench(bankData));
        sellButton.interactable = false;
    }

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
        if (bankData.Count == 0)
        {
            takenParts[(int)cd.cardValue - 1] = true;
            //color is set for the bank at this point
            color = cd.cardSuit;
            return true;
        } else
        {
            //what if this isn't the first card in the bench
            //1) does it match the set color of the bench
            if(cd.cardSuit == color)
            {
                //is this a new robot part?
                if(takenParts[(int)cd.cardValue - 1] == false)
                {
                    takenParts[(int)cd.cardValue - 1] = true;
                    return true;
                } 
            }

            //2) is this part type already in the bench AND there is only one type in the bench?
            if(takenParts[(int)cd.cardValue - 1] == true && hasOnlyOneType())
            {
                //turn off the color now, we're dupe parts only
                color = Card.CardSuit.empty;
                return true;
            }

            return false;
        }
    }

    //Helper method to tell if a workbench only has one part type in it
    private bool hasOnlyOneType()
    {
        int counter = 0;
        for (int i = 0; i < this.takenParts.Length; i++)
        {
            if (this.takenParts[i] == true)
            {
                counter++;
            }
        }

        if(counter > 1)
        {
            return false;
        } else
        {
            return true;
        }
    }

    private void cleanupTakenParts()
    {
        for(int i = 0; i < this.takenParts.Length; i++)
        {
            this.takenParts[i] = false;
        }
    }

    public void OnMouseDown()
    {
        //Mousing down on a workbench means we either are adding a card to it, or selling the bench
        if (GameplayManager.Instance.selected_cards.Count == 1)
        {
            //There is a card that has been selected from the belt
            //Add it to the player's selected bench
            Debug.Log("Adding to selected bench");
            // destroy other selects first
            GameObject[] selectInstances = GameObject.FindGameObjectsWithTag("SelectPrefab");
            foreach (GameObject instance in selectInstances)
            {
                Destroy(instance);
            }

            //Grab the card data from the selected card
            CardData selectedCardData = GameplayManager.Instance.selected_cards[0].GetCardData();
            // clear selected_cards first then add this to selected_cards
            GameplayManager.Instance.ClearSelectedCards();

            

            if (AddToBank(selectedCardData))
            {
                //delete the card from the river
                GameplayManager.Instance.RemoveCardFromRiver(selectedCardData);

                //decrement turns in round
                GameplayManager.Instance.decrementActionsTaken();

                //Incrememt the turn player since adding to the bench is a turn
                GameplayManager.Instance.IncrementActivePlayer();

                if (bankData.Count >= 2)
                {
                    sellButton.interactable = true;
                }
            }
        }
    }

    public bool sellWorkBench(List<CardData> sellData)
    {
        // Score tables for selling robots and heaps 
        //   - key is the number of parts
        //   - value is the points to be awarded 
        int numRobotParts = 0;
        int numHeapParts = 0;

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

                numRobotParts = sellData.Count;
                // in current build, possible to have multiple of same body part, so adjust number 
                if (sellData.Count > 5)
                {
                    numRobotParts = 5;
                }

                Debug.Log("Awarding " + robotScoreTable[numRobotParts] + " points to Player " + GameplayManager.Instance.activePlayer.playerNum);
                GameplayManager.Instance.AwardPoints(robotScoreTable[numRobotParts]);

                //Analytics 
                AnalyticsManager.Instance.LogWorkbenchSale(bankData, robotScoreTable[numRobotParts]);
                bankData.Clear();
                UpdateBankText();
                GameplayManager.Instance.UpdatePointsDisplay();
                //decrement turns in round
                GameplayManager.Instance.decrementActionsTaken();
                GameplayManager.Instance.IncrementActivePlayer();
            }
            else if (sellData[0].cardValue == sellData[1].cardValue)
            {
                // this workbench contains a heap 
                Debug.Log("Selling heap...");

                numHeapParts = sellData.Count;
                // in current build (I believe) possible to have more than 10 parts in a heap, so adjust number
                if (sellData.Count > 10)
                {
                    numHeapParts = 10;
                }

                Debug.Log("Awarding " + heapScoreTable[numHeapParts] + " points to Player " + GameplayManager.Instance.activePlayer.playerNum);
                GameplayManager.Instance.AwardPoints(heapScoreTable[numHeapParts]);

                //Analytics 
                AnalyticsManager.Instance.LogWorkbenchSale(bankData, heapScoreTable[numHeapParts]);
                bankData.Clear();
                UpdateBankText();
                GameplayManager.Instance.UpdatePointsDisplay();
                //decrement turns in round
                GameplayManager.Instance.decrementActionsTaken();
                GameplayManager.Instance.IncrementActivePlayer();
            }
            sellButton.interactable = false;

            //cleanup operations on the workbench
            this.color = Card.CardSuit.empty;
            cleanupTakenParts();
            return true;
        }
        else
        {
            Debug.Log("This workbench can't be sold yet");
            return false;
        }
    }
}
