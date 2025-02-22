using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Deck : MonoBehaviour
{
    /*public class CardData
    {
        //this class is just for storing values which we'll use to 
        //instantiate multiple cards from the card prefab
        public Card.CardValue cardValue;
        public Card.CardSuit cardSuit;
        public Texture2D texture;

        public CardData(Card.CardValue value, Card.CardSuit suit, Texture2D texture)
        {
            this.cardValue = value;
            this.cardSuit = suit;
            this.texture = texture;
        }

    }*/

    public GameObject cardPrefab;

    //Our deck is just a list of card data, when we want a card, we pop and instantiate a card
    public List<CardData> deck = new List<CardData>();
    public Texture2D[] cardTextures;

    // Start is called before the first frame update
    void Start()
    {
        /*InitializeDeck();
        //PrintDeck();
        ShuffleDeck();
        //PrintDeck();*/


        /*This is to test dealing a card and instantiating it*/
        /*Debug.Log(deck.Count);
        CardData cardData = DealCard();
        Debug.Log(deck.Count);*/

        //This card spawning logic might need to be handled elsewhere
        /*GameObject newCard = Instantiate(cardPrefab);
        newCard.GetComponent<Card>().Initialize(cardData.cardValue,
            cardData.cardSuit, cardData.texture);*/
        //Card should be dealt at 0,0,0
        //This is just to show we can instantiate cards once we have the cardData
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    //Populate the deck of cards represented as a stack, needs shuffling
    public void InitializeDeck()
    {
        //For right now deck consists of 5 suits, cards numbered 1-5
        for(int i = 0; i < cardTextures.Length; i++)
        {
            //right now deck creation depends on the filename
            CardData cardData = ParseTextureFileName(cardTextures[i]);
            deck.Add(cardData);  
        }
        //Debug.Log(deck.Count);
    }

    //Shuffle the deck using Fischer-Yates
    public void ShuffleDeck()
    {
        System.Random rand = new System.Random();
        int i = deck.Count - 1;
        while (i > 0)
        {
            int randomNumber = rand.Next(0, i + 1);
            CardData temp = deck[randomNumber];
            deck[randomNumber] = deck[i];
            deck[i] = temp;
            i--;
        }
    }

    //Just a bunch of debug.logs to see the cards
    void PrintDeck()
    {
        for (int i = 0; i < deck.Count; i++)
        {
            Debug.Log("Card value: " + deck[i].cardValue +
                " Card suit: " + deck[i].cardSuit);

        }
    }

    //Deal a card
    //Take the first card in the deck and remove it from the deck
    //Remove it from the deck because it could get banked and may not return
    //Return the card data to the caller
    public CardData DealCard()
    {
        if(deck.Count == 0)
        {
            //Not sure on behavior here
            //If the deck is empty but there is still a river to be created (in the case of every player banking a lot)
            //Not sure if we want to just have an empty river
            Debug.Log("Deck is empty!");
            return null;
        }
        CardData cardData = deck[deck.Count - 1];
        deck.RemoveAt(deck.Count - 1);

        return cardData;
    }

    //Return a card to the deck
    //Used for cleaning up the cards after a round and returning
    //the river and player cards (not banked) to the deck
    public void ReturnCardToDeck(CardData cardData)
    {
        deck.Add(cardData);
    }

    //Using the name of the texture, generate a CardData object
    //Filename has to be in the format "1_spades.png" otherwise this will break
    //Suit is case insensitive
    CardData ParseTextureFileName(Texture2D texture)
    {
        //Using the name of the texture, create card data
        string fileName = texture.name;
        string[] parts = fileName.Split('_');

        //Should be only 2 parts to a filename separated by "_"
        if(parts.Length != 2 )
        {
            Debug.Log("Invalid texture filename, cannot create card");
            return null;
        }

        //Check parts[0] which is the number of the card
        int value;
        int.TryParse(parts[0], out value);
        if(value < 1 || value > 5)
        {
            Debug.Log("Invalid card value in texture filename, cannot create card");
            return null;
        }

        Card.CardSuit suit;
        if(!System.Enum.TryParse(parts[1], true, out suit))
        {
            Debug.Log(parts[1]);
            Debug.Log("Invalid suit in texture filename, cannot create card");
            return null;
        }

        return new CardData((Card.CardValue)value, suit, texture);
    }
}
