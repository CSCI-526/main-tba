using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;


public class ArmAnim : MonoBehaviour
{
    public AudioSource punchSound;
    public AudioSource cashSound;

    public IEnumerator ArmAnimation(Bank wb, bool isLeft, int playerNum, List<CardData> sellData)
    {
        GameplayManager.Instance.ToggleOffInteractives();
        SpriteRenderer sr = this.GetComponent<SpriteRenderer>();
        sr.sortingOrder = 50;
        Vector3 originalPosition = transform.position;

        Vector3 windUpPosition = new Vector3(isLeft ? -100f : 90f, playerNum == 1 ? transform.position.y - 60.0f : transform.position.y + 60.0f, 0.0f);
        Vector3 punchPosition = new Vector3(isLeft ? -100f : 90f, playerNum == 1 ? transform.position.y + 224.0f : transform.position.y - 224.0f, 0.0f);
        // Step 1: Scale up
        yield return StartCoroutine(ScaleObject(1.0f));

        // Step 2: Move toward wind up position
        yield return StartCoroutine(MoveArm(1.0f, windUpPosition, false, playerNum == 1));
        // Step 3: Throw that punch
        yield return StartCoroutine(MoveArm(0.5f, punchPosition, true, false));

        if (wb.arm_ability == null)
        {
            wb.arm_ability = new ArmAbility();
        }
        // Step 4: Do the ability function here
        if (isLeft)
            wb.arm_ability.setLeft(true);
        else
            wb.arm_ability.setLeft(false);
        wb.arm_ability.Activate(sellData.Count, wb);
        cashSound.Play(0);
        //Analytics 
        if (SceneManager.GetActiveScene().name != "TutorialScene")
        {
            AnalyticsManager.Instance.LogWorkbenchSale(wb.bankData, 0);

            Debug.Log("Points awarded via Arm ability");
            sr.sortingOrder = 15;
            wb.cleanUpBench();
            cleanUp(originalPosition);
            GameplayManager.Instance.ToggleOnInteractives();
        }
    }

    private IEnumerator ScaleObject(float duration)
    {
        float elapsedTime = 0;
        Vector3 endScale = new Vector3(2.0f, 2.0f, 2.0f);

        while (elapsedTime < duration)
        {
            transform.localScale = Vector3.Lerp(new Vector3(1.0f, 1.0f, 1.0f), endScale, elapsedTime / duration);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        transform.localScale = endScale; // Ensure final scale is exactly what we want
    }

    private IEnumerator MoveArm(float duration, Vector3 targetLocation, bool PlaySound, bool rotate)
    {
        float elapsedTime = 0;
        Vector3 startPos = transform.position;
        Quaternion startRotation = transform.rotation;
        Vector3 originalScale = transform.localScale;
        float aspectRatio = GetComponent<SpriteRenderer>().sprite.bounds.size.x /
                             GetComponent<SpriteRenderer>().sprite.bounds.size.y;

        if (PlaySound)
        {
            punchSound.Play(0);
        }

        while (elapsedTime < duration)
        {
            transform.position = Vector3.Lerp(startPos, targetLocation, elapsedTime / duration);
            if (rotate)
            {
                // also need to rotate arm for P1
                transform.rotation = Quaternion.Slerp(startRotation, Quaternion.Euler(0.0f, 0.0f, 180.0f), elapsedTime / duration);
            }
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        transform.position = targetLocation;
        if (rotate)
        {
            transform.rotation = Quaternion.Euler(0.0f, 0.0f, 180.0f);
        }
    }

    private void cleanUp (Vector3 originalPosition) {
        transform.position = originalPosition;
        transform.rotation = Quaternion.Euler(0.0f, 0.0f, 0.0f);
        transform.localScale = new Vector3(1.0f, 1.0f, 1.0f);
        }
}

