using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Android;
using UnityEngine.SceneManagement;

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
        /*if (GameplayManager.Instance.selected_cards.Count == 0)
        {
            TooltipManager._instance.SetAndShowTooltip(cardSuit + " " + cardValue + "\nClick to select.");
        }
        else if (GameplayManager.Instance.selected_cards.Count == 1 && GameplayManager.Instance.selected_cards[0] != this)
        {
            TooltipManager._instance.SetAndShowTooltip(cardSuit + " " + cardValue + "\nClick to select.");
        }*/
        string tooltip = GameplayManager.Instance.GetCardTooltip(this);

        if (tooltip != "")
        {
            TooltipManager._instance.SetAndShowTooltip(tooltip);
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
            // spawn selected parts for workbench here
            GameObject[] workbenches = GameObject.FindGameObjectsWithTag("WorkBench");
            foreach (GameObject wb in workbenches)
            {
                Bank current = wb.GetComponent<Bank>();
                current.spawnSelection(this.GetCardData());
            }
        }

        //Tutorial specific on mouse down logic
        if(SceneManager.GetActiveScene().name == "TutorialScene")
        { 
            GameplayManager.Instance.UpdateTutorial();
        }
    }

    public IEnumerator Shake()
    {
        Vector3 position = transform.position;
        float elapsedTime = 0f;
        float t = 0f;

        while (elapsedTime < 0.5f)
        {
            t += Time.deltaTime * (1f / 0.05f);
            float xOffset = Mathf.Lerp(4.0f, -4.0f, Mathf.PingPong(t, 1));
            transform.localPosition = new Vector3(position.x + xOffset, position.y, position.z);

            elapsedTime += Time.deltaTime;
            yield return null;
        }

        transform.position = position;
    }

    public IEnumerator LinearAnimation(Vector2 targetPosition, Bank wb, CardData cd)
    {
        Vector2 startPosition = transform.position;
        float elapsedTime = 0f;

        while (elapsedTime < 0.5f)
        {
            transform.position = Vector2.Lerp(startPosition, targetPosition, elapsedTime / 0.5f);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        transform.position = targetPosition;
        wb.AddToWB(cd);
    }
}
