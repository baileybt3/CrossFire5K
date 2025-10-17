using UnityEngine;
using UnityEngine.SceneManagement;

public class BackToMenu : MonoBehaviour
{
    public void GoBack()
    {
        SceneManager.LoadScene("01_MainMenu");
    }
}
