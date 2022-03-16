using UnityEngine;
using UnityEngine.SceneManagement;
public class IngameMenu : MonoBehaviour
{
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
            SceneManager.LoadScene("MainMenu");
    }
}
