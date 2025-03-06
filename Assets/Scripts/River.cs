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

    //initial flop
    public void Flop(Deck deck)
    {
        addToRiver(deck);
        addToRiver(deck);
        addToRiver(deck);
        addToRiver(deck);
    }

    public void addToRiver(Deck deck)
    {
        riverData.Add(deck.DealCard());
        RefreshRiver();
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
            riverCards[i].transform.position = new Vector3((xshift * i) - xshift, 0f, 0f);
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
