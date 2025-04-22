using UnityEngine;
using System.Collections;

public class FootAnim : MonoBehaviour
{
    public AudioSource footSound;
    public AudioSource cashSound;

    public IEnumerator FootAnimation(Bank wb, bool isLeft)
    {
        int point_award = 0;
        SpriteRenderer sr = this.GetComponent<SpriteRenderer>();
        sr.sortingOrder = 50;
        // Step 1: Scale up
        yield return StartCoroutine(ScaleObject(1.0f));

        // Step 2: Move toward river
        yield return StartCoroutine(MoveFoot(1.0f, isLeft));

        // Step 3: Jerk up and down
        yield return StartCoroutine(StompAction(0.5f, 2));

        // Step 4: Award Points
        point_award = wb.foot_ability.Activate();
        GameplayManager.Instance.AwardPoints(point_award);
        cashSound.Play(0);
        //Analytics 
        AnalyticsManager.Instance.LogWorkbenchSale(wb.bankData, point_award);

        Debug.Log("Points awarded via Foot ability");
        sr.sortingOrder = 15;
        wb.cleanUpBench();
    }

    private IEnumerator ScaleObject(float duration)
    {
        float elapsedTime = 0;
        Vector3 endScale = new Vector3(1.2f, 2.0f, 1.0f);

        while (elapsedTime < duration)
        {
            transform.localScale = Vector3.Lerp(new Vector3(0.6f, 1.0f, 1.0f), endScale, elapsedTime / duration);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        transform.localScale = endScale; // Ensure final scale is exactly what we want
    }

    private IEnumerator MoveFoot(float duration, bool isLeft)
    {
        float elapsedTime = 0;
        Vector3 startPos = transform.position;
        Vector3 endPos = isLeft ? new Vector3(-115.0f, 30.0f, 0.0f) : new Vector3(85.0f, 30.0f, 0.0f);

        while (elapsedTime < duration)
        {
            transform.position = Vector3.Lerp(startPos, endPos, elapsedTime / duration);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        transform.position = endPos;
    }

    private IEnumerator StompAction(float duration, int stomp)
    {
        Vector3 basePosition = transform.position;
        float stompHeight = 60.0f;

        for (int i = 0; i < stomp; i++)
        {
            // Jerk up
            yield return StartCoroutine(MoveObject(basePosition, basePosition + Vector3.up * stompHeight, duration));
            if (i == 0)
            {
                footSound.Play(0);
            }
            // Jerk down
            yield return StartCoroutine(MoveObject(basePosition + Vector3.up * stompHeight, basePosition, duration));
        }
    }

    private IEnumerator MoveObject(Vector3 startPos, Vector3 endPos, float duration)
    {
        float elapsedTime = 0;

        while (elapsedTime < duration)
        {
            transform.position = Vector3.Lerp(startPos, endPos, elapsedTime / duration);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        transform.position = endPos; // Ensure final position is exactly what we want
    }
}
