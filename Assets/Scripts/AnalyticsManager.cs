using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class AnalyticsManager : MonoBehaviour
{
    //This DB is NOT locked currently
    private string firebaseURL = "https://botorbought-default-rtdb.firebaseio.com/";
    public static AnalyticsManager Instance { get; private set; }

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(this.gameObject);
        }
        else
        {
            Instance = this;
            DontDestroyOnLoad(this.gameObject);
        }
    }

    public void LogWorkbenchSale(List<CardData> soldBankCards, int totalPointsAwarded)
    {
        string url = $"{firebaseURL}workbench_sales.json";

        //take the carddata given and generate a name string from that
        List<string> soldCardNames = new List<string>();
        foreach (CardData card in soldBankCards)
        {
            string cardName = card.cardValue.ToString() + card.cardSuit.ToString();
            soldCardNames.Add(cardName);
        }

        WorkBenchSale sale = new WorkBenchSale(soldCardNames, totalPointsAwarded);
        string json = JsonUtility.ToJson(sale);
        StartCoroutine(PostRequest(url, json));
    }

    IEnumerator PostRequest(string url, string json)
    {
        using (UnityWebRequest request = new UnityWebRequest(url, "POST"))
        {
            byte[] jsonToSend = new System.Text.UTF8Encoding().GetBytes(json);
            using UploadHandlerRaw uploadHandler = new UploadHandlerRaw(jsonToSend);
            request.uploadHandler = uploadHandler;
            request.downloadHandler = new DownloadHandlerBuffer();
            request.disposeUploadHandlerOnDispose = true;
            request.disposeDownloadHandlerOnDispose = true;
            request.SetRequestHeader("Content-Type", "application/json");
            request.timeout = 5;

            yield return request.SendWebRequest();

            if(request.result != UnityWebRequest.Result.Success)
            {
                Debug.Log("Post request failed!");
            } else
            {
                Debug.Log("Post request succeeded!");
            }
        }
    }
}
