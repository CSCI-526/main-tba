using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExampleWorkBench : MonoBehaviour
{
    [Header("Card Placement Settings")]
    public float horizontalSpacing = 0.5f;  // how far apart in X to place cards
    public float verticalOffset = 0f;       // how far in Y to place them (if you want a row/column offset)
    public float cardScale = 0.5f;         // scale factor to resize cards

    public string name_ = "WorkBench1";
    
    
    public List<Card> cards = new List<Card>();
    
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    // put card so they do not overlap
    private void putCard()
    {
        
    }
    
    private void OnMouseDown()
    {
        // If we have selected cards
        if (GameplayManager.Instance.selected_cards.Count > 0)
        {
            Debug.Log("Storing all selected cards on the Workbench.");
            
            foreach (Card card in GameplayManager.Instance.selected_cards)
            {
                if (card.waiting_to_put)
                {
                    // Parent the card to this Workbench
                    card.transform.SetParent(this.transform);

                    card.place = name_;
                    
                    // Apply a uniform scale if you'd like to shrink or grow cards
                    card.transform.localScale = Vector3.one * cardScale;

                    // Calculate offset for this new card
                    // We'll place it to the right of any existing cards
                    float xPos = cards.Count * horizontalSpacing;
                    float yPos = 0f + verticalOffset;
                    card.transform.localPosition = new Vector3(xPos, yPos, 0f);
                    
                    card.waiting_to_put = false;
                    
                    cards.Add(card);
                }
            }

            // Clear the list now that we've placed them
            GameplayManager.Instance.ClearSelectedCards();
            Debug.Log("here");
            Debug.Log(cards.Count);
        }
        else
        {
            Debug.Log("No cards are currently selected.");
        }
    }

    public void ClearCards()
    {
        cards.Clear();
    }
}
