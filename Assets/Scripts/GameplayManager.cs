using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using static System.Net.Mime.MediaTypeNames;
using UnityEngine.SceneManagement;
using static Card;

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
    public string player1Name = "Player 1";
    public string player2Name = "Player 2";
    public TextMeshProUGUI player1NameText;
    public TextMeshProUGUI player2NameText;
    public TMP_InputField player1Field;
    public TMP_InputField player2Field;
    public int maxCharCount = 12;

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
    private Coroutine turnOverlayCoroutine;
    [SerializeField]
    private GameObject p1ScoreMeter;
    [SerializeField]
    private GameObject p2ScoreMeter;
    public GameObject pointTable;
    public GameObject weaponTable;
    public GameObject nameScreen;

    public TMP_Text turnsLeftText;

    public TMP_Text currPlayerText;
    public Button next_button;
    public Button rules_button;
    public GameObject rules_panel;
    private bool rules_toggle = false;
    
    public FootAbility foot_ability;
    public HeadAbility head_ability;
    public ArmAbility arm_ability;
    public GameObject ConveyorBelt;

    //public TextMeshProUGUI msg;

    private int actionsTakenInRound = 4;

    public bool in_game_tutorial = false;
    public GameObject frame1;
    public GameObject frame2;

    public TMP_Text p1activeText;
    public TMP_Text p2activeText;

    // currectly selected cards
    public List<Card> selected_cards = new List<Card>();

    //editable variables
    public int pointsToWin = 20;
    
    public List<GameObject> cards_tmp_holder = new List<GameObject>();
    public List<GameObject> wbs_tmp_holder = new List<GameObject>();
    public List<GameObject> buttons_tmp_holder = new List<GameObject>();

    // Audio Sources
    public AudioSource passSound;
    public AudioSource refreshSound;

    // analytics
    public int totalTurns = 0;
    public int totalPassTurns = 0;

    //tutorial UI
    public GameObject workbenches;
    public GameObject p1Work1SellButton;
    public GameObject p1Work2SellButton;
    public GameObject p2Work1SellButton;
    public GameObject p2Work2SellButton;
    public GameObject passButton;
    public bool pastFirstTutorial = false;
    public bool onFinalTutorial = false;
    public bool MadeWrongChoice = false;
    public GameObject buildingGuide;
    public bool onWeaponTut = false;
    

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
        
        foot_ability = new FootAbility();
        head_ability = new HeadAbility();
        arm_ability = new ArmAbility();

        //important to note that scene names matter
        if(SceneManager.GetActiveScene().name == "TutorialScene")
        {
            StartCoroutine(StartTutorial());
        }
    }

    //control handler
    void Update()
    {
        /*if (Input.GetKeyDown(KeyCode.A))
        {
            ToggleOffInteractives();
        }
        if (Input.GetKeyDown(KeyCode.B))
        {
            ToggleOnInteractives();
        }*/
        
        /*
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
        if (Input.GetKeyDown(KeyCode.H))
        {
            HeadAbility headAbility = new HeadAbility();
            headAbility.Activate(4);
        }*/

        //terminate game when any player reach score 30
        if (playerList[0].score >= pointsToWin){
            Winner.gameWinner = player1Name;
            AnalyticsManager.Instance.LogGameTurns(totalTurns, totalPassTurns, 1);
            StartCoroutine(TheFinalOne());
        }
        if (playerList[1].score >= pointsToWin){
            Winner.gameWinner = player2Name;
            AnalyticsManager.Instance.LogGameTurns(totalTurns, totalPassTurns, 2);
            StartCoroutine(TheFinalOne());
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

    public void MoveCardToWB(Vector2 position, Bank wb, CardData cd)
    {
        if (selected_cards.Count == 0)
        {
            Debug.LogWarning("Tried to move card to workbench, but no card is selected!");
            return;
        }

        StartCoroutine(selected_cards[0].GetComponent<Card>().LinearAnimation(position, wb, cd));
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
        //SetActivePlayer(0);

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
            //currPlayerText.text += "New Round!\n";
            if (totalTurns == 0)
            {
                turnsLeftText.text = "";
                StartCoroutine(ShowNewRoundMessage(currPlayerText.text + player1Name + " starts!", player1Name + "'s Turn!", true));
                p1activeText.text = "(Started this round)";
                p2activeText.text = "";
            }
            else if (activePlayer.playerNum == 1)
            {
                turnsLeftText.text = "";
                StartCoroutine(FlopCoroutine());
                IEnumerator FlopCoroutine()
                {
                    yield return StartCoroutine(ConveyorBelt.GetComponent<ConveyorBeltAnim>().ConveyorBeltAnimation(river.riverCards));
                    yield return StartCoroutine(ShowNewRoundMessage(currPlayerText.text + player1Name + " starts!\n(" + player2Name + " began last round)", player1Name + "'s Turn!", false));
                }
                p1activeText.text = "(Started this round)";
                p2activeText.text = "";
            }
            else 
            {
                turnsLeftText.text = "";
                StartCoroutine(FlopCoroutine());
                IEnumerator FlopCoroutine()
                {
                    yield return StartCoroutine(ConveyorBelt.GetComponent<ConveyorBeltAnim>().ConveyorBeltAnimation(river.riverCards));
                    yield return StartCoroutine(ShowNewRoundMessage(currPlayerText.text + player2Name + " starts!\n(" + player1Name + " began last round)", player2Name + "'s Turn!", false));
                }
                p2activeText.text = "(Started this round)";
                p1activeText.text = "";
            }
        }
        else 
        {
            if (activePlayer.playerNum == 1)
            {
                currPlayerText.text += player1Name + "'s Turn!";
            }
            else if (activePlayer.playerNum == 2)
            {
                currPlayerText.text += player2Name + "'s Turn!";
            }
            turnsLeftText.text = actionsTakenInRound + " turns left in round!";
            ShowTurnMessage(currPlayerText.text);
        }
        

        if (activePlayer.playerNum == 1)
        {
            // change location of active player indicator text 
            currPlayerText.rectTransform.anchoredPosition = new Vector2(-180, -120);

            // disable Collider component of inactive player so active player can only click their workbenches
            playerList[0].WB1.GetComponent<Bank>().enabled = true;
            playerList[0].WB2.GetComponent<Bank>().enabled = true;
            playerList[1].WB1.GetComponent<Bank>().enabled = false;
            playerList[1].WB2.GetComponent<Bank>().enabled = false;
            playerList[0].WB1.GetComponent<Bank>().addable = true;
            playerList[0].WB2.GetComponent<Bank>().addable = true;
            playerList[1].WB1.GetComponent<Bank>().addable = false;
            playerList[1].WB2.GetComponent<Bank>().addable = false;
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
            playerList[1].WB1.GetComponent<Bank>().addable = true;
            playerList[1].WB2.GetComponent<Bank>().addable = true;
            playerList[0].WB1.GetComponent<Bank>().addable = false;
            playerList[0].WB2.GetComponent<Bank>().addable = false;
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
                passSound.Play(0);
                decrementActionsTaken();
                // CheckRefreshRiver();
                IncrementActivePlayer();
                GameplayManager.Instance.selected_cards.Clear();
                totalPassTurns++;
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
            refreshSound.Play(0);
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

        totalTurns++;
    }

 public void ToggleOffInteractives()
    {
        // ———  Cards & SelectPrefabs  ———
        if (cards_tmp_holder.Count == 0)
        {
            cards_tmp_holder.AddRange(GameObject.FindGameObjectsWithTag("Card"));
            cards_tmp_holder.AddRange(GameObject.FindGameObjectsWithTag("SelectPrefab"));
        }

        foreach (var go in cards_tmp_holder)
        {
            if (go == null) continue;
            // Debug.Log(go.GetComponent<Card>().GetCardData().getCardString());
            if (go.TryGetComponent<Card>(        out var card)) card.enabled = false;
            if (go.TryGetComponent<BoxCollider2D>(out var col))  col.enabled  = false;
        }

        // ———  WorkBenches  ———
        if (wbs_tmp_holder.Count == 0)
        {
            wbs_tmp_holder.AddRange(GameObject.FindGameObjectsWithTag("WorkBench"));
        }

        foreach (var go in wbs_tmp_holder)
        {
            if (go == null) continue;
            if (go.TryGetComponent<Bank>(        out var bank)) bank.enabled = false;
            if (go.TryGetComponent<BoxCollider2D>(out var col))  col.enabled  = false;
        }

        // ———  UI Buttons  ———
        if (buttons_tmp_holder.Count == 0)
        {
            buttons_tmp_holder.AddRange(GameObject.FindGameObjectsWithTag("Button"));
        }

        foreach (var go in buttons_tmp_holder)
        {
            if (go == null) continue;
            if (go.TryGetComponent<Button>(out var btn)) btn.interactable = false;
        }
    }

    /* ------------------------------------------------------------------ */
    /*  Enable                                                            */
    /* ------------------------------------------------------------------ */
    public void ToggleOnInteractives()
    {
        foreach (var go in cards_tmp_holder)
        {
            if (go == null) continue;
            if (go.TryGetComponent<Card>(        out var card)) card.enabled = true;
            if (go.TryGetComponent<BoxCollider2D>(out var col))  col.enabled  = true;
        }
        cards_tmp_holder.Clear();

        foreach (var go in wbs_tmp_holder)
        {
            if (go == null) continue;
            if (go.TryGetComponent<Bank>(        out var bank)) bank.enabled = true;
            if (go.TryGetComponent<BoxCollider2D>(out var col))  col.enabled  = true;
        }
        wbs_tmp_holder.Clear();

        foreach (var go in buttons_tmp_holder)
        {
            if (go == null) continue;
            if (go.TryGetComponent<Button>(out var btn)) btn.interactable = true;
        }

        buttons_tmp_holder.Clear();
    }
    
    // Do not use this function use ToggleOffInteractives() and ToggleOnInteractives() instead
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
    
    // Do not use this function use ToggleOffInteractives() and ToggleOnInteractives() instead
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

    public void ShowTurnMessage(string turnMessage, float duration = 2f, bool scaleImage = false, float scaleX = 1f, float scaleY = 1f)
    // Function for displaying turn messages, i.e. "Player 1's Turn!" for 2 seconds
    // Calls corresponding coroutine 
    // Adding scaling values so that the background image can be scaled up if the message content is larger
    {
        if (turnOverlayCoroutine != null)
        {
            StopCoroutine(turnOverlayCoroutine);
        }
        // start a new overlay coroutine
        turnOverlayCoroutine = StartCoroutine(ShowTurnMessageCoroutine(turnMessage, duration, scaleImage, scaleX, scaleY));
    }

    private IEnumerator ShowTurnMessageCoroutine(string turnMessage, float duration, bool scaleImage, float scaleX, float scaleY)
    {

        ToggleOffInteractives();

        turnMessageText.text = turnMessage;
        Vector3 oldImageScale = new Vector3(1f,1f,1f);
        if (scaleImage)
        {
            Transform imageBackgroundTransform = turnMessagePanel.transform.Find("Image");
            oldImageScale = imageBackgroundTransform.localScale;
            imageBackgroundTransform.localScale = new Vector3(scaleX, scaleY, 1f);
        }
        turnMessagePanel.SetActive(true);
        yield return new WaitForSeconds(duration);
        turnMessagePanel.SetActive(false);

        if (scaleImage)
        {
            Transform imageBackgroundTransform = turnMessagePanel.transform.Find("Image");
            imageBackgroundTransform.localScale = oldImageScale;
        }

        ToggleOnInteractives();

        turnOverlayCoroutine = null;
    }

    private IEnumerator ShowNewRoundMessage(string newRoundMessage, string turnMessage, bool isGameStart)
    {
        if (!isGameStart)
        {
            ShowTurnMessage("New Round!\nRefreshing Conveyor Belt...", 1.5f);
            yield return new WaitForSeconds(2f);
        }
        ShowTurnMessage(newRoundMessage);
        yield return new WaitForSeconds(2.5f);
        turnsLeftText.text = "4 turns left in round!";
        ShowTurnMessage(turnMessage);
    }

    public void ShowPointTable()
    {
        pointTable.SetActive(!pointTable.activeSelf);
    }

    public void HidePointTable()
    {
        pointTable.SetActive(false);
    }
    
    public void ShowWeaponTable()
    {
        weaponTable.SetActive(!weaponTable.activeSelf);
    }

    public void HideWeaponTable()
    {
        weaponTable.SetActive(false);
    }

    public void HideNameScreen()
    {
        player1NameText.text = player1Name;
        player2NameText.text = player2Name;
        nameScreen.SetActive(false);
        river.Flop(deck);
        StartCoroutine(FlopCoroutine());
        
        IEnumerator FlopCoroutine()
        {
            yield return StartCoroutine(ConveyorBelt.GetComponent<ConveyorBeltAnim>().ConveyorBeltAnimation(river.riverCards));
            turnMessagePanel.SetActive(true);
            SetActivePlayer(0);
        }
    }

    public void GetPlayer1Name(string name)
    {
        player1Name = name;
        if (player1Name.Length > maxCharCount)
        {
            player1Name = player1Name.Substring(0, maxCharCount);
            player1Field.text = player1Name;
        }
    }

    public void GetPlayer2Name(string name)
    {
        player2Name = name;
        if (player2Name.Length > maxCharCount)
        {
            player2Name = player2Name.Substring(0, maxCharCount);
            player2Field.text = player2Name;
        }
    }

    public void Player1NameDefault(string name)
    {
        if (name == "")
        {
            player1Name = "Player 1";
        }
    }

    public void Player2NameDefault(string name)
    {
        if (name == "")
        {
            player2Name = "Player 1";
        }
    }

    private IEnumerator RunTutorialMessages()
    {
        yield return StartCoroutine(ShowTurnMessageCoroutine("Welcome to Bot or Be Bought!", 2f, true, 2f, 3f));
        yield return StartCoroutine(ShowTurnMessageCoroutine("Build robots and sell them for points! First player to reach 20 points wins!", 4f, true, 2f, 3f));
        yield return StartCoroutine(ShowTurnMessageCoroutine("Your turn!", 2f, true, 2f, 3f));
    }

    public void RunMoreTutorialMessages()
    {
        // ToggleOffInteractives();
        workbenches.SetActive(false);
        buildingGuide.SetActive(true);
    }

    public void RunSkipAhead()
    {
        buildingGuide.SetActive(false);
        StartCoroutine(RunEvenMoreTutorialMessages());


        //Now we should clear everything in the benches and river and reactivate all UI items
        river.riverData.Clear();
        List<Card.CardSuit> suits = new List<Card.CardSuit>();
        suits.Add(Card.CardSuit.Black);
        suits.Add(Card.CardSuit.Red);
        suits.Add(Card.CardSuit.Blue);
        suits.Add(Card.CardSuit.Black);
        suits.Add(Card.CardSuit.Green);

        List<Card.CardValue> values = new List<Card.CardValue>();
        values.Add(Card.CardValue.LeftArm);
        values.Add(Card.CardValue.LeftArm);
        values.Add(Card.CardValue.RightArm);
        values.Add(Card.CardValue.LeftFoot);
        values.Add(Card.CardValue.Head);

        river.PredefinedFlop(deck, suits, values);

        //Clear the banks first then add
        p1firstWB.ClearBank();
        p1firstWB.cleanupTakenParts();
        p1secondWB.ClearBank();
        p1secondWB.cleanupTakenParts();
        /* Debugging
        Debug.Log("Bank 1 should be clear!! Count: " + p1firstWB.bankData.Count);
        for (int i = 0; i < 5; i++)
        {
            if (p1firstWB.takenParts[i])
            {
                Debug.Log("DEBUG: Found the culprit: " + i);
            }
        }
        */
        if (p1firstWB.takenParts[2])
        {
            p1firstWB.takenParts[2] = false;
        }
        else if (p1secondWB.takenParts[2])
        {
            p1secondWB.takenParts[2] = false;
        }
        
        p1firstWB.AddToWBTutorial(deck.DealSpecificCard(CardSuit.Blue, CardValue.Head));
        p1firstWB.AddToWBTutorial(deck.DealSpecificCard(CardSuit.Blue, CardValue.LeftArm));
        p1firstWB.AddToWBTutorial(deck.DealSpecificCard(CardSuit.Blue, CardValue.LeftFoot));
        p1firstWB.AddToWBTutorial(deck.DealSpecificCard(CardSuit.Blue, CardValue.RightFoot));
        
        /*
        p1firstWB.bankData.Add(deck.DealSpecificCard(CardSuit.Blue, CardValue.Head));
        p1firstWB.bankData.Add(deck.DealSpecificCard(CardSuit.Blue, CardValue.LeftArm));
        p1firstWB.bankData.Add(deck.DealSpecificCard(CardSuit.Blue, CardValue.LeftFoot));
        p1firstWB.bankData.Add(deck.DealSpecificCard(CardSuit.Blue, CardValue.RightFoot));*/
        Debug.Log(p1firstWB.bankData.Count);
        //p1firstWB.drawBench();

        p1secondWB.ClearBank();
        p1secondWB.AddToWBTutorial(deck.DealSpecificCard(CardSuit.Gold, CardValue.RightArm));
        p1secondWB.AddToWBTutorial(deck.DealSpecificCard(CardSuit.Green, CardValue.RightArm));
        /*
        p1secondWB.bankData.Add(deck.DealSpecificCard(CardSuit.Gold, CardValue.RightArm));
        p1secondWB.bankData.Add(deck.DealSpecificCard(CardSuit.Green, CardValue.RightArm));*/
        Debug.Log(p1secondWB.bankData.Count);
        //p1secondWB.drawBench();
    }


    public void RunWeaponTut()
    {
        StartCoroutine(RunWeaponTutCoroutine());
    }

    private IEnumerator RunWeaponTutCoroutine()
    {
        MadeWrongChoice = false;
        onWeaponTut = true;

        p1firstWB.robotBody.SetActive(false);
        p1Work1SellButton.SetActive(false);
        p2firstWB.gameObject.SetActive(true);
        p2secondWB.gameObject.SetActive(true);

        yield return StartCoroutine(WeaponTutorialMessages());

        p1ScoreMeter.SetActive(true);
        // CLEARING RIVER FIRST and FLOP PREDETERMINED CARDS...
        river.riverData.Clear();

        List<Card.CardSuit> suits = new List<Card.CardSuit>();
        suits.Add(Card.CardSuit.Gold);
        suits.Add(Card.CardSuit.Black);
        suits.Add(Card.CardSuit.Red);
        suits.Add(Card.CardSuit.Black);
        suits.Add(Card.CardSuit.Blue);

        List<Card.CardValue> values = new List<Card.CardValue>();
        values.Add(Card.CardValue.LeftArm);
        values.Add(Card.CardValue.Head);
        values.Add(Card.CardValue.RightArm);
        values.Add(Card.CardValue.LeftFoot);
        values.Add(Card.CardValue.Head);

        river.PredefinedFlop(deck, suits, values);

        // CLEAR BANKS FROM PREVIOUS PART and ADD PREDEFINED PARTS...
        p1firstWB.ClearBank();
        p1firstWB.cleanupTakenParts();
        p1secondWB.ClearBank();
        p1secondWB.cleanupTakenParts();
        p2firstWB.ClearBank();
        p2firstWB.cleanupTakenParts();
        p2secondWB.ClearBank();
        p2secondWB.cleanupTakenParts();

        p1firstWB.AddToWBTutorial(deck.DealSpecificCard(CardSuit.Red, CardValue.Head));
        p1firstWB.AddToWBTutorial(deck.DealSpecificCard(CardSuit.Red, CardValue.LeftArm));
        p1firstWB.AddToWBTutorial(deck.DealSpecificCard(CardSuit.Red, CardValue.LeftFoot));
        p1secondWB.AddToWBTutorial(deck.DealSpecificCard(CardSuit.Green, CardValue.RightArm));

        yield return null;

        Debug.Log("DEBUG: should be adding to p2wb2 now...");
        p2secondWB.AddToWBTutorial(deck.DealSpecificCard(CardSuit.Gold, CardValue.Head));
        p2secondWB.AddToWBTutorial(deck.DealSpecificCard(CardSuit.Gold, CardValue.LeftArm));
        p2secondWB.AddToWBTutorial(deck.DealSpecificCard(CardSuit.Gold, CardValue.RightArm));
        Debug.Log("DEBUG: p2wb2 contents: " + p2secondWB.bankData[0].getCardString() + p2secondWB.bankData[1].getCardString() + p2secondWB.bankData[2].getCardString());
    }

    private IEnumerator WeaponTutorialMessages()
    {
        p1ScoreMeter.SetActive(false);
        yield return StartCoroutine(ShowTurnMessageCoroutine("Nice job!", 2f, true, 2, 3));
        yield return StartCoroutine(ShowTurnMessageCoroutine("Now let's try building a weapon!", 4f, true, 2, 3));
        workbenches.SetActive(true);
        p1ScoreMeter.SetActive(true);
    }


    public IEnumerator RunEvenMoreTutorialMessages()
    {
        // ToggleOffInteractives();
        yield return StartCoroutine(ShowTurnMessageCoroutine("Let's skip ahead in the game...", 2f, true, 2f, 3f));
        yield return StartCoroutine(ShowTurnMessageCoroutine("Can you complete a robot\nand sell it?", 4f, true, 2f, 3f));
        workbenches.SetActive(true);
        p1ScoreMeter.SetActive(true);
        // ToggleOnInteractives();
    }

    private IEnumerator FinalTutorialMessages()
    {
        workbenches.SetActive(false);
        p1ScoreMeter.SetActive(false);
        yield return StartCoroutine(ShowTurnMessageCoroutine("Nice work!", 2f, true, 2f, 3f));
        // ShowTurnMessage("Each robot part weapon has\na unique powerful ability!", 4f, true, 2, 3);
        // yield return new WaitForSeconds(4f);
        yield return StartCoroutine(ShowTurnMessageCoroutine("Tutorial Complete!\nTry a new game with a friend!", 4f, true, 2f, 3f));
    }

    public IEnumerator FailedTutorialMessages()
    {
        workbenches.SetActive(false);
        p1ScoreMeter.SetActive(false);
        yield return StartCoroutine(ShowTurnMessageCoroutine("Not quite right!", 2f, true, 2f, 3f));
        yield return StartCoroutine(ShowTurnMessageCoroutine("You added to a weapon!", 3f, true, 2f, 3f));
        yield return StartCoroutine(ShowTurnMessageCoroutine("Try again!", 2f, true, 2f, 3f));
        workbenches.SetActive(true);
        p1ScoreMeter.SetActive(true);

        river.riverData.Clear();
        List<Card.CardSuit> suits = new List<Card.CardSuit>();
        suits.Add(Card.CardSuit.Black);
        suits.Add(Card.CardSuit.Red);
        suits.Add(Card.CardSuit.Blue);
        suits.Add(Card.CardSuit.Black);
        suits.Add(Card.CardSuit.Green);

        List<Card.CardValue> values = new List<Card.CardValue>();
        values.Add(Card.CardValue.LeftArm);
        values.Add(Card.CardValue.LeftArm);
        values.Add(Card.CardValue.RightArm);
        values.Add(Card.CardValue.LeftFoot);
        values.Add(Card.CardValue.Head);

        river.PredefinedFlop(deck, suits, values);

        //Clear the banks first then add
        p1firstWB.ClearBank();
        p1firstWB.AddToWBTutorial(deck.DealSpecificCard(CardSuit.Blue, CardValue.Head));
        p1firstWB.AddToWBTutorial(deck.DealSpecificCard(CardSuit.Blue, CardValue.LeftArm));
        p1firstWB.AddToWBTutorial(deck.DealSpecificCard(CardSuit.Blue, CardValue.LeftFoot));
        p1firstWB.AddToWBTutorial(deck.DealSpecificCard(CardSuit.Blue, CardValue.RightFoot));
        /*
        p1firstWB.bankData.Add(deck.DealSpecificCard(CardSuit.Blue, CardValue.Head));
        p1firstWB.bankData.Add(deck.DealSpecificCard(CardSuit.Blue, CardValue.LeftArm));
        p1firstWB.bankData.Add(deck.DealSpecificCard(CardSuit.Blue, CardValue.LeftFoot));
        p1firstWB.bankData.Add(deck.DealSpecificCard(CardSuit.Blue, CardValue.RightFoot));*/
        Debug.Log(p1firstWB.bankData.Count);
        //p1firstWB.drawBench();

        p1secondWB.ClearBank();
        p1secondWB.AddToWBTutorial(deck.DealSpecificCard(CardSuit.Gold, CardValue.RightArm));
        p1secondWB.AddToWBTutorial(deck.DealSpecificCard(CardSuit.Green, CardValue.RightArm));
        /*
        p1secondWB.bankData.Add(deck.DealSpecificCard(CardSuit.Gold, CardValue.RightArm));
        p1secondWB.bankData.Add(deck.DealSpecificCard(CardSuit.Green, CardValue.RightArm));*/
        Debug.Log(p1secondWB.bankData.Count);
        //p1secondWB.drawBench();
    }

    public void RunFailedWeaponTut()
    {
        StartCoroutine(FailedWeaponTutMessages());
    }

    public IEnumerator FailedWeaponTutMessages()
    {
        workbenches.SetActive(false);
        p1ScoreMeter.SetActive(false);
        yield return StartCoroutine(ShowTurnMessageCoroutine("Not quite right!", 2f, true, 2f, 3f));
        yield return StartCoroutine(ShowTurnMessageCoroutine("You added to a robot!", 3f, true, 2f, 3f));
        yield return StartCoroutine(ShowTurnMessageCoroutine("Try again!", 2f, true, 2f, 3f));
        workbenches.SetActive(true);
        p1ScoreMeter.SetActive(true);

        // CLEARING RIVER FIRST and FLOP PREDETERMINED CARDS...
        river.riverData.Clear();

        List<Card.CardSuit> suits = new List<Card.CardSuit>();
        suits.Add(Card.CardSuit.Gold);
        suits.Add(Card.CardSuit.Black);
        suits.Add(Card.CardSuit.Red);
        suits.Add(Card.CardSuit.Black);
        suits.Add(Card.CardSuit.Blue);

        List<Card.CardValue> values = new List<Card.CardValue>();
        values.Add(Card.CardValue.LeftArm);
        values.Add(Card.CardValue.Head);
        values.Add(Card.CardValue.RightArm);
        values.Add(Card.CardValue.LeftFoot);
        values.Add(Card.CardValue.Head);

        river.PredefinedFlop(deck, suits, values);

        // CLEAR BANKS FROM PREVIOUS PART and ADD PREDEFINED PARTS...
        p1firstWB.ClearBank();
        p1firstWB.cleanupTakenParts();
        p1secondWB.ClearBank();
        p1secondWB.cleanupTakenParts();

        p1firstWB.AddToWBTutorial(deck.DealSpecificCard(CardSuit.Red, CardValue.Head));
        p1firstWB.AddToWBTutorial(deck.DealSpecificCard(CardSuit.Red, CardValue.LeftArm));
        p1firstWB.AddToWBTutorial(deck.DealSpecificCard(CardSuit.Red, CardValue.LeftFoot));
        p1secondWB.AddToWBTutorial(deck.DealSpecificCard(CardSuit.Green, CardValue.RightArm));

        p2secondWB.AddToWBTutorial(deck.DealSpecificCard(CardSuit.Gold, CardValue.Head));
        p2secondWB.AddToWBTutorial(deck.DealSpecificCard(CardSuit.Gold, CardValue.LeftArm));
        p2secondWB.AddToWBTutorial(deck.DealSpecificCard(CardSuit.Gold, CardValue.RightArm));
    }

    /*
     * Ripped this logic out from the card class
     * Let the gameplay manager decide what the tooltip should be for the card on hover
     */
    public String GetCardTooltip(Card card)
    {
        String cardName = card.cardValue + "";
        if (card.cardValue == CardValue.LeftArm)
        {
            cardName = " Left Arm";
        }
        else if (card.cardValue == CardValue.RightArm)
        {
            cardName = " Right Arm";
        }
        else if (card.cardValue == CardValue.LeftFoot)
        {
            cardName = " Left Foot";
        }
        else if (card.cardValue == CardValue.RightFoot)
        {
            cardName = " Right Foot";
        }
        else
        {
            cardName = " Head";
        }
        if (SceneManager.GetActiveScene().name == "TutorialScene")
        {
            if (!pastFirstTutorial)
            {
                return "Click a part to select it!";
            }
            else
            {
                if (selected_cards.Count == 0)
                {
                    return card.cardSuit + cardName + "\nClick to select.";
                }
                else if (selected_cards.Count == 1 && selected_cards[0] != card)
                {
                    return card.cardSuit + cardName + "\nClick to select.";
                }
                else if (selected_cards.Count == 1 && selected_cards[0] == card)
                {
                    return card.cardSuit + cardName + "\nClick again to deselect.";
                }
            }
        }

        else if (SceneManager.GetActiveScene().name != "TutorialScene")
        {
            if (selected_cards.Count == 0)
            {
                return card.cardSuit + cardName;
            }
            else if (selected_cards.Count == 1 && selected_cards[0] != card)
            {
                return card.cardSuit + cardName;
            }
            else if (selected_cards.Count == 1 && selected_cards[0] == card)
            {
                return card.cardSuit + cardName;
            }
        }

        return "";
    }

    /*
     * Method to start the tutorial if gameplay manager detects in start 
     * it is in TutorialScene scene.
     */
    public IEnumerator StartTutorial()
    {
        Debug.Log("Tutorial starts");
        
        //Turn off everythign that isn't needed for now UI wise
        //Stuff in Game Components
        workbenches.SetActive(false);
        turnLights1.SetActive(false);
        turnLights2.SetActive(false);
        p1ScoreMeter.SetActive(false);
        p2ScoreMeter.SetActive(false);
        frame1.SetActive(false);
        frame2.SetActive(false);

        //And some UI buttons
        p1Work1SellButton.SetActive(false);
        p1Work2SellButton.SetActive(false);
        p2Work1SellButton.SetActive(false);
        p2Work2SellButton.SetActive(false);
        passButton.SetActive(false);

        // ToggleOffInteractives();
        yield return StartCoroutine(RunTutorialMessages());
        // ToggleOnInteractives();
        
        //Once the message completes, we need to fill the bench with some cards
        //Set player as active?
   
        //Flop with predefined cards
        List<Card.CardSuit> suits = new List<Card.CardSuit>();
        suits.Add(Card.CardSuit.Blue);
        suits.Add(Card.CardSuit.Black);
        suits.Add(Card.CardSuit.Red);
        suits.Add(Card.CardSuit.Gold);
        suits.Add(Card.CardSuit.Green);

        List<Card.CardValue> values = new List<Card.CardValue>();
        values.Add(Card.CardValue.Head);
        values.Add(Card.CardValue.LeftArm);
        values.Add(Card.CardValue.RightArm);
        values.Add(Card.CardValue.LeftFoot);
        values.Add(Card.CardValue.RightFoot);

        river.PredefinedFlop(deck, suits, values);
    }

    /*
     * At this point, the player has selected a card while in the tutorial
     */
    public void UpdateTutorial()
    {
        //Reenable the player 1 workbenches
        //And only allow them to assign

        /*
        Transform wb1 = workbenches.transform.Find("P1 Workbench 1");
        Transform wb2 = workbenches.transform.Find("P1 Workbench 2");

        if(wb1 != null && wb2 != null)
        {
            wb1.gameObject.SetActive(true);
            wb2.gameObject.SetActive(true);
        }*/
        GameObject[] selectInstances = GameObject.FindGameObjectsWithTag("SelectPrefab");

        //if there are no select instances already, then do this
        if (selectInstances.Length == 1)
        {
            workbenches.SetActive(true);
            if (selected_cards.Count > 0)
            {
                GameObject tutWB1 = workbenches.transform.GetChild(0).gameObject;
                tutWB1.SetActive(true);
                tutWB1.GetComponent<Bank>().spawnSelection(selected_cards[0].GetCardData());
                GameObject tutWB2 = workbenches.transform.GetChild(1).gameObject;
                tutWB2.SetActive(true);
                tutWB2.GetComponent<Bank>().spawnSelection(selected_cards[0].GetCardData());

                //Disable the other 2 opponent benches 2dboxcolliders, needed for dragging func
                GameObject tutWB3 = workbenches.transform.GetChild(2).gameObject;
                GameObject tutWB4 = workbenches.transform.GetChild(3).gameObject;

                tutWB3.GetComponent<BoxCollider2D>().enabled = false;
                tutWB4.GetComponent<BoxCollider2D>().enabled = false;
            }
        }
    }

    /*
     * Trigger the next half of the tutorial
     * We call this from the bank onmouseclick button when a card is added to the bank for the first time
     */
    public IEnumerator TriggerNextTutorial()
    {
        pastFirstTutorial = true;
        onFinalTutorial = true;

        if (!MadeWrongChoice)
        {
            RunMoreTutorialMessages();
        } else
        {
            yield return StartCoroutine(FailedTutorialMessages());
        }
    }

    public IEnumerator EndTutorial()
    {
        yield return StartCoroutine(FinalTutorialMessages());
        SceneManager.LoadScene("MainMenuScene");
    }

    private IEnumerator TheFinalOne()
    {
        yield return new WaitForSeconds(2.0f);
        SceneManager.LoadScene(2);
    }
}
