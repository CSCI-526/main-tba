using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class FootAbility : IAbility
{
    private bool left_;
    public int duplicate_count_;
    
    public void Activate(int duplicateCount, Bank workBench)
    {
        Debug.Log("FootAbility Activated" + left_);
    }

    public int Activate()
    {
        Debug.Log("FootAbility Activated" + left_ + "dupliate count" + duplicate_count_);
        
        // find the left two or right two if existed
        int num_card_left = 0;
        int card1 = 3;
        int card2 = 4;
        if (left_)
        {
            card1 = 0;
            card2 = 1;
        }
        for (int i = 0; i < GameplayManager.Instance.river.riverData.Count; ++i)
        {
            CardData currentCard = GameplayManager.Instance.river.riverData[i];
            if (currentCard.pos == card1 ||
                currentCard.pos == card2)
            {
                num_card_left++;
                // delete card from river
                GameplayManager.Instance.river.LocateAndDelete(currentCard);
                i--;
            }
        }

        /*if (left_)
        {
            GameplayManager.Instance.msg.text = "Award Using left foot: Remain left " + num_card_left + " * heap size " +
                                                duplicate_count_ + " = " + num_card_left * duplicate_count_;
        }
        else
        {
            GameplayManager.Instance.msg.text = "Award using right foot: Remain left " + num_card_left + " * heap size " +
                                                duplicate_count_ + " = " + num_card_left * duplicate_count_;
        }*/
        
        return num_card_left * duplicate_count_;
    }

    public void setLeft(bool value)
    {
        left_ = value;
    }

    public bool getLeft()
    {
        return left_;
    }
}
