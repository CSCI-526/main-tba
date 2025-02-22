using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Bank : MonoBehaviour
{
    //card data for all cards in bank
    public List<CardData> bankData = new List<CardData>();

    //text listing all of the cards
    public TMP_Text bankText;

    public void AddToBank(CardData cd)
    {
        bankData.Add(cd);
        UpdateBankText();
    }

    public void UpdateBankText()
    {
        string t = "Bank:\n";
        for(int i = 0; i < bankData.Count; i++)
        {
            t += (i+1) + ") " + bankData[i].getCardString() + "\n";
        }
        bankText.text = t;
    }
}
