using UnityEngine;
using UnityEngine.SceneManagement;

public class DeathSceneMainMenu : MonoBehaviour
{
    // This is the name of the scene you want to load
    private string sceneToLoad = "IntroScene";

    // This method runs when you press the Play button
     public void PlayGame()
    {
        Debug.Log("Play button pressed! Loading IntroScene...");
        SceneManager.LoadScene(sceneToLoad);
    }
}
