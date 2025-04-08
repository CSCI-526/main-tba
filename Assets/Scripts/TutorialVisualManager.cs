using UnityEngine;

public class TutorialVisualManager : MonoBehaviour
{
    public GameObject darken1;
    public GameObject frame1;
    public GameObject darken2;
    public GameObject frame2;
    public GameObject darken3;
    private int tutorialStep;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        tutorialStep = 1;
        darken1.SetActive(false);
        darken2.SetActive(false);
        darken3.SetActive(false);
        
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

            if (GameplayManager.Instance.playerList[0].WB1.bankData.Count > 0 || GameplayManager.Instance.playerList[0].WB2.bankData.Count > 0)
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
}
