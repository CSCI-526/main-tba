using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HeadAbility : IAbility
{
    //Handles the head ability
    //Shows the player the next 2 + duplicateCount cards
    //We can fix this scaling function later
    public void Activate(int duplicateCount)
    {
        //Get the peekList from the GameplayManagaer
        List<CardData> peekList = GameplayManager.Instance.deck.PeekNextNCards(duplicateCount);

        //Display this list for the current player somehow
        foreach (CardData card in peekList)
        {
            Debug.Log(card.getCardString());
        }

    }
}
