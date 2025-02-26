using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hand : MonoBehaviour
{
    //card data for all cards in hand
    public List<CardData> handData = new List<CardData>();

    //array of card objects shown in game
    public List<GameObject> cards = new List<GameObject>();

    //corresponding player bank
    public Bank bank;

    public GameObject cardPrefab;

    public float xshift = 70f;
    public float yshift = -120f;

    public void GetStartingHand(Deck deck)
    {
        handData.Add(deck.DealCard());
        handData.Add(deck.DealCard());
        handData.Add(deck.DealCard());
    }

    //show the cards in a player's hand
    //when switching between turns, we will hide the previous player's hand and then show the next player's
    public void ShowHand()
    {
        for(int i = 0; i < handData.Count; i++)
        {
            cards.Add(Instantiate(cardPrefab));
            cards[i].GetComponent<Card>().Initialize(handData[i].cardValue, handData[i].cardSuit, handData[i].texture);
            cards[i].transform.position = new Vector3((xshift * i) - xshift, yshift, 0f);
        }
    }

    //hide the cards in a player's hand
    //when switching between turns, we will hide the previous player's hand and then show the next player's
    public void HideHand()
    {
        while(cards.Count > 0)
        {
            GameObject toDestroy = cards[0];
            cards.RemoveAt(0);
            Destroy(toDestroy);
        }
    }

    //transfer card from hand into bank
    public void BankCard(int index)
    {
        if (index < handData.Count) {
            CardData cd = handData[index];
            bank.AddToBank(cd);
            handData.RemoveAt(index);
            HideHand();
            ShowHand();
        }
    }

    //if a card is clicked, locate it in the hand and bank it
    public bool LocateAndBank(CardData cd)
    {
        for(int i = 0; i < handData.Count; i++)
        {
            //if card matches bank it
            if (cd.cardSuit == handData[i].cardSuit && cd.cardValue == handData[i].cardValue)
            {
                BankCard(i);
                return true;
            }
        }
        return false;
    }
}
