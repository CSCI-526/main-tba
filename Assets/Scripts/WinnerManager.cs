using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class WinnerManager : MonoBehaviour
{
    public TMP_Text winnerText;

    // Start is called before the first frame update
    void Start()
    {
        winnerText.text = Winner.gameWinner + " is winner!";
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
