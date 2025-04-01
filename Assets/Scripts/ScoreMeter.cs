using UnityEngine;

public class ScoreMeter : MonoBehaviour
{
    public Transform fillBar;
    private int currScore = 0;
    private float maxWidth = 1.0f;
    private Vector3 fullScale;
    private Vector3 initialPosition;

    void Start()
    {
        fullScale = fillBar.localScale;
        initialPosition = fillBar.localPosition;
        UpdateFill();
    }

    private void OnMouseEnter()
    {
        string msg = currScore + "/20 points to win.";
        TooltipManager._instance.SetAndShowTooltip(msg);
    }   

    public void OnMouseExit()
    {
        TooltipManager._instance.HideTooltip();
    }

    public void AwardPoints(int points)
    {
        currScore += points;
        UpdateFill();
    }

    private void UpdateFill()
    {
        float fillAmount = (float) currScore / 20;
        fillBar.localScale = new Vector3(fullScale.x * fillAmount, fullScale.y, fullScale.z);

        float offset = (fillBar.localScale.x - fullScale.x) * 0.5f;
        fillBar.localPosition = initialPosition + new Vector3(offset, 0, 0);
    }
}
