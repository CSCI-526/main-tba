using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting.Dependencies.NCalc;
using UnityEngine;

public class SellArea : MonoBehaviour
{
    public string name_ = "SellArea";
    
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    
    private void OnMouseDown()
    {
        // If we have selected cards
        if (GameplayManager.Instance.selected_cards.Count > 0)
        {
            Debug.Log("Selling all selected cards...");

            // We'll copy the list so we don't modify it while iterating
            var cardsToSell = new System.Collections.Generic.List<Card>(GameplayManager.Instance.selected_cards);

            // Clear the original list so we don't keep references 
            GameplayManager.Instance.ClearSelectedCards();
            
            // clear workbench hold cards
            cardsToSell[0].transform.parent.GetComponent<ExampleWorkBench>().ClearCards();

            foreach (Card card in cardsToSell)
            {
                if (card.waiting_to_put)
                {
                    // Sell the card
                    GameplayManager.Instance.SellCard(card.GetCardData());

                    card.place = name_;

                    // Destroy the card from the scene (or hide it)
                    Destroy(card.gameObject);
                }
            }
        }
        else
        {
            Debug.Log("No cards are currently selected.");
        }
    }
}
