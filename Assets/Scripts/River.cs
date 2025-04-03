using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Collections.LowLevel.Unsafe;
using UnityEngine;

public class River : MonoBehaviour
{
    //card data for all cards in river
    public List<CardData> riverData = new List<CardData>();

    //array of card objects shown in river
    public List<GameObject> riverCards = new List<GameObject>();
    public GameObject cardPrefab;

    public GameplayManager gm;

    public float xshift = 70f;
    public float yshift;

    //initial flop
    public void Flop(Deck deck)
    {
        addToRiver(deck);
        riverData[0].pos = 0;
        addToRiver(deck);
        riverData[1].pos = 1;
        addToRiver(deck);
        riverData[2].pos = 2;
        addToRiver(deck);
        riverData[3].pos = 3;
        addToRiver(deck);
        riverData[4].pos = 4;
        RefreshRiver();
    }

    //Add a predefined list of cards to the river instead of
    //randomly dealing from the deck
    public void PredefinedFlop(Deck deck, List<Card.CardSuit> suits, List<Card.CardValue> values)
    {
        if(suits.Count != values.Count)
        {
            Debug.Log("More suits than values given or vice versa.");
            return;
        }
        for (int i = 0; i < suits.Count; i++)
        {
            CardData card = deck.DealSpecificCard(suits[i], values[i]);
            riverData.Add(card);
            riverData[i].pos = i;
        }
        RefreshRiver();
    }

    public void addToRiver(Deck deck)
    {
        riverData.Add(deck.DealCard());
    }

    //call this to show the new cards that are added to the river
    public void RefreshRiver()
    {
        while (riverCards.Count > 0)
        {
            GameObject toDestroy = riverCards[0];
            riverCards.RemoveAt(0);
            Destroy(toDestroy);
        }
        for (int i = 0; i < riverData.Count; i++)
        {
            riverCards.Add(Instantiate(cardPrefab));
            riverCards[i].GetComponent<Card>().Initialize(riverData[i].cardValue, riverData[i].cardSuit, riverData[i].texture);
            riverCards[i].transform.position = new Vector3((xshift * riverData[i].pos) - xshift*2.25f, yshift, 0f);
        }
    }

    public void BankCard(int index)
    {
        if (index < riverData.Count)
        {
            CardData cd = riverData[index];
            //I think there needs to be logic here to control which bench the river card goes to
            if (gm.activePlayer.WB1.AddToBank(cd))
            {
                riverData.RemoveAt(index);
            } else if (gm.activePlayer.WB2.AddToBank(cd))
            {
                riverData.RemoveAt(index);
            }
            RefreshRiver();
        }
    }

    //if a card is clicked, locate it in the river and bank it
    //if it's not there, return false, otherwise true
    public bool LocateAndBank(CardData cd)
    {
        for (int i = 0; i < riverData.Count; i++)
        {
            //if card matches bank it
            if (cd.cardSuit == riverData[i].cardSuit && cd.cardValue == riverData[i].cardValue)
            {
                BankCard(i);
                return true;
            }
        }
        return false;
    }

    //Just a function to locate and delete a card in the river
    //No banking, let the bank class handle that
    public bool LocateAndDelete(CardData cd)
    {
        for (int i = 0; i < riverData.Count; i++)
        {
            //if card matches delete it
            if (cd.cardSuit == riverData[i].cardSuit && cd.cardValue == riverData[i].cardValue)
            {
                riverData.RemoveAt(i);
                RefreshRiver();
                return true;
            }
        }
        return false;
    }
}
