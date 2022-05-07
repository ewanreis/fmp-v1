using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.SceneManagement;
public class PauseMenu : MonoBehaviour
{
    public GameObject pauseMenu;
    public AudioClip[] pauseSounds;
    public AudioSource vfxSource;
    public static bool isPaused;

    void Start() => pauseMenu.SetActive(false);

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (!isPaused)
                PauseGame();

            else
                ResumeGame();
        }
    }

    public void PauseGame()
    {
        vfxSource.PlayOneShot(pauseSounds[1]);
        pauseMenu.SetActive(true);
        Cursor.lockState = CursorLockMode.None;
        Time.timeScale = 0f;
        isPaused = true;
    }

    public void ResumeGame()
    {
        vfxSource.PlayOneShot(pauseSounds[0]);
        pauseMenu.SetActive(false);
        Cursor.lockState = CursorLockMode.Locked;
        Time.timeScale = 1f;
        isPaused = false;
    }
    public void OpenSubMenu() => vfxSource.PlayOneShot(pauseSounds[1]);
    public void CloseSubMenu() => vfxSource.PlayOneShot(pauseSounds[0]);
    public void ReturnToMenu() => SceneManager.LoadScene(0); 
    public void QuitGame() => Application.Quit();
}