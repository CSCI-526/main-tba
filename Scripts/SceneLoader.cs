using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoader : MonoBehaviour
{
    // This function loads the Game Scene
    public void StartGame()
    {
        SceneManager.LoadScene(1); // Loads scene by index (Game Scene)
    }
}
