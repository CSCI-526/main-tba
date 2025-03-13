using System.Collections;
using System.Collections.Generic;
using System.Net;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

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

    // determine which player this workbench belongs to
    public int playerNumber;
    public GameObject select;

    // button to sell this workbench
    [SerializeField]
    private Button sellButton;
    [SerializeField]
    private TextMeshProUGUI sellButtonText;

    private bool foot_ready_ = false;

    private int last_card_num_;

    void Start()
    {
        sellButton.onClick.AddListener(() => sellWorkBench(bankData));
        sellButton.interactable = false;
    }

    void Update()
    {
        if (last_card_num_ != bankData.Count)
        {
            last_card_num_ = bankData.Count;
            int res = CheckFootAbilityCondition();
            if (res != 0)
            {
                if (res == 1) GameplayManager.Instance.foot_ability.setLeft(true);
                else if (res == 2) GameplayManager.Instance.foot_ability.setLeft(false);
                GameplayManager.Instance.foot_ability.duplicate_count_ = bankData.Count;
                foot_ready_ = true;
                sellButtonText.text = "USE";
            }
            else
            {
                foot_ready_ = false;
                sellButtonText.text = "SELL";
            }
        }

        if (GameplayManager.Instance.activePlayer.playerNum == playerNumber && bankData.Count >= 2)
        {
            sellButton.interactable = true;
        }
        else
        {
            sellButton.interactable = false;
        }
    }

    public bool AddToBank(CardData cd)
    {
        if (isValidAddition(cd)) {
            takenParts[(int)cd.cardValue - 1] = true;
            if (bankData.Count == 0)
            {
                //color is set for the bank at this point
                color = cd.cardSuit;
            }
            else if (hasOnlyOneType())
            {
                // this means the build should only be parts, ignore color
                color = Card.CardSuit.empty;
            }
            bankData.Add(cd);
            UpdateBankText();
            return true;
        }
        else
        {
            GameplayManager.Instance.msg.text = "Invalid Add!";
            StartCoroutine(RemoveAfterDelay(2f));
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
                    return true;
                } 
            }

            //2) is this part type already in the bench AND there is only one type in the bench?
            if(takenParts[(int)cd.cardValue - 1] == true && hasOnlyOneType())
            {
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
                    if(bankData[0].cardSuit == bankData[1].cardSuit)
                    {
                        sellButtonText.text = "SELL";
                    }
                    else if(bankData[0].cardValue == bankData[1].cardValue)
                    {
                        sellButtonText.text = "USE";
                    }
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
        /*Dictionary<int, int> heapScoreTable = new()
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
        };*/

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

                GameplayManager.Instance.msg.text = "Awarding " + robotScoreTable[numRobotParts] + " points to Player " + GameplayManager.Instance.activePlayer.playerNum;
                StartCoroutine(RemoveAfterDelay(2f));
                
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

                int point_award = 0;
                if (foot_ready_)
                {
                    point_award = GameplayManager.Instance.foot_ability.Activate();
                    StartCoroutine(RemoveAfterDelay(3f));
                }
                
                GameplayManager.Instance.msg.text = "Awarding " + point_award + " points to Player " + GameplayManager.Instance.activePlayer.playerNum;
                StartCoroutine(RemoveAfterDelay(2f));
                
                //Analytics 
                AnalyticsManager.Instance.LogWorkbenchSale(bankData, point_award);
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
            GameplayManager.Instance.msg.text = "This workbench can't be sold yet";
            StartCoroutine(RemoveAfterDelay(3f));
            Debug.Log("This workbench can't be sold yet");
            return false;
        }
    }
    
    IEnumerator RemoveAfterDelay(float seconds)
    {
        yield return new WaitForSeconds(seconds);
        GameplayManager.Instance.msg.text = "";
    }

    private void OnMouseEnter()
    {
        string msg = string.Empty;
        // Show Workbench info when mouse hover and no cards selected 
        if (GameplayManager.Instance.selected_cards.Count == 0)
        {
            if (bankData.Count < 2)
            {
                msg += "Collect more parts to build a robot or a weapon!";
            }
            else
            {
                if (bankData[0].cardValue == bankData[1].cardValue)
                {
                    switch ((int)bankData[0].cardValue)
                    {
                        case 1:
                        msg += "Peek at the next cards on conveyor belt.\nCurrent power: " + bankData.Count;
                        break;

                        case 2:
                        msg += "Destroy parts from opponent's LEFT workbench.\nCurrent power: " + bankData.Count;
                        break;

                        case 3:
                        msg += "Destroy parts from opponent's RIGHT workbench.\nCurrent power: " + bankData.Count;
                        break;

                        case 4:
                        msg += "Collect points based on cards remaining on LEFT half of conveyor belt.\nCurrent power: " + bankData.Count;
                        break;

                        case 5:
                        msg += "Collect points based on cards remaining on RIGHT half of conveyor belt.\nCurrent power: " + bankData.Count;
                        break;
                    }
                }

                else if (bankData[0].cardSuit == bankData[1].cardSuit)
                {
                    if (bankData.Count < 5)
                    {
                        msg += "Building " + bankData[0].cardSuit + " robot...\n" + bankData.Count + " parts so far.";
                    }
                    else if (bankData.Count == 5)
                    {
                        msg += bankData[0].cardSuit + " robot complete!";
                    }
                    
                }
            }
            TooltipManager._instance.SetAndShowTooltip(msg);
        }
        /*
        else if(GameplayManager.Instance.selected_cards.Count == 1)
        {
            CardData selectedCardData = GameplayManager.Instance.selected_cards[0].GetCardData();
            if (isValidAddition(selectedCardData))
            {
                TooltipManager._instance.SetAndShowTooltip("Click to add selected card.");
            }
            else
            {
                TooltipManager._instance.SetAndShowTooltip("Selected card cannot be added to this workbench.");
            }
        }
        */
    }

    private void OnMouseExit()
    {
        TooltipManager._instance.HideTooltip();
    }

    // Method to spawn in selection ring around workbench given a valid placement
    public void spawnSelection(CardData cd)
    {
        if (GameplayManager.Instance.activePlayer.playerNum != playerNumber)
        {
            return;
        }
        if (!isValidAddition(cd))
        {
            return;
        }
        // Passed all boolean checks, show selected asset
        Quaternion setRotation = transform.rotation;
        setRotation.eulerAngles = new Vector3(0.0f, 0.0f, 90.0f);
        Instantiate(select, new Vector2(transform.position.x, transform.position.y), setRotation);
    }

    // return 1 for l, 2 for r, 0 for not met
    private int CheckFootAbilityCondition()
    {
        if (hasOnlyOneType() && bankData.Count > 1)
        {
            if (bankData[0].cardValue == Card.CardValue.LeftFoot)
            {
                return 1;
            }
            else if (bankData[0].cardValue == Card.CardValue.RightFoot)
            {
                return 2;
            }
        }

        return 0;
    }
}
