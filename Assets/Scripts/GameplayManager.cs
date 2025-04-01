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
    [SerializeField]
    private GameObject turnLights1;
    [SerializeField]
    private GameObject turnLights2;
    [SerializeField]
    private GameObject turnMessagePanel;
    [SerializeField]
    private TMP_Text turnMessageText;
    [SerializeField]
    private GameObject p1ScoreMeter;
    [SerializeField]
    private GameObject p2ScoreMeter;

    public TMP_Text currPlayerText;
    public Button next_button;
    public Button rules_button;
    public GameObject rules_panel;
    private bool rules_toggle = false;
    
    public FootAbility foot_ability;
    public HeadAbility head_ability;
    public ArmAbility arm_ability;

    public TextMeshProUGUI msg;

    private int actionsTakenInRound = 4;

    public bool in_game_tutorial = false;
    public GameObject frame1;
    public GameObject frame2;
    
    // currectly selected cards
    public List<Card> selected_cards = new List<Card>();

    //editable variables
    public int pointsToWin = 20;
    
    public List<GameObject> cards_tmp_holder = new List<GameObject>();
    public List<GameObject> wbs_tmp_holder = new List<GameObject>();
    
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
        
        foot_ability = new FootAbility();
        head_ability = new HeadAbility();
        arm_ability = new ArmAbility();
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

        //testing head ability
        /*if (Input.GetKeyDown(KeyCode.H))
        {
            HeadAbility headAbility = new HeadAbility();
            headAbility.Activate(4);
        }*/

        //terminate game when any player reach score 30
        if (playerList[0].score >= pointsToWin){
            Winner.gameWinner = playerList[0].playerNum;
            SceneManager.LoadScene(2);
        }
        if (playerList[1].score >= pointsToWin){
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

    public void ShakeCard()
    {
        StartCoroutine(selected_cards[0].GetComponent<Card>().Shake());
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
        SetActivePlayer(0);

        //buttons
        next_button.gameObject.SetActive(true);
        rules_button.gameObject.SetActive(false);
        next_button.onClick.AddListener(() => OnButtonClick(1));
        // rules_button.onClick.AddListener(() => OnButtonClick(3));
        //withdraw_button.onClick.AddListener(() => OnButtonClick(3));

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
        currPlayerText.text = "";
        if (actionsTakenInRound == 4)
        {
            currPlayerText.text += "New Round!\n";
        }
        currPlayerText.text += "Player " + activePlayer.playerNum + "'s Turn!";

        if (activePlayer.playerNum == 1)
        {
            // change location of active player indicator text 
            currPlayerText.rectTransform.anchoredPosition = new Vector2(-180, -120);

            // disable Collider component of inactive player so active player can only click their workbenches
            playerList[0].WB1.GetComponent<Bank>().enabled = true;
            playerList[0].WB2.GetComponent<Bank>().enabled = true;
            playerList[1].WB1.GetComponent<Bank>().enabled = false;
            playerList[1].WB2.GetComponent<Bank>().enabled = false;

            ShowTurnMessage(currPlayerText.text);

        }
        else if (activePlayer.playerNum == 2)
        {
            // change location of active player indicator text
            currPlayerText.rectTransform.anchoredPosition = new Vector2(-180, 130);

            // change location of active player indicator text
            playerList[1].WB1.GetComponent<Bank>().enabled = true;
            playerList[1].WB2.GetComponent<Bank>().enabled = true;
            playerList[0].WB1.GetComponent<Bank>().enabled = false;
            playerList[0].WB2.GetComponent<Bank>().enabled = false;

            ShowTurnMessage(currPlayerText.text);
        }
        
        // set active frame
        setActiveFrame(playerNum);
    }

    void setActiveFrame(int playerNum)
    {
        if (playerNum == 0)
        {
            frame2.SetActive(true);
            frame1.SetActive(false);
        }
        else
        {
            frame2.SetActive(false);
            frame1.SetActive(true);
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
                // CheckRefreshRiver();
                IncrementActivePlayer();
                GameplayManager.Instance.selected_cards.Clear();
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
        if(activePlayer.playerNum == 1)
        {
            p1ScoreMeter.GetComponent<ScoreMeter>().AwardPoints(points);
        }
        else if(activePlayer.playerNum == 2)
        {
            p2ScoreMeter.GetComponent<ScoreMeter>().AwardPoints(points);
        }

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

            if (activePlayer.playerNum == 1)
            {
                turnLights1.GetComponent<TurnIndicator>().DecrementTurn();
            }
            else if (activePlayer.playerNum == 2)
            {
                turnLights2.GetComponent<TurnIndicator>().DecrementTurn();
            }
        }

        if(actionsTakenInRound == 0)
        {
            //Time to refresh the river
            CheckRefreshRiver();

            turnLights1.GetComponent<TurnIndicator>().AllLightsOn();
            turnLights2.GetComponent<TurnIndicator>().AllLightsOn();

            // set active player to alternate who goes first
            Debug.Log("current value of active's playerNum: " + activePlayer.playerNum);
            IncrementActivePlayer();
        }
    }
    
    // Call this function whenever you change `inGameTutorial`.
    public void ToggleCards()
    {
        // 1) Find all active objects with tag "card"
        if (cards_tmp_holder.Count == 0)
        {
            cards_tmp_holder = new List<GameObject>(GameObject.FindGameObjectsWithTag("Card"));
            cards_tmp_holder.AddRange(new List<GameObject>(GameObject.FindGameObjectsWithTag("SelectPrefab")));
        }
        
        Debug.Log("toggle" + cards_tmp_holder.Count);
        
        // 2) If in tutorial, set them inactive, otherwise set them active
        bool setActive = !in_game_tutorial; // e.g. if inGameTutorial = true => setActive = false
        foreach (var card in cards_tmp_holder)
        {
            card.SetActive(setActive);
        }
    }
    
    public void ToggleWBs()
    {
        // 1) Find all active objects with tag "card"
        if (wbs_tmp_holder.Count == 0)
        {
            wbs_tmp_holder = new List<GameObject>(GameObject.FindGameObjectsWithTag("WorkBench"));
        }
        
        Debug.Log("toggle" + wbs_tmp_holder.Count);
        
        // 2) If in tutorial, set them inactive, otherwise set them active
        bool setActive = !in_game_tutorial; // e.g. if inGameTutorial = true => setActive = false
        foreach (var wb in wbs_tmp_holder)
        {
            wb.SetActive(setActive);
        }
    }

    public void ShowTurnMessage(string turnMessage, float duration = 2f)
    // Function for displaying turn messages, i.e. "Player 1's Turn!" for 2 seconds
    // Calls corresponding coroutine 
    {
        StartCoroutine(ShowTurnMessageCoroutine(turnMessage, duration));
    }

    private IEnumerator ShowTurnMessageCoroutine(string turnMessage, float duration)
    {
        turnMessageText.text = turnMessage;
        turnMessagePanel.SetActive(true);
        yield return new WaitForSeconds(duration);
        turnMessagePanel.SetActive(false);
    }
}
