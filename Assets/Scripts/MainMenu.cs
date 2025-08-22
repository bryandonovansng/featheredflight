using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    public void StartGame()
    {
        SceneManager.LoadScene("FlyingLevel"); // Replace with your actual gameplay scene name
    }

    public void ExitGame()
    {
        Debug.Log("Quit game.");
        Application.Quit();
    }
}