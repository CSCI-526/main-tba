using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConveyorBeltAnim : MonoBehaviour
{
    public List<Vector3> originalPositions = new List<Vector3>();
    public IEnumerator ConveyorBeltAnimation(List<GameObject> riverCards)
    {
        GameplayManager.Instance.ToggleOffInteractives();
        
        //Vector3 xshift = new Vector3(70f, 0f, 0f);
        //Vector3 yshift = new Vector3(0f, 70f, 0f);
        for (int i = 0; i < riverCards.Count; i++){
            originalPositions.Add(riverCards[i].transform.position);
            riverCards[i].transform.position = originalPositions[0];
            riverCards[i].SetActive(false);
        }

        yield return StartCoroutine(MoveCards1());
        IEnumerator MoveCards1()
        {
            riverCards[4].SetActive(true);
            StartCoroutine(MoveObject(riverCards[4] ,originalPositions[0] , originalPositions[1], 0.5f));
            yield return new WaitForSeconds(0.5f);
        }
        yield return StartCoroutine(MoveCards2());
        IEnumerator MoveCards2()
        {
            riverCards[3].SetActive(true);
            StartCoroutine(MoveObject(riverCards[4] ,originalPositions[1] , originalPositions[2], 0.5f));
            StartCoroutine(MoveObject(riverCards[3] ,originalPositions[0] , originalPositions[1], 0.5f));
            yield return new WaitForSeconds(0.5f);
        }
        yield return StartCoroutine(MoveCards3());
        IEnumerator MoveCards3()
        {
            riverCards[2].SetActive(true);
            StartCoroutine(MoveObject(riverCards[4] ,originalPositions[2] , originalPositions[3], 0.5f));
            StartCoroutine(MoveObject(riverCards[3] ,originalPositions[1] , originalPositions[2], 0.5f));
            StartCoroutine(MoveObject(riverCards[2] ,originalPositions[0] , originalPositions[1], 0.5f));
            yield return new WaitForSeconds(0.5f);
        }
        yield return StartCoroutine(MoveCards4());
        IEnumerator MoveCards4()
        {
            riverCards[1].SetActive(true);
            StartCoroutine(MoveObject(riverCards[4] ,originalPositions[3] , originalPositions[4], 0.5f));
            StartCoroutine(MoveObject(riverCards[3] ,originalPositions[2] , originalPositions[3], 0.5f));
            StartCoroutine(MoveObject(riverCards[2] ,originalPositions[1] , originalPositions[2], 0.5f));
            StartCoroutine(MoveObject(riverCards[1] ,originalPositions[0] , originalPositions[1], 0.5f));
            yield return new WaitForSeconds(0.5f);
        }
        yield return StartCoroutine(MoveCards5());
        IEnumerator MoveCards5()
        {
            riverCards[0].SetActive(true);
            yield return new WaitForSeconds(0.2f);
        }
        Debug.Log("CB Anim");
        
        GameplayManager.Instance.ToggleOnInteractives();
    }

    private IEnumerator MoveObject(GameObject obj, Vector3 startPos, Vector3 endPos, float duration)
    {
        float elapsedTime = 0;

        while (elapsedTime < duration)
        {
            obj.transform.position = Vector3.Lerp(startPos, endPos, elapsedTime / duration);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        obj.transform.position = endPos; // Ensure final position is exactly what we want
    }
}