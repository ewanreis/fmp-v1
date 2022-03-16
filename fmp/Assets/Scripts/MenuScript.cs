using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.Audio;
public class MenuScript : MonoBehaviour
{
    private bool buttonAvailable = true, isCreepy = false;
    private int level = 1, pIsFullscreen, pGraphicsPreset;
    private float pMasterVolume, pMusicVolume, pVfxVolume;

    public Slider masterSlider, musicSlider, vfxSlider;
    public GameObject settingsMenu, howToPlayScreen;
    public Button continueButton, backButton;

    public AudioClip[] clips;
    public AudioSource musicSource, vfxSource;
    public AudioMixer audioMixer;

    enum Sounds
    {
        continueHover, // 0
        continueClick, // 1
        clickDeny, // 2
        startClick, // 3
        hoverDeny, // 4
        startHover, // 5
        quitClick, // 6
        menuMusic, // 7
        menuMusicCreepy, // 8
        genericClick // 9
    }

    void Start()
    {
        pMasterVolume = PlayerPrefs.GetFloat("playerMasterVolume", 0);
        pMusicVolume = PlayerPrefs.GetFloat("playerMusicVolume", 0);
        pVfxVolume = PlayerPrefs.GetFloat("playerVfxVolume", 0);
        pIsFullscreen = PlayerPrefs.GetInt("playerFullscreen", 0);
        audioMixer.SetFloat("masterVolume", pMasterVolume);
        audioMixer.SetFloat("musicVolume", pMusicVolume);
        audioMixer.SetFloat("vfxVolume", pVfxVolume);
        masterSlider.value = pMasterVolume;
        musicSlider.value = pMusicVolume;
        vfxSlider.value = pVfxVolume;
        Screen.fullScreen = (pIsFullscreen == 0) ? false : true;
        Sounds music = (isCreepy) ? Sounds.menuMusicCreepy : Sounds.menuMusic;
        Cursor.lockState = CursorLockMode.None;
        settingsMenu.SetActive(false);
        howToPlayScreen.SetActive(false);
        musicSource.PlayOneShot(clips[(int)music], 0.2f);
        PlayerPrefs.Save();
    }

    public void ContinueGame()
    {
        Sounds buttonSound = (buttonAvailable) ? Sounds.continueClick : Sounds.clickDeny;
        vfxSource.PlayOneShot(clips[(int)buttonSound]);
        if (buttonAvailable)
            Invoke("LoadScene", 1.5f);
    }

    public void StartGame()
    {
        level = 2;
        vfxSource.PlayOneShot(clips[(int)Sounds.startClick]);
        Invoke("LoadScene", 1.5f);
    }

    public void QuitGame()
    {
        vfxSource.PlayOneShot(clips[(int)Sounds.quitClick]);
        Invoke("EndGame", 1f);
        PlayerPrefs.Save();
    }

    public void SetVolumeMaster(float volume)  
    {
        audioMixer.SetFloat("masterVolume", volume); 
        PlayerPrefs.SetFloat("playerMasterVolume", volume); 
    }

    public void SetVolumeMusic(float volume)
    {
        audioMixer.SetFloat("musicVolume", volume);
        PlayerPrefs.SetFloat("playerMusicVolume", volume);
    }

    public void SetVolumeVfx(float volume)
    {
        audioMixer.SetFloat("vfxVolume", volume);
        PlayerPrefs.SetFloat("playerVfxVolume", volume);
    }

    public void ChangeMenuScreen(bool isMain)
    {
        Button selectedButton = (isMain) ? continueButton : backButton;
        selectedButton.Select();
    }

    public void SetGraphicsPreset(int preset) => print(preset);
    public void SetFullscreen(bool isFullscreen) => Screen.fullScreen = isFullscreen;
    public void ContinueHover() => vfxSource.PlayOneShot(clips[ (buttonAvailable) ? (int)Sounds.continueHover : (int)Sounds.hoverDeny ]);
    public void StartHover() => vfxSource.PlayOneShot(clips[(int)Sounds.startHover]);
    public void GenericClick() => vfxSource.PlayOneShot(clips[(int)Sounds.genericClick]);

    private void EndGame() => Application.Quit();
    private void LoadScene() => SceneManager.LoadScene(level); 
}