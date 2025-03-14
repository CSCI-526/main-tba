using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HeadAbility : IAbility
{
    //Handles the head ability
    //Shows the player the next 2 + duplicateCount cards
    //We can fix this scaling function later
    public void Activate(int duplicateCount, Bank workBench)
    {
        //Get the peekList from the GameplayManagaer
        List<CardData> peekList = GameplayManager.Instance.deck.PeekNextNCards(duplicateCount);

        //Display this list for the current player somehow
        //Gameplaymanager has to start the coroutine unfortunately because start coroutine requires monobehavior
        GameplayManager.Instance.StartCoroutine(SeeFutureCards(peekList));

    }

    private IEnumerator SeeFutureCards(List<CardData> cards)
    {
        GameplayManager.Instance.msg.text = "Head player, press H next turn to see future cards!";

        //Weird issue where this only good at the beginning of your turn
        //And you need your opponent to turn their back lol
        while (!Input.GetKeyDown(KeyCode.H))
        {
            yield return null;
        }

        string futureText = "";
        foreach (CardData card in cards) 
        { 
            futureText += card.getCardString() + " ";
        }
        GameplayManager.Instance.msg.text = futureText;
    }
}
