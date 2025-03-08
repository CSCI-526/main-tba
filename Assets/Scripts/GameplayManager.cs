using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using static System.Net.Mime.MediaTypeNames;
using UnityEngine.SceneManagement;

public class GameplayManager : MonoBehaviour
{

    //Made GamePlayer to encapsulate the player objects in the gameplay manager
    //There is a Player class but I didn't want to break gameplaying
    public class GamePlayer
    {
        public int playerNum;
        public int score;
        public bool passed;
        public Bank WB1;
        public Bank WB2;

        public GamePlayer(int playerNum, Bank WB1, Bank WB2)
        {
            this.playerNum = playerNum;
            this.score = 0;
            this.passed = false;
            this.WB1 = WB1;
            this.WB2 = WB2;
        }
    }

    //objects that carry over from turn to turn
    public Deck deck;
    public River river;

    //p1 objects
    public Bank p1firstWB;
    public Bank p1secondWB;

    //p2 objects
    public Bank p2firstWB;
    public Bank p2secondWB;

    //Player list
    public GamePlayer[] playerList = new GamePlayer[2];
    public GamePlayer activePlayer;


    //UI elements
    [SerializeField]
    private GameObject gameCanvas;
    [SerializeField]
    private GameObject inGameTut1;
    [SerializeField]
    private GameObject inGameTut2;

    public TMP_Text instructionText;
    public TMP_Text currPlayerText;
    public TMP_Text p1ScoreText;
    public TMP_Text p2ScoreText;
    public Button next_button;
    public Button rules_button;
    public GameObject rules_panel;
    private bool rules_toggle = false;

    private int actionsTakenInRound = 4;
    
    // currectly selected cards
    public List<Card> selected_cards = new List<Card>();
    
    // ---------- Singleton Setup -----------
    public static GameplayManager Instance { get; private set; }

    private void Awake()
    {
        // If an Instance already exists and it’s not this, destroy this.
        // Otherwise, make this the Singleton instance.
        // outer class can refer to this single instance by GamePlayerManager.Instance
        if (Instance != null && Instance != this)
        {
            Destroy(this.gameObject);
        }
        else
        {
            Instance = this;
            // If you want the GameplayManager to persist across scenes:
            // DontDestroyOnLoad(this.gameObject);
        }
    }

    //initialize everything needed at the top of the game
    void Start()
    {
        inGameTut1.SetActive(false);
        inGameTut2.SetActive(false);
        InitializePlayers();
        InitializeGame();
        river.Flop(deck);
    }

    //control handler
    void Update()
    {
        //dev debug keys - h to hide hand, s to show hand
        if (Input.GetKeyDown(KeyCode.H))
        {
            activePlayer.WB1.bankText.gameObject.SetActive(false);
            activePlayer.WB2.bankText.gameObject.SetActive(false);
        }
        if (Input.GetKeyDown(KeyCode.S))
        {
            activePlayer.WB1.bankText.gameObject.SetActive(true);
            activePlayer.WB2.bankText.gameObject.SetActive(true);
        }

        //Debug testing setting active player
        if (Input.GetKeyDown(KeyCode.D))
        {
            int currPlayerNum = activePlayer.playerNum - 1;
            Debug.Log(currPlayerNum);
            if (currPlayerNum >= 0 && currPlayerNum < 2)
            {
                SetActivePlayer(currPlayerNum + 1);
            } else if (currPlayerNum == 2)
            {
                SetActivePlayer(0);
            }
        }

        //add to river - r
        if (Input.GetKeyDown(KeyCode.R))
        {
            river.addToRiver(deck);
        }

        //terminate game when any player reach score 30
        if (playerList[0].score >= 30){
            Winner.gameWinner = playerList[0].playerNum;
            SceneManager.LoadScene(2);
        }
        if (playerList[1].score >= 30){
            Winner.gameWinner = playerList[1].playerNum;
            SceneManager.LoadScene(2);
        }
    }

    //search the river and the active player's hand for a card and bank it
    public void LocateAndBank(CardData cd)
    {
        river.LocateAndBank(cd);
    }

    //Making this helper function to just remove a card from the river
    //Adding the card to the bank will be hanled by the OnButtonClick in the bank so user has
    //agency in adding card to which bank
    public bool RemoveCardFromRiver(CardData cd)
    {
        return river.LocateAndDelete(cd);
    }

    //Initialize players from the player objects given
    void InitializePlayers()
    {
        //Using GamePlayer and not Player. Can be changed but doing this to not break things that rely on player
        GamePlayer p1 = new GamePlayer(1, p1firstWB, p1secondWB);
        GamePlayer p2 = new GamePlayer(2, p2firstWB, p2secondWB);

        playerList[0] = p1;
        playerList[1] = p2;

        p1.WB1.UpdateBankText();
        p1.WB2.UpdateBankText();
        p2.WB1.UpdateBankText();
        p2.WB2.UpdateBankText();

        activePlayer = playerList[0];
    }

    //Init game
    void InitializeGame()
    {
        deck.InitializeDeck();
        deck.ShuffleDeck();

        //buttons
        next_button.gameObject.SetActive(true);
        rules_button.gameObject.SetActive(false);
        next_button.onClick.AddListener(() => OnButtonClick(1));
        // rules_button.onClick.AddListener(() => OnButtonClick(3));
        //withdraw_button.onClick.AddListener(() => OnButtonClick(3));

        //instructionText.gameObject.SetActive(false);
    }

    //Helper to switch to a different player
    //Show that player's hand, bank, and current chips
    void SetActivePlayer(int playerNum)
    {
        if (activePlayer != null)
        {
            // activePlayer.WB1.bankText.gameObject.SetActive(false);
            // activePlayer.WB2.bankText.gameObject.SetActive(false);
        }
        activePlayer = playerList[playerNum];
        activePlayer.WB1.UpdateBankText();
        activePlayer.WB2.UpdateBankText();
        currPlayerText.text = "Player" + activePlayer.playerNum + "'s turn";

        if (activePlayer.playerNum == 1)
        {
            // change location of active player indicator text 
            currPlayerText.rectTransform.anchoredPosition = new Vector2(-200, -150);

            // disable Collider component of inactive player so active player can only click their workbenches
            playerList[0].WB1.GetComponent<Collider2D>().enabled = true;
            playerList[0].WB2.GetComponent<Collider2D>().enabled = true;
            playerList[1].WB1.GetComponent<Collider2D>().enabled = false;
            playerList[1].WB2.GetComponent<Collider2D>().enabled = false;
            
        }
        else if (activePlayer.playerNum == 2)
        {
            // change location of active player indicator text
            currPlayerText.rectTransform.anchoredPosition = new Vector2(-200, 130);

            // change location of active player indicator text
            playerList[1].WB1.GetComponent<Collider2D>().enabled = true;
            playerList[1].WB2.GetComponent<Collider2D>().enabled = true;
            playerList[0].WB1.GetComponent<Collider2D>().enabled = false;
            playerList[0].WB2.GetComponent<Collider2D>().enabled = false;
        }
    }


    void OnButtonClick(int buttonID)
    {
        switch (buttonID)
        {
            case 1:
                Debug.Log("PASS button clicked!");
                activePlayer.passed = true;
                decrementActionsTaken();
                //CheckRefreshRiver();
                IncrementActivePlayer();
                break;

            case 3:
                Debug.Log("rules button clicked!");
                rules_toggle = !rules_toggle;
                // make other component active, change text
                // rules_button.GetComponentInChildren<TextMeshProUGUI>().text = rules_toggle ? "Hide" : "Rules";
                // rules_panel.SetActive(rules_toggle);
                
                // Work in progress!
                //InGameTutorialDisplay();

                break;

            default:
                Debug.Log("Unknown button clicked!");
                break;
        }
    }
    
    public void ClearSelectedCards()
    {
        selected_cards.Clear();
    }

    public void Pass()
    {
        // TODO: implement pass logic after turn implementation is done
    }

    public void SellCard(CardData cd)
    {
        Debug.Log("Sell" + cd);
        // TODO: implement interaction with score system
    }

    public void AwardPoints(int points)
    {
        activePlayer.score += points;
        Debug.Log("Player " + activePlayer.playerNum + " now has " + activePlayer.score + " points!");
    }

    public void IncrementActivePlayer()
    {
        // destroy other selects prefabs to reset selection for the turn.
        GameObject[] selectInstances = GameObject.FindGameObjectsWithTag("SelectPrefab");
        foreach (GameObject instance in selectInstances)
        {
            Destroy(instance);
        }
        if (activePlayer.playerNum == 2)
        {
            SetActivePlayer(0);
        }
        else
        {
            SetActivePlayer(activePlayer.playerNum);
        }
    }

    public void CheckRefreshRiver()
    {
        /*
        // If river has run dry: reflop river
        if (river.riverCards.Count == 0)
        {
            playerList[0].passed = false;
            playerList[1].passed = false;
            river.Flop(deck);
        }
        // OR if both players have passed: delete current river and reflop river
        // set the flags as false here too, before river.Flop(deck);
        // also when opponent has passed, opponent gets ONE more card? or keep going?
        if (playerList[0].passed && playerList[1].passed)
        {
            Debug.Log("Both players passed! New river...");
            while (river.riverCards.Count > 0)
            {
                GameObject toDestroy = river.riverCards[0];
                river.riverCards.RemoveAt(0);
                Destroy(toDestroy);
            }
            river.riverData.Clear();
            river.Flop(deck);
            playerList[0].passed = false;
            playerList[1].passed = false;
        }*/

        //River gets refreshed when actionsTakenInRound hits 0
        //Round is over both players have taken 2 actions each (taken a card, passed, or sold a workbench)
        Debug.Log("Actions in round taken: " + actionsTakenInRound);
        if(actionsTakenInRound == 0)
        {
            river.riverData.Clear();
            river.Flop(deck);
            actionsTakenInRound = 4;
        }
    }

    public void decrementActionsTaken()
    {
        if (actionsTakenInRound > 0)
        {
            actionsTakenInRound--;
        }

        if(actionsTakenInRound == 0)
        {
            //Time to refresh the river
            CheckRefreshRiver();
        }
    }
    public bool InactivePlayerPassed()
    {
        if (activePlayer.playerNum == 1)
        {
            return playerList[1].passed;
        }
        else 
        {
            return playerList[0].passed;
        }
    }

    public void UpdatePointsDisplay()
    {
        if (activePlayer.playerNum == 1)
        {
            p1ScoreText.text = "Score: " + activePlayer.score;
        }
        else if (activePlayer.playerNum == 2)
        {
            p2ScoreText.text = "Score: " + activePlayer.score;
        }
    }

    /* Work in progress to show in-game tutorial, feel free to disregard!
    public void InGameTutorialDisplay()
    {
        gameCanvas.SetActive(false);
        p1firstWB.GetComponent<BoxCollider2D>().enabled = false;
        p1secondWB.GetComponent<BoxCollider2D>().enabled = false;
        // p1secondWB.SetActive(false);
        inGameTut1.SetActive(true);
    }
    */
}
