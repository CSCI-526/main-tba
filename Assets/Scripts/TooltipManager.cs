using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class TooltipManager : MonoBehaviour
{
    public static TooltipManager _instance;
    public TextMeshProUGUI textComponent;
    private RectTransform rectTransform;

    private void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        // Singleton pattern 
        if (_instance != null && _instance != this)
        {
            Destroy(this.gameObject);
        }
        else
        {
            _instance = this;
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        Cursor.visible = true;
        gameObject.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        Vector2 mousePos = Input.mousePosition;

        float tooltipWidth = rectTransform.rect.width;
        float tooltipHeight = rectTransform.rect.height;
        float buffer = 10f;
        float pivotX = 0;
        float pivotY = 0;

        if (mousePos.y - tooltipHeight + buffer < 0)
        {
            pivotY = 0f;
        }
        else
        {
            pivotY = 1f;
        }

        if (mousePos.x + tooltipWidth + buffer > Screen.width)
        {
            pivotX = 1f;
        }
        else
        {
            pivotX = 0f;
        }

        rectTransform.pivot = new Vector2(pivotX, pivotY);
        rectTransform.position = Input.mousePosition;
    }

    public void SetAndShowTooltip(string message)
    {
        gameObject.SetActive(true);
        textComponent.text = message;
    }

    public void HideTooltip()
    {
        gameObject.SetActive(false);
        textComponent.text = string.Empty;
    }
}
