using UnityEngine;
using UnityEngine.SceneManagement;

public class TutorialLoader : MonoBehaviour
{
    [SerializeField]
    private GameObject tutMessage1;
    [SerializeField]
    private GameObject tutMessage2;
    [SerializeField]
    private GameObject menuCanvas;

    [SerializeField]
    private GameObject tutPage0;
    [SerializeField]
    private GameObject tutPage1;
    [SerializeField]
    private GameObject tutPage2;
    [SerializeField]
    private GameObject tutPage3;
    [SerializeField]
    private GameObject tutPage4;
    [SerializeField]
    private GameObject tutPage5;
    [SerializeField]
    private GameObject tutPage6;
    
    void Start()
    {
        menuCanvas.SetActive(true);
        tutMessage1.SetActive(false);
        tutMessage2.SetActive(false);
    }

    public void ShowTut1()
    {
        menuCanvas.SetActive(false);
        tutMessage1.SetActive(true);
        tutMessage2.SetActive(false);
    }

    public void ShowTut2()
    {
        menuCanvas.SetActive(false);
        tutMessage1.SetActive(false);
        tutMessage2.SetActive(true);
    }

    public void ReturnMainMenu()
    {
        menuCanvas.SetActive(true);
        tutPage1.SetActive(false);
        tutPage2.SetActive(false);
        tutPage3.SetActive(false);
        tutPage4.SetActive(false);
        tutPage5.SetActive(false);
        tutPage6.SetActive(false);
        GameplayManager.Instance.in_game_tutorial = false;
        GameplayManager.Instance.ToggleCards();
        GameplayManager.Instance.ToggleWBs();
        GameplayManager.Instance.cards_tmp_holder.Clear();
        GameplayManager.Instance.wbs_tmp_holder.Clear();
    }
    
    public void ShowTutPage0()
    {
        GameplayManager.Instance.in_game_tutorial = true;
        GameplayManager.Instance.ToggleCards();
        GameplayManager.Instance.ToggleWBs();
        menuCanvas.SetActive(false);
        tutPage0.SetActive(true);
        tutPage1.SetActive(false);
        tutPage2.SetActive(false);
        tutPage3.SetActive(false);
        tutPage4.SetActive(false);
        tutPage5.SetActive(false);
        tutPage6.SetActive(false);
    }

    public void ShowTutPage1()
    {
        menuCanvas.SetActive(false);
        tutPage0.SetActive(false);
        tutPage1.SetActive(true);
        tutPage2.SetActive(false);
        tutPage3.SetActive(false);
        tutPage4.SetActive(false);
        tutPage5.SetActive(false);
        tutPage6.SetActive(false);
    }
    public void ShowTutPage2()
    {
        menuCanvas.SetActive(false);
        tutPage0.SetActive(false);
        tutPage1.SetActive(false);
        tutPage2.SetActive(true);
        tutPage3.SetActive(false);
        tutPage4.SetActive(false);
        tutPage5.SetActive(false);
        tutPage6.SetActive(false);
    }
    public void ShowTutPage3()
    {
        menuCanvas.SetActive(false);
        tutPage0.SetActive(false);
        tutPage1.SetActive(false);
        tutPage2.SetActive(false);
        tutPage3.SetActive(true);
        tutPage4.SetActive(false);
        tutPage5.SetActive(false);
        tutPage6.SetActive(false);
    }
    public void ShowTutPage4()
    {
        menuCanvas.SetActive(false);
        tutPage0.SetActive(false);
        tutPage1.SetActive(false);
        tutPage2.SetActive(false);
        tutPage3.SetActive(false);
        tutPage4.SetActive(true);
        tutPage5.SetActive(false);
        tutPage6.SetActive(false);
    }
    public void ShowTutPage5()
    {
        menuCanvas.SetActive(false);
        tutPage0.SetActive(false);
        tutPage1.SetActive(false);
        tutPage2.SetActive(false);
        tutPage3.SetActive(false);
        tutPage4.SetActive(false);
        tutPage5.SetActive(true);
        tutPage6.SetActive(false);
    }
    public void ShowTutPage6()
    {
        menuCanvas.SetActive(false);
        tutPage0.SetActive(false);
        tutPage1.SetActive(false);
        tutPage2.SetActive(false);
        tutPage3.SetActive(false);
        tutPage4.SetActive(false);
        tutPage5.SetActive(false);
        tutPage6.SetActive(true);
    }
}
