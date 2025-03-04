using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using static System.Net.Mime.MediaTypeNames;

public class GameplayManager : MonoBehaviour
{

    //Made GamePlayer to encapsulate the player objects in the gameplay manager
    //There is a Player class but I didn't want to break gameplaying
    public class GamePlayer
    {
        public int playerNum;
        public int score;
        public Bank WB1;
        public Bank WB2;

        public GamePlayer(int playerNum, Bank WB1, Bank WB2)
        {
            this.playerNum = playerNum;
            this.score = 0;
            this.WB1 = WB1;
            this.WB2 = WB2;
        }
    }



    public enum GamePhase{
        BeforeStart,//0//everyone can see the screen //show rules
        BlindBetting,//1//if player i is dealer, player i+1 and i+2 have to bet 10
        Deal3Hands_PreFlopBetting,//2//player i+3 start betting until everyone is call or fold
        Flop3Cards_HandBanking,//3//Select banking card from player i+1 to dealer
        FlopBetting,//4//from player i+1 until everyone is call or fold
        FlopBanking,//5//from player i+1 to dealer, can use 15 money to bank a card from community
                    //if a card is bought, deal another card from the deck
        TurnBetting,//6//from player i+1 until everyone is call or fold
        TurnBanking,//7//from player i+1 to dealer, can use 20 money to bank a card from community
                    //if a card is bought, deal another card from the decFlopBetting,
        RiverBetting,//8//from player i+1 until everyone is call or fold
        RiverBanking,//9//from player i+1 to dealer, can use 25 money to bank a card from community
                    //if a card is bought, deal another card from the deck
        Withdrawal,//10//choose cards to use in this round from bank
        Showdown,//11//show the winner, all players' hands and what combination each player has
        BeforeNextround //12//everyone can see the screen
                        //put all showdown cards to deck and reshuffle
                        //give pot money to the winner
                        //show all player's remaining money on the scene
                        //check all player's money, if money == 0, game over
    }

    //objects that carry over from turn to turn
    public Deck deck;
    public River river;
    public int dealer;
    public int curr_phase;
    int saved_phase;
    public int select_card;
    public List<CardData> combinedData = new List<CardData>();
    public GamePlayer winner;

    //int count;

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
    public TMP_Text currEventText;
    public TMP_Text instructionText;
    public TMP_Text currPlayerText;
    public TMP_Text p1ScoreText;
    public TMP_Text p2ScoreText;
    public Button next_button;
    public Button next_player_button;
    public Button rules_button;
    public GameObject rules_panel;
    private bool rules_toggle = false;
    
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
            if (currPlayerNum == 0)
            {
                SetActivePlayer(currPlayerNum + 1);
            } else if (currPlayerNum == 1)
            {
                SetActivePlayer(0);
            }
        }

        //add to river - r
        if (Input.GetKeyDown(KeyCode.R))
        {
            river.addToRiver(deck);
        }
    }

    //search the river and the active player's hand for a card and bank it
    public void LocateAndBank(CardData cd)
    {
        river.LocateAndBank(cd);
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

        //for now let's just start with player one as the dealer (conveniently they'll bet first since blinds have put in already)
        SetActivePlayer(0);
        dealer = 1;

        //buttons
        next_button.gameObject.SetActive(true);
        next_player_button.gameObject.SetActive(false);
        rules_button.gameObject.SetActive(true);
        next_button.onClick.AddListener(() => OnButtonClick(1));
        next_player_button.onClick.AddListener(() => OnButtonClick(2));
        rules_button.onClick.AddListener(() => OnButtonClick(3));
        //withdraw_button.onClick.AddListener(() => OnButtonClick(3));


        curr_phase = 0;
        ChangePhase(curr_phase);

        //instructionText.gameObject.SetActive(false);
    }


    /*void OnSubmitBet(string betAmount)
    {
        if (!string.IsNullOrEmpty(betAmount))
        {
            Debug.Log("User submitted: " + betAmount);
            lastBet = int.Parse(betAmount);
            betPlaced = true;
            bet_input.text = "";
        }
    }*/
    
    //Calls the coroutine
    /*public void StartBettingRound()
    {
        currEventText.text = "Betting Round! Input bet and press enter!";
        StartCoroutine(HandleBettingRound());  
    }*/
    //Handle the betting round
    //Take bets from each player, add it to the pot
    //This is where the pot gets updated
    /*public IEnumerator HandleBettingRound()
    {
       
        for (int i = 0; i < playerList.Length; i++)
        {
            SetActivePlayer(i);
            betPlaced = false;

            bool validBet = false;

            while (!validBet)
            {
                yield return new WaitUntil(() => betPlaced);

                if(lastBet <= activePlayer.chipTotal)
                {
                    validBet = true;
                } else
                {
                    Debug.Log("Invalid bet given! Bet is more than what player currently has.");
                    betPlaced = false;
                }
            }
        }

        //At this point all bets have been submitted move on to the next phase
        //Kill the listener
    }*/

    //Only happens once at the start of the game
    //Each player takes one of three cards and banks it
    public void BankingRound()
    {
        currEventText.text = "Bank a card from your hand by clicking the card!";

        if(activePlayer.playerNum == dealer){
            next_button.gameObject.SetActive(true);
            next_player_button.gameObject.SetActive(false);
            curr_phase = 3;
        }
        else{
            next_button.gameObject.SetActive(false);
            next_player_button.gameObject.SetActive(true);
        }
    }

    //Method to call the river's flop method
    public void DealFlop()
    {
        river.Flop(deck);
    }

    //Method to call to handle players banking from the river
    public void RiverBankingRound()
    {
        currEventText.text = "Bank a river card!";

        if(activePlayer.playerNum == dealer){
            next_button.gameObject.SetActive(true);
            next_player_button.gameObject.SetActive(false);
            curr_phase = saved_phase;
        }
        else{
            next_button.gameObject.SetActive(false);
            next_player_button.gameObject.SetActive(true);
        }
    }

    //Each player picks cards from their bank
    //and makes a hand
    public void Withdrawal()
    {
        currEventText.text = "Take cards from your bank! Input card number and press enter";
        Startwithdraw();

        if(activePlayer.playerNum == dealer){
            next_button.gameObject.SetActive(true);
            next_player_button.gameObject.SetActive(false);
            curr_phase = saved_phase;
        }
        else{
            next_button.gameObject.SetActive(false);
            next_player_button.gameObject.SetActive(true);
        }

    }

    void OnSubmitWithdraw(string card_no)
    {
        if (!string.IsNullOrEmpty(card_no))
        {
            Debug.Log("withdraw: " + card_no);
            select_card = int.Parse(card_no);
        }
    }

    public void Startwithdraw()
    {
        currEventText.text = "Withdraw the hand you want to use in this round!";
    }

    /*public void WithdrawFromBankToHand(int num)
    {
        Debug.Log("move: " + num);
        CardData temp = activePlayer.bank.bankData[num-1];
        activePlayer.hand.handData.Add(temp);
        activePlayer.bank.bankData.RemoveAt(num-1);
        activePlayer.bank.UpdateBankText();
        activePlayer.hand.HideHand();
        activePlayer.hand.ShowHand();
    }*/

    /*public void showHands()
    {
        currEventText.text = "Show Down!";
        activePlayer.hand.ShowHand();

        if(activePlayer.playerNum == dealer){
            next_button.gameObject.SetActive(true);
            next_player_button.gameObject.SetActive(false);
            curr_phase = saved_phase;
        }
        else{
            next_button.gameObject.SetActive(false);
            next_player_button.gameObject.SetActive(true);
        }

    }*/
 

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
        //activePlayer.hand.ShowHand();
        //activePlayer.bank.bankText.gameObject.SetActive(true);
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

    void DoBeforeStart(){
        //show information
        currEventText.text ="Player" + dealer +  " is dealer.";
    }

    /*void DoBlindBetting(){
        Debug.Log("DoBlindBetting");
        currEventText.text ="Player" + activePlayer.playerNum +  " blind bet 10";
        //Debug.Log("curr_event");
        GamePlayer blind1 = playerList[activePlayer.playerNum-1];
        blind1.chipTotal = blind1.chipTotal - 10;

        //put into pot
        potTotal += 10;
        potText.text = "Pot: " + potTotal;
        money.updateValue(activePlayer.chipTotal);

        if(activePlayer.playerNum == dealer+2){
            next_button.gameObject.SetActive(true);
            next_player_button.gameObject.SetActive(false);
        }
        else{
            next_button.gameObject.SetActive(false);
            next_player_button.gameObject.SetActive(true);
        }
        //next_button.gameObject.SetActive(true);
        //Debug.Log("Deck found with " + deck.deck.Count + " cards.");
    }*/

    /*public void DoDeal3Hands_PreFlopBetting(){
        //currEventText.text = "Press S to see your hands. Press H to hide hands.";
        instructionText.gameObject.SetActive(true);
        SetActivePlayer(1);
        StartBettingRound();
    }*/

    void DoFlop3Cards(){
        currEventText.text = "Bank a card from your hand by clicking the card!";
        DealFlop();
        SetActivePlayer(1);
        saved_phase = curr_phase;
        curr_phase = 12;
    }

    /*void DoFlopBetting(){
        StartBettingRound();
        next_button.gameObject.SetActive(true);
    }*/

    void DoFlopBanking(){
        currEventText.text = "Bank a river card!";
        SetActivePlayer(1);
        saved_phase = curr_phase;
        curr_phase = 14;
    }

    /*void DoTurnBetting(){
        river.addToRiver(deck);
        StartBettingRound();
    }*/

    void DoTurnBanking(){
        currEventText.text = "Bank a river card!";
        SetActivePlayer(1);
        saved_phase = curr_phase;
        curr_phase = 14;
    }

    /*void DoRiverBetting(){
        river.addToRiver(deck);
        StartBettingRound();
    }*/

    void DoRiverBanking(){
        currEventText.text = "Bank a river card!";
        SetActivePlayer(1);
        saved_phase = curr_phase;
        curr_phase = 14;
    }

    void DoWithdrawal(){
        currEventText.text = "Withdraw the hand you want to use in this round!";
        Startwithdraw();
        saved_phase = curr_phase;
        curr_phase = 15;
    }

    void DoShowdown(){
        currEventText.text = "Show Down";
        //activePlayer.hand.ShowHand();
        saved_phase = curr_phase;
        curr_phase = 16;
    }

    void DoBeforeNextround(){
        currEventText.text = "Start Next Round!";
        curr_phase = -1;
        if (dealer == 3){
            dealer = 1;
        }
        else{
            dealer++;
        }

        //initialize deck and put hands to deck

        SetActivePlayer(dealer-1);
        //ChangePhase(curr_phase);

    }

    void OnButtonClick(int buttonID)
    {
        switch (buttonID)
        {
            case 1:
                Debug.Log("next_button clicked!");
                if (activePlayer.playerNum == 2){
                    SetActivePlayer(0);
                }
                else{
                    SetActivePlayer(activePlayer.playerNum);
                }
                curr_phase++;
                Debug.Log("curr_phase: " + curr_phase);
                Debug.Log("player "+ activePlayer.playerNum);
                //next_button.gameObject.SetActive(false);
                ChangePhase(curr_phase);
                break;
            case 2:
                Debug.Log("next_player_button clicked!");
                Debug.Log("curr_phase: " + curr_phase);
                Debug.Log("player "+ activePlayer.playerNum);
                if (activePlayer.playerNum == 3){
                    SetActivePlayer(0);
                }
                else{
                    SetActivePlayer(activePlayer.playerNum);
                }
                //next_button.gameObject.SetActive(false);
                ChangePhase(curr_phase);
                break;
            case 3:
                Debug.Log("rules button clicked!");
                rules_toggle = !rules_toggle;
                // make other component active, change text
                rules_button.GetComponentInChildren<TextMeshProUGUI>().text = rules_toggle ? "Hide" : "Rules";
                rules_panel.SetActive(rules_toggle);
                break;

            default:
                Debug.Log("Unknown button clicked!");
                break;
        }
    }

    void ChangePhase(int curr_event){
        /*if(curr_event == (int)GamePhase.BeforeStart){
            DoBeforeStart();
        }
        else if(curr_event == (int)GamePhase.BlindBetting){
            DoBlindBetting();
        }
        else if(curr_event == (int)GamePhase.Deal3Hands_PreFlopBetting){
            DoDeal3Hands_PreFlopBetting();
        }
        else if(curr_event == (int)GamePhase.Flop3Cards_HandBanking){
            DoFlop3Cards();
        }
        else if(curr_event == (int)GamePhase.FlopBetting){
            DoFlopBetting();
        }
        else if(curr_event == (int)GamePhase.FlopBanking){
            DoFlopBanking();
        }
        else if(curr_event == (int)GamePhase.TurnBetting){
            DoTurnBetting();
        }
        else if(curr_event == (int)GamePhase.TurnBanking){
            DoTurnBanking();
        }
        else if(curr_event == (int)GamePhase.RiverBetting){
            DoRiverBetting();
        }
        else if(curr_event == (int)GamePhase.RiverBanking){
            DoRiverBanking();
        }
        else if(curr_event == (int)GamePhase.Withdrawal){//10
            DoWithdrawal();
        }
        else if(curr_event == (int)GamePhase.Showdown){//11
            DoShowdown();
        }
        else if(curr_event == (int)GamePhase.BeforeNextround){//12
            DoBeforeNextround();
        }
        else if(curr_event == 13){//banking
            BankingRound();
        }
        //else if(curr_event == 14){//betting
        //    HandleBettingRound();
        //}
        else if(curr_event == 15){//banking from river
            RiverBankingRound();
        }
        else if(curr_event == 16){//withdraw
            Withdrawal();
        }
        else if(curr_event == 17){//showdown
            showHands();
        }*/
        
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
}
