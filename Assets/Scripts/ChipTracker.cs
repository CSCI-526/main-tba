using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ChipTracker : MonoBehaviour
{
    public TMP_Text t;
    public bool isPot;

    public void updateValue(int chips)
    {
        string finalText = "";
        if (isPot)
        {
            finalText = "Pot: " + chips;
        }
        else
        {
            finalText = "Money: " + chips;
        }
        t.text = finalText;
    }
}
