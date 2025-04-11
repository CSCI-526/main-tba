using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class AnalyticsManager : MonoBehaviour
{
    [System.Serializable]
    private class LoginPayload
    {
        public string email;
        public string password;
        public bool returnSecureToken = true;
    }

    [System.Serializable]
    public class TokenOnly
    {
        public string idToken;
    }

    //This DB is NOT locked currently
    private string firebaseURL = "https://botorbought-85297-default-rtdb.firebaseio.com/";
    public static AnalyticsManager Instance { get; private set; }

    //didn't end up using the auth or user imported packages
    
    private string idToken = null;
    private string email = "csci526@teamtba.com";
    private string password = "SecurePassword213?bot";
    private string apiKey = "AIzaSyB3ImcBHHO7FPE6wm_ISLv0q_FWHhIPVBw";

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

        StartCoroutine(SignInAndGetToken());
    }

    private IEnumerator SignInAndGetToken()
    {
        //https://firebase.google.com/docs/reference/rest/auth#section-sign-in-email-password
        string loginUrl = $"https://identitytoolkit.googleapis.com/v1/accounts:signInWithPassword?key={apiKey}";

        LoginPayload payload = new LoginPayload
        {
            email = this.email,
            password = this.password,
            returnSecureToken = true
        };

        string jsonPayload = JsonUtility.ToJson(payload);

        using (UnityWebRequest request = new UnityWebRequest(loginUrl, "POST"))
        {
            byte[] jsonToSend = new System.Text.UTF8Encoding().GetBytes(jsonPayload);
            using UploadHandlerRaw uploadHandler = new UploadHandlerRaw(jsonToSend);
            request.uploadHandler = uploadHandler;
            request.downloadHandler = new DownloadHandlerBuffer();
            request.disposeUploadHandlerOnDispose = true;
            request.disposeDownloadHandlerOnDispose = true;
            request.SetRequestHeader("Content-Type", "application/json");
            request.timeout = 5;

            yield return request.SendWebRequest();

            if (request.result != UnityWebRequest.Result.Success)
            {
                Debug.Log("Firebase sign in failed!");
                Debug.LogError($"Error: {request.error}");
                Debug.LogError($"Response: {request.downloadHandler.text}");
            }
            else
            {
                string json = request.downloadHandler.text;
                TokenOnly tokenResponse = JsonUtility.FromJson<TokenOnly>(json);
                idToken = tokenResponse.idToken;
                Debug.Log(idToken);
                Debug.Log("Firebase sign in successful, token acquired!");
            }
        }
    }

    public void LogWorkbenchSale(List<CardData> soldBankCards, int totalPointsAwarded)
    {
        string url = $"{firebaseURL}workbench_sales.json?auth={idToken}";

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

    public void LogGameTurns(int totalTurns, int totalPassTurns, int winnerPlayerNum)
    {
        string url = $"{firebaseURL}game_turns.json?auth={idToken}";

        GameTurnsCounts turns = new GameTurnsCounts(totalTurns, totalPassTurns, winnerPlayerNum);
        string json = JsonUtility.ToJson(turns);
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
