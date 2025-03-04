using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Bank : MonoBehaviour
{
    //card data for all cards in bank
    public List<CardData> bankData = new List<CardData>();

    public Card.CardSuit color = Card.CardSuit.empty;
    public Card.CardValue partType = Card.CardValue.empty;

    //text listing all of the cards
    public TMP_Text bankText;

    public bool AddToBank(CardData cd)
    {
        if (isValidAddition(cd)) {
            bankData.Add(cd);
            UpdateBankText();
            return true;
        }
        else
        {
            Debug.Log("Card is invalid!");
            return false;
        }
    }

    public void UpdateBankText()
    {
        string t = "Workbench:\n";
        for(int i = 0; i < bankData.Count; i++)
        {
            t += bankData[i].getCardString() + "\n";
        }
        bankText.text = t;
    }

    public bool isValidAddition(CardData cd)
    {
        //if empty then it has to be valid
        if (bankData.Count == 0)
        {
            partType = cd.cardValue;
            color = cd.cardSuit;
            return true;
        }
        //if there's one or more parts
        else
        {
            //if it doesn't match parttype or color it's invalid
            if (cd.cardSuit != color && cd.cardValue != partType)
            {
                return false;
            }
            //if it doesn't match the color, then we're not building based on color
            if (cd.cardSuit != color)
            {
                color = Card.CardSuit.empty;
            }
            if (cd.cardValue != partType)
            {
                partType = Card.CardValue.empty;
            }
            return true;

        }
    }

    public void sellWorkBench()
    {
        while(bankData.Count > 0)
        {
            bankData.RemoveAt(0);
            //increase points here for each part sold
        }
    }
}
