using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class TutorialVisualManager : MonoBehaviour
{
    public GameObject darken1;
    public GameObject frame1;
    public GameObject darken2;
    public GameObject frame2;
    public GameObject darken3;
    public GameObject buildTutorial;
    public TMP_Text chosenColor;
    public TMP_Text weaponText;
    public Image robotIcon;
    public Image headWeaponIcon;
    public Image lArmWeaponIcon;
    public Image rArmWeaponIcon;
    public Image lLegWeaponIcon;
    public Image rLegWeaponIcon;
    public GameObject moreRobotInfo;
    public GameObject moreWeaponInfo;
    private int tutorialStep;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        tutorialStep = 1;
        darken1.SetActive(false);
        darken2.SetActive(false);
        darken3.SetActive(false);

        headWeaponIcon.gameObject.SetActive(false);
        lArmWeaponIcon.gameObject.SetActive(false);
        rArmWeaponIcon.gameObject.SetActive(false);
        lLegWeaponIcon.gameObject.SetActive(false);
        rLegWeaponIcon.gameObject.SetActive(false);

        buildTutorial.SetActive(false);
        moreRobotInfo.SetActive(false);
        moreWeaponInfo.SetActive(false);
    
    }

    // Update is called once per frame
    void Update()
    {
        switch (tutorialStep)
        {
            case 1:
            darken1.SetActive(true);

            if (GameplayManager.Instance.selected_cards.Count > 0)
            {
                tutorialStep = 2;
            }
            break;

            case 2:
            darken1.SetActive(false);
            darken2.SetActive(true);

            frame1.SetActive(false);
            frame2.SetActive(true);

            
            if (GameplayManager.Instance.playerList[0].WB1.bankData.Count > 0)
            {
                SetTextIcons(GameplayManager.Instance.playerList[0].WB1.bankData[0]);
                tutorialStep = 3;
            }
            else if (GameplayManager.Instance.playerList[0].WB2.bankData.Count > 0)
            {
                SetTextIcons(GameplayManager.Instance.playerList[0].WB2.bankData[0]);
                tutorialStep = 3;
            }
            
            break;

            case 3:
            darken1.SetActive(false);
            darken2.SetActive(false);
            darken3.SetActive(true);

            frame1.SetActive(true);
            frame2.SetActive(true);

            break;
        }
    }

    void SetTextIcons(CardData choice)
    {
        string colorChoice = choice.cardSuit.ToString();
        string partChoice = choice.cardValue.ToString();

        chosenColor.text = colorChoice;

        switch (colorChoice)
        {
            case "Black":
            chosenColor.color = Color.black;
            robotIcon.color = Color.black;
            break;

            case "Blue":
            chosenColor.color = Color.blue;
            robotIcon.color = Color.blue;
            break;

            case "Red":
            chosenColor.color = Color.red;
            robotIcon.color = Color.red;
            break;

            case "Green":
            chosenColor.color = Color.green;
            robotIcon.color = Color.green;
            break;

            case "Gold":
            chosenColor.color = Color.yellow;
            robotIcon.color = Color.yellow;
            break;

            case "empty":
            chosenColor.color = Color.white;
            robotIcon.color = Color.white;
            break;
        }

        switch (partChoice)
        {
            case "Head":
            headWeaponIcon.gameObject.SetActive(true);
            break;

            case "LeftArm":
            lArmWeaponIcon.gameObject.SetActive(true);
            break;

            case "RightArm":
            rArmWeaponIcon.gameObject.SetActive(true);
            break;

            case "LeftFoot":
            lLegWeaponIcon.gameObject.SetActive(true);
            break;

            case "RightFoot":
            rLegWeaponIcon.gameObject.SetActive(true);
            break;
        }

        weaponText.text += partChoice + " parts =";
    }

    public void AdditionalRobotInfo()
    {
        if (!moreRobotInfo.activeInHierarchy)
        {
            moreRobotInfo.SetActive(true);
        }
        else
        {
            moreRobotInfo.SetActive(false);
        }
    }

    public void AdditionalWeaponInfo()
    {
        if (!moreWeaponInfo.activeInHierarchy)
        {
            moreWeaponInfo.SetActive(true);
        }
        else
        {
            moreWeaponInfo.SetActive(false);
        }
    }
}
