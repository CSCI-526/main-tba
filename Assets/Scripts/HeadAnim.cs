using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class HeadAnim : MonoBehaviour
{
    public AudioSource headSound;
    public AudioSource cashSound;

    public IEnumerator HeadAnimation(Bank wb, int playerNum, List<CardData> sellData, int benchNum)
    {
        SpriteRenderer sr = this.GetComponent<SpriteRenderer>();
        sr.sortingOrder = 50;

        Vector3 goToPosition = new Vector3(transform.position.x, playerNum == 1 ? transform.position.y + 215.0f : transform.position.y - 215.0f, 0.0f);
        Vector3 returnPosition = transform.position;
        // Step 1: Scale up
        yield return StartCoroutine(ScaleObject(1.0f));

        // Step 2: Move toward other wb
        yield return StartCoroutine(MoveHead(1.0f, goToPosition));
        // Step 3: Stand still and scan
        yield return StartCoroutine(Jiggle(1.5f));
        // Step 4: Return to own workbench
        yield return StartCoroutine(MoveHead(1.0f, returnPosition));

        // Step 4: Do the ability function here
        wb.head_ability.Activate(sellData.Count, wb, benchNum);
        cashSound.Play(0);
        //Analytics
        AnalyticsManager.Instance.LogWorkbenchSale(wb.bankData, 0);

        Debug.Log("Points awarded via Head ability");
        sr.sortingOrder = 15;
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

    private IEnumerator MoveHead(float duration, Vector3 targetLocation)
    {
        float elapsedTime = 0;
        Vector3 startPos = transform.position;

        while (elapsedTime < duration)
        {
            transform.position = Vector3.Lerp(startPos, targetLocation, elapsedTime / duration);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        transform.position = targetLocation;
    }

    private IEnumerator Jiggle(float duration)
    {
        headSound.Play(0);
        float elapsedTime = 0f;
        Vector3 originalPosition = transform.position;

        while (elapsedTime < duration)
        {
            // Calculate jiggle offset with decreasing intensity as we approach the end
            float normalizedTime = elapsedTime / duration;

            // Random position offset
            Vector3 positionOffset = new Vector3(
                Random.Range(-5.0f, 5.0f),
                Random.Range(-5.0f, 5.0f),
                0.0f
            );
            transform.position = originalPosition + positionOffset;
            yield return null;

            elapsedTime += Time.deltaTime;
        }

        transform.position = originalPosition;
    }

    private IEnumerator Delay(float seconds)
    {
        headSound.Play(0);
        yield return new WaitForSeconds(seconds);
    }
}

