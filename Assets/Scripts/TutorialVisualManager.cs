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
    public GameObject buildTutContinue;

    public GameObject robotTutorial;
    public GameObject robotWhatDo;
    public GameObject robotHowBuild;
    public GameObject robotTutButton;

    public GameObject weaponTutorial;
    public GameObject weaponWhatDo;
    public GameObject weaponHowBuild;
    public GameObject weaponTutButton;


    public Image robotIcon;
    public Image headWeaponIcon;
    public Image lArmWeaponIcon;
    public Image rArmWeaponIcon;
    public Image lLegWeaponIcon;
    public Image rLegWeaponIcon;
    public GameObject moreRobotInfo;
    public GameObject moreWeaponInfo;

    private bool seenRobotTut = false;
    private bool seenWeaponTut = false;

    private int tutorialStep;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        tutorialStep = 1;

        // Activate first darken
        darken1.SetActive(false);
        darken2.SetActive(false);
        darken3.SetActive(false);

        buildTutContinue.SetActive(false);
        robotTutorial.SetActive(false);
        weaponTutorial.SetActive(false);

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
                tutorialStep = 3;
            }
            else if (GameplayManager.Instance.playerList[0].WB2.bankData.Count > 0)
            {
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


    public void AdditionalRobotInfo()
    {
        seenRobotTut = true;
        buildTutorial.SetActive(false);
        robotTutorial.SetActive(true);
    }

    public void AdditionalWeaponInfo()
    {
        seenWeaponTut = true;
        buildTutorial.SetActive(false);
        weaponTutorial.SetActive(true);
    }

    public void ReturnBuildInfo()
    {
        robotTutorial.SetActive(false);
        weaponTutorial.SetActive(false);
        buildTutorial.SetActive(true);
        if (seenRobotTut)
        {
            robotTutButton.SetActive(false);
            if (!seenWeaponTut)
            {
                buildTutContinue.SetActive(false);
            }
        }
        if (seenWeaponTut)
        {
            weaponTutButton.SetActive(false);
            if (!seenRobotTut)
            {
                buildTutContinue.SetActive(false);
            }
        }
        if (seenRobotTut && seenWeaponTut)
        {
            buildTutContinue.SetActive(true);
        }
    }

    public void ToggleRobotDoInfo()
    {
        if(!robotWhatDo.activeSelf)
        {
            robotWhatDo.SetActive(true);
        }
        else
        {
            robotWhatDo.SetActive(false);
        }
    }

    public void ToggleRobotBuildInfo()
    {
        if(!robotHowBuild.activeSelf)
        {
            robotHowBuild.SetActive(true);
        }
        else
        {
            robotHowBuild.SetActive(false);
        }
    }

    public void ToggleWeaponDoInfo()
    {
        if(!weaponWhatDo.activeSelf)
        {
            weaponWhatDo.SetActive(true);
        }
        else
        {
            weaponWhatDo.SetActive(false);
        }
    }

    public void ToggleWeaponBuildInfo()
    {
        if(!weaponHowBuild.activeSelf)
        {
            weaponHowBuild.SetActive(true);
        }
        else
        {
            weaponHowBuild.SetActive(false);
        }
    }
}
