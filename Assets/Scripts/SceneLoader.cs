using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoader : MonoBehaviour
{
    [SerializeField]
    private GameObject tutMessage1;
    [SerializeField]
    private GameObject tutMessage2;
    [SerializeField]
    private GameObject menuCanvas;

    void Start()
    {
        menuCanvas.SetActive(true);
        tutMessage1.SetActive(false);
        tutMessage2.SetActive(false);
    }

    // This function loads the Game Scene
    public void StartGame()
    {
        SceneManager.LoadScene(1); // Loads scene by index (Game Scene)
    }
    public void RestartGame()
    {
        SceneManager.LoadScene(1); // Loads scene by index (Game Scene)
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
        tutMessage1.SetActive(false);
        tutMessage2.SetActive(false);
    }


}
