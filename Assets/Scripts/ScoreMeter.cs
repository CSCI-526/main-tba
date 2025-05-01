using UnityEngine;
using System.Collections;

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
        StartCoroutine(UpdateFillCoroutine(points));
    }

    private void UpdateFill()
    {
        float fillAmount = (float) currScore / 20;
        fillBar.localScale = new Vector3(fullScale.x * fillAmount, fullScale.y, fullScale.z);

        float offset = (fillBar.localScale.x - fullScale.x) * 0.5f;
        fillBar.localPosition = initialPosition + new Vector3(offset, 0, 0);
    }

    public IEnumerator UpdateFillCoroutine(int points)
    {
        float targetFillAmount = (float) currScore / 20;

        Vector3 targetScale = new Vector3(fullScale.x * targetFillAmount, fullScale.y, fullScale.z);

        float offset = (targetScale.x - fullScale.x) * 0.5f;
        Vector3 targetPosition = initialPosition + new Vector3(offset, 0, 0);

        float elapsedTime = 0f;
        float duration = 2.0f;

        while (elapsedTime < duration)
        {
            float t = elapsedTime / duration;

            // Interpolate scale and position
            fillBar.localScale = Vector3.Lerp(fillBar.localScale, targetScale, t);
            fillBar.localPosition = Vector3.Lerp(fillBar.localPosition, targetPosition, t);

            elapsedTime += Time.deltaTime;
            yield return null;
        }
        fillBar.localScale = targetScale;
        fillBar.localPosition = targetPosition;
    }
}
