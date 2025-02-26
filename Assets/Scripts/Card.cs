using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Card : MonoBehaviour
{
    public enum CardValue
    {
        One = 1,
        Two = 2,
        Three = 3,
        Four = 4,
        Five = 5
    }

    public enum CardSuit
    {
        Hearts,
        Diamonds,
        Clubs,
        Spades,
        Stars
    }

    public CardValue cardValue;
    public CardSuit cardSuit;
    public SpriteRenderer spriteRenderer;
    public Texture2D texture;
    public GameplayManager gm;

    public bool bankable = true;

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

        gm = GameObject.FindGameObjectWithTag("GameplayManager").GetComponent<GameplayManager>();

        if (spriteRenderer != null && texture != null)
        {
            Debug.Log("Creating sprite");
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

    //when clicked, pass to gameplay manager to find it in the river or hand and bank it
    //when clicked, a card should be banked
    private void OnMouseDown()
    {
        Debug.Log("Clicked!");
        if (bankable)
        {
            gm.LocateAndBank(GetCardData());
        }
    }
}
