using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Android;

public class Card : MonoBehaviour
{
    public enum CardValue
    {
        Head = 1,
        LeftArm = 2,
        RightArm = 3,
        LeftFoot = 4,
        RightFoot = 5,
        empty = 6
    }

    public enum CardSuit
    {
        Black,
        Blue,
        Red,
        Green,
        Gold,
        empty
    }

    public CardValue cardValue;
    public CardSuit cardSuit;
    public SpriteRenderer spriteRenderer;
    public Texture2D texture;
    public GameplayManager gm;
    public GameObject select;

    public bool bankable = true;
    public bool waiting_to_put = false;
    public string place;

    // Start is called before the first frame update
    void Start()
    {
        //Initialize(CardValue.One, CardSuit.Hearts, this.texture);
    }

    public void Initialize(CardValue cV, CardSuit cS, Texture2D tex)
    {
        this.cardValue = cV;
        this.cardSuit = cS;
        this.texture = tex;

        this.place = "River";

        gm = GameObject.FindGameObjectWithTag("GameplayManager").GetComponent<GameplayManager>();

        if (spriteRenderer != null && texture != null)
        {
            spriteRenderer.sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height)
                , new Vector2(0.5f, 0.5f));
        }

        spriteRenderer.sortingOrder = 10; 

        float spriteWidth = spriteRenderer.sprite.bounds.size.x;
        float spriteHeight = spriteRenderer.sprite.bounds.size.y;

        float cardWidth = transform.localScale.x;
        float cardHeight = transform.localScale.y;

        float scaleX = cardWidth / spriteWidth;
        float scaleY = cardHeight / spriteHeight;
        float finalScale = Mathf.Min(scaleX, scaleY);
 
        spriteRenderer.transform.localScale = new Vector3(finalScale, finalScale, 1f);
    }

    public CardData GetCardData()
    {
        return new CardData(cardValue, cardSuit, texture);
    }

    // Mouseover Tooltip functions 
    private void OnMouseEnter()
    {
        if (GameplayManager.Instance.selected_cards.Count == 0)
        {
            TooltipManager._instance.SetAndShowTooltip("Click to select this card.");
        }
        else if (GameplayManager.Instance.selected_cards.Count == 1 && GameplayManager.Instance.selected_cards[0] != this)
        {
            TooltipManager._instance.SetAndShowTooltip("Click to change to this card.");
        }
        
    }

    private void OnMouseExit()
    {
        TooltipManager._instance.HideTooltip();
    }

    //when clicked, pass to gameplay manager to find it in the river or hand and bank it
    //when clicked, a card should be banked
    private void OnMouseDown()
    {
        if (!GameplayManager.Instance.selected_cards.Contains(this))
        {
            // destroy other selects first
            GameObject[] selectInstances = GameObject.FindGameObjectsWithTag("SelectPrefab");
            foreach (GameObject instance in selectInstances)
            {
                Destroy(instance);
            }
            // clear selected_cards first then add this to selected_cards
            gm.ClearSelectedCards();
            GameplayManager.Instance.selected_cards.Add(this);
            Instantiate(select, new Vector2(transform.position.x, transform.position.y), Quaternion.identity);
        }
        //Commenting out this logic and moved it to the bank class so that you can select where the selected card goes
        /*else
        {
            gm.LocateAndBank(GetCardData());
            if (gm.activePlayer.WB1.isValidAddition(GetCardData()) || gm.activePlayer.WB2.isValidAddition(GetCardData()))
            {
                if (!gm.InactivePlayerPassed())
                {
                    gm.IncrementActivePlayer();
                }
                else if (gm.InactivePlayerPassed() && gm.river.riverCards.Count == 0)
                {
                    gm.IncrementActivePlayer();
                }
            }
            gm.CheckRefreshRiver();
            return;
        }*/
        
        // TODO: use game phase to allow select card using place and cur game phase (avoid mix select)
        /*if (GameplayManager.Instance.curr_phase == 1 && place != "River")
        {
            return;
        }
        if (GameplayManager.Instance.curr_phase == 2 && place != "Workbench1")
        {
            return;
        }*/

        /*if (place == "River")
        {
            // wait to be put somewhere (workbench or other place)
            if (!waiting_to_put)
            {
                // If this card is not yet waiting, mark it as selected
                waiting_to_put = true;
                if (!GameplayManager.Instance.selected_cards.Contains(this))
                {
                    GameplayManager.Instance.selected_cards.Add(this);
                }

                Debug.Log("Card added to selection. Total selected = " + GameplayManager.Instance.selected_cards.Count);
            }
            else
            {
                // If it was already selected, unselect it
                waiting_to_put = false;
                if (GameplayManager.Instance.selected_cards.Contains(this))
                {
                    GameplayManager.Instance.selected_cards.Remove(this);
                }

                Debug.Log("Card removed from selection. Total selected = " +
                          GameplayManager.Instance.selected_cards.Count);
            }
        } else if (place == "WorkBench1")
        {
            // find it's parent workbench
            ExampleWorkBench wb = transform.parent.GetComponent<ExampleWorkBench>();
            Debug.Log(wb.cards.Count);
            
            // one click and select all cards
            if (!waiting_to_put)
            {
                foreach (Card card in wb.cards)
                {
                    card.waiting_to_put = true;
                    if (!GameplayManager.Instance.selected_cards.Contains(card))
                    {
                        GameplayManager.Instance.selected_cards.Add(card);
                    }
                    Debug.Log("Card added to selection. Total selected = " + GameplayManager.Instance.selected_cards.Count);
                }
            }
            else
            {
                foreach (Card card in wb.cards)
                {
                    card.waiting_to_put = false;
                    if (GameplayManager.Instance.selected_cards.Contains(card))
                    {
                        GameplayManager.Instance.selected_cards.Remove(card);
                    }
                    Debug.Log("Card removed from selection. Total selected = " +
                              GameplayManager.Instance.selected_cards.Count);
                }
            }
        }*/
    }
}
