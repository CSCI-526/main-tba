using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Linq;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;
//using static UnityEditor.Experimental.AssetDatabaseExperimental.AssetDatabaseCounters;

public class Bank : MonoBehaviour
{
    //card data for all cards in bank
    public List<CardData> bankData = new List<CardData>();

    public Card.CardSuit color = Card.CardSuit.empty;
    
    //partType no longer in use, deletable?
    public Card.CardValue partType = Card.CardValue.empty;
    //init an array of taken parts, this assumes only 5 distinct parts
    public bool[] takenParts = { false, false, false, false, false };

    //text listing all of the cards
    public TMP_Text bankText;

    // determine which player this workbench belongs to
    public int playerNumber;
    public GameObject select;

    // button to sell this workbench
    [SerializeField]
    public Button sellButton;
    [SerializeField]
    public TextMeshProUGUI sellButtonText;

    // for robot visual representations
    [SerializeField]
    public GameObject robotBody;
    [SerializeField]
    private GameObject robotHead;
    [SerializeField]
    private GameObject robotLArm;
    [SerializeField]
    private GameObject robotRArm;
    [SerializeField]
    private GameObject robotLLeg;
    [SerializeField]
    private GameObject robotRLeg;

    // for weapon visuals 
    [SerializeField]
    public GameObject headWeapon;
    [SerializeField]
    private GameObject lArmWeapon;
    [SerializeField]
    private GameObject rArmWeapon;
    [SerializeField]
    private GameObject lLegWeapon;
    [SerializeField]
    private GameObject rLegWeapon;

    private bool foot_ready_ = false;

    public bool isHeadAbilityActive = false;

    private int last_card_num_;

    // public variable to see if the bank is enabled or not
    public bool enabled = true;

    // audio files to play
    public AudioSource insertSound;
    public AudioSource sellSound;
    public AudioSource punchSound;
    public AudioSource footSound;
    public AudioSource headSound;

    void Start()
    {
        sellButton.onClick.AddListener(() => sellWorkBench(bankData));
        sellButton.gameObject.SetActive(false);

        robotBody.SetActive(false);
        robotHead.SetActive(false);
        robotLArm.SetActive(false);
        robotRArm.SetActive(false);
        robotLLeg.SetActive(false);
        robotRLeg.SetActive(false);

        headWeapon.SetActive(false);
        lArmWeapon.SetActive(false);
        rArmWeapon.SetActive(false);
        lLegWeapon.SetActive(false);
        rLegWeapon.SetActive(false);

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
                // sellButtonText.text = "USE";
            }
            else
            {
                foot_ready_ = false;
                // sellButtonText.text = "SELL";
            }
        }

        if (GameplayManager.Instance.activePlayer.playerNum == playerNumber && bankData.Count >= 2)
        {
            if (SceneManager.GetActiveScene().name != "TutorialScene")
            {
                sellButton.gameObject.SetActive(true);
            }
        }
        else
        {
            sellButton.gameObject.SetActive(false);
        }
    }

    public bool AddToBank(CardData cd)
    {
        if (isValidAddition(cd) && enabled) {
            GameplayManager.Instance.MoveCardToWB(transform.position, this, cd);

            return true;
        }
        else
        {
            GameplayManager.Instance.msg.text = "Invalid Add!";
            StartCoroutine(RemoveAfterDelay(3f));
            Debug.Log("Card is invalid!");
            return false;
        }
    }

    public void AddToWBTutorial(CardData cd)
    {
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
        Debug.Log("DEBUG: Added " + cd.cardValue + " " + cd.cardSuit);

        drawRobot(cd);

        UpdateBankText();
    }
    public void AddToWB(CardData cd)
    {
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
        Debug.Log("DEBUG: Added " + cd.cardValue + " " + cd.cardSuit);

        insertSound.Play(0);
        drawRobot(cd);

        UpdateBankText();

        // activate body when all parts are present
        if (takenParts.All(b => b))
        {
            // Debug.Log("DEBUG: this bench has a complete robot!");
            robotBody.SetActive(true);
            robotBody.GetComponent<SpriteRenderer>().color = getColor(cd);
        }

        // destroy other selects first
        GameObject[] selectInstances = GameObject.FindGameObjectsWithTag("SelectPrefab");
        foreach (GameObject instance in selectInstances)
        {
            Destroy(instance);
        }
        // clear selected_cards first then add this to selected_cards
        GameplayManager.Instance.ClearSelectedCards();

        //delete the card from the river
        GameplayManager.Instance.RemoveCardFromRiver(cd);

        if (SceneManager.GetActiveScene().name != "TutorialScene")
        {
            //decrement turns in round
            GameplayManager.Instance.decrementActionsTaken();
        }

        //don't increment active player if we're in the beginning stages of the tutorial
        if (SceneManager.GetActiveScene().name != "TutorialScene")
        {
            //Incrememt the turn player since adding to the bench is a turn
            GameplayManager.Instance.IncrementActivePlayer();
        } else if (!GameplayManager.Instance.pastFirstTutorial)
        {
            //Don't increment, print out a nice job message
            StartCoroutine(GameplayManager.Instance.TriggerNextTutorial());
        } else if (GameplayManager.Instance.onFinalTutorial)
        {
            Debug.Log(this.name);
            if(this.name == "P1 Workbench 1")
            {
                if (!GameplayManager.Instance.onWeaponTut)
                {
                    GameplayManager.Instance.p1Work1SellButton.SetActive(true);

                }
                else
                {
                    StartCoroutine(GameplayManager.Instance.FailedWeaponTutMessages());
                }
        
            } else if (this.name == "P1 Workbench 2")
            {
                if (!GameplayManager.Instance.onWeaponTut)
                {
                    GameplayManager.Instance.MadeWrongChoice = true;
                    StartCoroutine(GameplayManager.Instance.TriggerNextTutorial());
                }
                else
                {
                    GameplayManager.Instance.p1Work2SellButton.SetActive(true);
                }
                
            }
        }


        if (bankData.Count >= 2)
        {
            if (SceneManager.GetActiveScene().name != "TutorialScene")
            {
                sellButton.gameObject.SetActive(true);
            }
            if (bankData[0].cardValue == bankData[1].cardValue)
            {
                sellButtonText.text = "USE";
            }
            else if (bankData[0].cardSuit == bankData[1].cardSuit)
            {
                sellButtonText.text = "SELL";
            }
        }
    }

    public void RemoveFromBank()
    {
        if (bankData.Count > 0)
        {
            //Get the card data before removal
            CardData cD = bankData[0];
            //Remove the first item from the bank
            bankData.RemoveAt(0);
            //Use card data to clean takenArray

            //This is causing arm bug
            //You should only set this to false if the bench is a robot build, not a set of duplicate parts
            if (bankData.Count > 1 && !hasOnlyOneType())
            {
                takenParts[(int)cD.cardValue - 1] = false;
            }

            //If we're down to our last card, set the color of the workbench, give option back to build robot
            //Need to set the takenParts as well just in case this was a dupe stack
            if(bankData.Count == 1)
            {
                color = bankData[0].cardSuit;
                takenParts[(int)bankData[0].cardValue - 1] = true;
            }

            //Update bank text to reflect change
            // this.UpdateBankText();
            removeSprite(cD);
        } else
        {
            Debug.Log("Can't remove bank empty");
        }
    }

    public void ClearBank()
    {
        while(bankData.Count != 0)
        {
            RemoveFromBank();
        }
        
    }

    public void UpdateBankText()
    {
        /* Commenting out, since we now have robot icons and no longer need text. 
        string t = "Workbench:\n";
        
        for(int i = 0; i < bankData.Count; i++)
        {
            t += bankData[i].getCardString() + "\n";
        }
        bankText.text = t;
        */
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
            Debug.Log("CHECKING VALIDATION: " + takenParts[0] + " " + takenParts[1] + " " + takenParts[2] + " " + takenParts[3] + " " + takenParts[4]);
            if (takenParts[(int)cd.cardValue - 1] == true && hasOnlyOneType())
            {
                return true;
            }

            return false;
        }
    }

    //Helper method to tell if a workbench only has one part type in it
    public bool hasOnlyOneType()
    {
        int counter = 0;
        for (int i = 0; i < this.takenParts.Length; i++)
        {
            if (this.takenParts[i] == true)
            {
                counter++;
            }
        }

        Debug.Log("CHECKING VALIDATION: " + counter);

        if(counter > 1)
        {
            return false;
        } else
        {
            return true;
        }
    }

    public void cleanupTakenParts()
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

            //Grab the card data from the selected card
            CardData selectedCardData = GameplayManager.Instance.selected_cards[0].GetCardData();

            if (AddToBank(selectedCardData))
            {

            }
            else
            {
                Debug.Log("Shake");
                GameplayManager.Instance.ShakeCard();
            }
        }
    }

    public bool sellWorkBench(List<CardData> sellData)
    {
        if (SceneManager.GetActiveScene().name == "TutorialScene")
        {
            if (!GameplayManager.Instance.onWeaponTut)
            {
                GameplayManager.Instance.RunWeaponTut();
                return false;

            }
            else
            {
                StartCoroutine(GameplayManager.Instance.EndTutorial());
                return false;
            }
        }
        else
        {
            // Score tables for selling robots and heaps 
            //   - key is the number of parts
            //   - value is the points to be awarded 
            int numRobotParts = 0;
            int numHeapParts = 0;

            Dictionary<int, int> robotScoreTable = new()
            {
                {2, 3},
                {3, 5},
                {4, 8},
                {5, 13}
            };

            if (sellData.Count > 1)
            {
                // The checks here only check from the first two cards inside the workbench; 
                // This is (probably?) sufficient considering validity checks were done by the workbench class 
                // already when cards were being added
                if (sellData[0].cardValue == sellData[1].cardValue)
                {
                    // this workbench contains a heap 
                    Debug.Log("Selling heap...");
                    Debug.Log(sellData[0].cardValue);
                    Debug.Log(sellData[1].cardValue);

                    //head ability check
                    Card.CardValue abilityValue = CheckAbility();

                    //send cardValue to switch statement that activates the relevant ability
                    //There has to be a more elegant way to do this but for now this is the way
                    switch (abilityValue)
                    {
                        case Card.CardValue.Head:
                            Debug.Log("Activating head ability, copying bench");
                            //activate head ability
                            isHeadAbilityActive = true;
                        
                            int benchNum = 0;
                            if(gameObject.name == "P1 Workbench 1" || gameObject.name == "P2 Workbench 1")
                            {
                                benchNum = 1;
                            }
                            else if (gameObject.name == "P1 Workbench 2" || gameObject.name == "P2 Workbench 2")
                            {
                                benchNum = 2;
                            }
                            // Debug.Log("BENCH NUM OF HEAD: " + benchNum);

                            GameplayManager.Instance.head_ability.Activate(sellData.Count, this, benchNum);
                            headSound.Play(0);
                            StartCoroutine(RemoveAfterDelay(3f));
                            AnalyticsManager.Instance.LogWorkbenchSale(bankData, 0);
                            // cleanUpBench();
                            return true;
                        case Card.CardValue.LeftArm:
                            Debug.Log("Activating left arm ability, destroy left bench");
                            GameplayManager.Instance.arm_ability.setLeft(true);
                            GameplayManager.Instance.arm_ability.Activate(sellData.Count, this);
                            punchSound.Play(0);
                            AnalyticsManager.Instance.LogWorkbenchSale(bankData, 0);
                            cleanUpBench();
                            //activate ability
                            StartCoroutine(RemoveAfterDelay(3f));
                            return true;
                        case Card.CardValue.RightArm:
                            Debug.Log("Activating right arm ability, destroy right bench");
                            GameplayManager.Instance.arm_ability.setLeft(false);
                            GameplayManager.Instance.arm_ability.Activate(sellData.Count, this);
                            punchSound.Play(0);
                            AnalyticsManager.Instance.LogWorkbenchSale(bankData, 0);
                            cleanUpBench();
                            //activate ability
                            StartCoroutine(RemoveAfterDelay(3f));
                            return true;
                        //TODO make this better or add the feet cases, right now I just break out of the 
                        //switch to do the feet conditions
                        default:
                            Debug.Log("Probably leg ability activated");
                            break;
                    }

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
                        footSound.Play(0);
                        StartCoroutine(RemoveAfterDelay(3f));
                    }
                
                    GameplayManager.Instance.AwardPoints(point_award);
                
                    //Analytics 
                    AnalyticsManager.Instance.LogWorkbenchSale(bankData, point_award);
                
                    cleanUpBench();
                }
                else if (sellData[0].cardSuit == sellData[1].cardSuit)
                {
                    // this workbench contains a robot 
                    Debug.Log("Selling robot...");
                    sellSound.Play(0);

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

                    cleanUpBench();
                }
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
    }
    
    IEnumerator RemoveAfterDelay(float seconds)
    {
        yield return new WaitForSeconds(seconds);
        GameplayManager.Instance.msg.text = "";
    }

    private void OnMouseEnter()
    {
        string msg = string.Empty;
        /* FOR TESTING; feel free to ignore
        msg += "COUNT: " + bankData.Count;
        msg += "\nCOLOR: " + color;
        TooltipManager._instance.SetAndShowTooltip(msg);
        */

        // Show Workbench info when mouse hover and no cards selected 
        if (GameplayManager.Instance.selected_cards.Count == 0)
        {
            
            if (bankData.Count < 2)
            {
                msg += "Collect more parts to build a robot or a weapon!";
                /*if (bankData.Count == 1)
                {
                    msg += "\nCurrent Part:\n" + bankData[0].cardSuit + " " + bankData[0].cardValue;
                }*/
            }
            else
            {
                if (bankData[0].cardValue == bankData[1].cardValue)
                {
                    switch ((int)bankData[0].cardValue)
                    {
                        case 1:
                        msg += "Copy up to " + (bankData.Count) + " parts from opposite workbench.";
                        break;

                        case 2:
                        msg += "Destroy " + (bankData.Count - 1) +  " parts from opponent's LEFT workbench.";
                        break;

                        case 3:
                        msg += "Destroy " + (bankData.Count - 1) + " parts from opponent's RIGHT workbench.";
                        break;

                        case 4:
                        msg += "Destroy parts on the LEFT side of the conveyor belt. Gain " + bankData.Count + " points per part destroyed.";
                        break;

                        case 5:
                        msg += "Destroy parts on the RIGHT side of the conveyor belt. Gain " + bankData.Count + " points per part destroyed.";
                        break;
                    }
                }

                else if (bankData[0].cardSuit == bankData[1].cardSuit)
                {
                    if (bankData.Count < 5)
                    {
                        msg += "Building " + bankData[0].cardSuit + " robot...\n" + bankData.Count + " parts so far";
                        /*for (int i = 0; i < bankData.Count; i++)
                        {
                            msg += "\n" + bankData[i].cardValue;
                        }*/
                    }
                    else if (bankData.Count == 5)
                    {
                        msg += bankData[0].cardSuit + " robot complete!";
                    }
                    
                }
            }
            TooltipManager._instance.SetAndShowTooltip(msg);
        } else
        {
            Debug.Log("got here with 1 selected card");
            //Gross tutorial only code
            if (SceneManager.GetActiveScene().name == "TutorialScene")
            {
                msg += "Click a workbench to collect selected part.";
                TooltipManager._instance.SetAndShowTooltip(msg);
            }
        }
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

    //more general method to check which ability
    //just return the cardvalue of the first card in the bench
    private Card.CardValue CheckAbility()
    {
        return bankData[0].cardValue;
    }

    public void cleanUpBench()
    {
        bankData.Clear();
        UpdateBankText();
        //decrement turns in round
        GameplayManager.Instance.decrementActionsTaken();
        GameplayManager.Instance.IncrementActivePlayer();
        
        sellButton.gameObject.SetActive(false);

        // remove all sprites
        robotBody.SetActive(false);
        robotHead.SetActive(false);
        robotLArm.SetActive(false);
        robotRArm.SetActive(false);
        robotLLeg.SetActive(false);
        robotRLeg.SetActive(false);
        headWeapon.SetActive(false);
        lArmWeapon.SetActive(false);
        rArmWeapon.SetActive(false);
        lLegWeapon.SetActive(false);
        rLegWeapon.SetActive(false);

        //cleanup operations on the workbench
        this.color = Card.CardSuit.empty;
        cleanupTakenParts();
    }

    public void drawBench()
    {
        foreach (CardData card in bankData)
        {
            drawRobot(card);
        }
    }

    public void drawRobot(CardData cd)
    /*
        Method to draw new robot part on bench
        Takes in CardData of added part, sets the corresponding sprite to active, and changes its color accordingly
    */
    {
        if (bankData.Count == 1 || (bankData[0].cardSuit == bankData[1].cardSuit && bankData[0].cardValue != bankData[1].cardValue))
        {
            switch ((int)cd.cardValue)
            {
                case 1:
                robotHead.SetActive(true);
                robotHead.GetComponent<SpriteRenderer>().color = getColor(cd);
                break;

                case 2:
                robotLArm.SetActive(true);
                robotLArm.GetComponent<SpriteRenderer>().color = getColor(cd);
                break;

                case 3:
                robotRArm.SetActive(true);
                robotRArm.GetComponent<SpriteRenderer>().color = getColor(cd);
                break;

                case 4:
                robotLLeg.SetActive(true);
                robotLLeg.GetComponent<SpriteRenderer>().color = getColor(cd);
                break;

                case 5:
                robotRLeg.SetActive(true);
                robotRLeg.GetComponent<SpriteRenderer>().color = getColor(cd);
                break;
            }
        }
        else if (bankData.Count != 1 && bankData[0].cardValue == bankData[1].cardValue)
        {
            switch ((int) cd.cardValue)
            {
                case 1:
                robotHead.SetActive(false);
                headWeapon.SetActive(true);
                break;

                case 2:
                robotLArm.SetActive(false);
                lArmWeapon.SetActive(true);
                break;

                case 3:
                robotRArm.SetActive(false);
                rArmWeapon.SetActive(true);
                break;

                case 4:
                robotLLeg.SetActive(false);
                lLegWeapon.SetActive(true);
                break;

                case 5:
                robotRLeg.SetActive(false);
                rLegWeapon.SetActive(true);
                break;
            }
        }
    }

    public Color getColor(CardData cd)
    /*
        Helper method for drawRobot 
        Implemented mostly to avoid nested switch statements for better clarity 
    */
    {
        switch ((int) cd.cardSuit)
        {
            case 0:
            return new Color(0f, 0f, 0f);

            case 1:
            return new Color(0.1804f, 0.2745f, 0.8314f);

            case 2:
            return new Color(0.9451f, 0.0627f, 0.0627f);

            case 3:
            return new Color(0.1098f,0.79611f, 0.1882f);

            case 4:
            return new Color(0.9412f, 0.9412f, 0.0902f);
        }
        return new Color(1f, 1f, 1f);
    }

    private void removeSprite(CardData cd)
    {
        switch ((int)cd.cardValue)
        {
            case 1:
            robotHead.SetActive(false);
            break;

            case 2:
            robotLArm.SetActive(false);
            break;

            case 3:
            robotRArm.SetActive(false);
            break;

            case 4:
            robotLLeg.SetActive(false);
            break;

            case 5:
            robotRLeg.SetActive(false);
            break;
        }

        if (this.hasOnlyOneType() && this.bankData.Count <= 1)
        {
            switch ((int)cd.cardValue)
            {
                case 1:
                headWeapon.SetActive(false);
                break;

                case 2:
                lArmWeapon.SetActive(false);
                break;

                case 3:
                rArmWeapon.SetActive(false);
                break;

                case 4:
                lLegWeapon.SetActive(false);
                break;

                case 5:
                rLegWeapon.SetActive(false);
                break;
            }
        }
    }
}
