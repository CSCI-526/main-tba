using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardData
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

    public string getCardString()
    {
        return cardValue + " of " + cardSuit;
    }

}
