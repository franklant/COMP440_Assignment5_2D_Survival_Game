using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    // This is the name of the scene you want to load
    private string sceneToLoad = "MainScene";

    // This method runs when you press the Play button
     public void PlayGame()
    {
        Debug.Log("Play button pressed! Loading MainScene...");
        SceneManager.LoadScene(sceneToLoad);
    }
}
