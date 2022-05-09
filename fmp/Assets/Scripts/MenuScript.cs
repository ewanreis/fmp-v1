using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.Audio;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class MenuScript : MonoBehaviour
{
    private bool postProcessingEnabled;
    private int pIsFullscreen, pGraphicsPreset, pPostProcessingEnabled;
    private float pMasterVolume, pMusicVolume, pVfxVolume, pBrightness;

    public Slider masterSlider, musicSlider, vfxSlider, brightnessSlider;
    public GameObject settingsMenu, howToPlayScreen;
    public Button newGameButton, backButton;

    public AudioClip[] clips;
    public AudioSource musicSource, vfxSource;
    public AudioMixer audioMixer;

    public Volume renderingVolume;
    public LiftGammaGain liftGammaGain;
    public Tonemapping toneMap;
    public FilmGrain filmGrain;
    public Vignette vignette;
    public DepthOfField depthOfField;

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

    private void Start()
    {
        Time.timeScale = 1;
        SetPlayerPreferences();
    }

    private void SetPlayerPreferences()
    {
        // Gets the post processing volume profiles
        if (!renderingVolume.profile.TryGet(out liftGammaGain)) throw new System.NullReferenceException(nameof(liftGammaGain));
        if (!renderingVolume.profile.TryGet(out toneMap)) throw new System.NullReferenceException(nameof(toneMap));
        if (!renderingVolume.profile.TryGet(out filmGrain)) throw new System.NullReferenceException(nameof(filmGrain));
        if (!renderingVolume.profile.TryGet(out vignette)) throw new System.NullReferenceException(nameof(vignette));
        if (!renderingVolume.profile.TryGet(out depthOfField)) throw new System.NullReferenceException(nameof(depthOfField));

        // Gets all of the variables from the Player Prefs
        pBrightness = PlayerPrefs.GetFloat("playerBrightness", 0.75f);
        pMasterVolume = PlayerPrefs.GetFloat("playerMasterVolume", 0);
        pMusicVolume = PlayerPrefs.GetFloat("playerMusicVolume", 0);
        pVfxVolume = PlayerPrefs.GetFloat("playerVfxVolume", 0);
        pIsFullscreen = PlayerPrefs.GetInt("playerFullscreen", 0);
        pPostProcessingEnabled = PlayerPrefs.GetInt("playerPostProcessingEnabled", 0);

        // Setting the volume on the audio mixer channels
        audioMixer.SetFloat("masterVolume", pMasterVolume);
        audioMixer.SetFloat("musicVolume", pMusicVolume);
        audioMixer.SetFloat("vfxVolume", pVfxVolume);

        // Setting the menu sliders to correspond to the saved Player Pref values
        masterSlider.value = pMasterVolume;
        musicSlider.value = pMusicVolume;
        vfxSlider.value = pVfxVolume;
        brightnessSlider.value = pBrightness;

        // Toggling post processing and fullscreen based on player prefs
        postProcessingEnabled = (pPostProcessingEnabled == 0) ? false : true;
        Screen.fullScreen = (pIsFullscreen == 0) ? false : true;

        Cursor.lockState = CursorLockMode.None;

        settingsMenu.SetActive(false);
        howToPlayScreen.SetActive(false);
        musicSource.PlayOneShot(clips[(int)Sounds.menuMusic], 0.2f);
        SetGammaAlpha(pBrightness);
        TogglePP(postProcessingEnabled);
        PlayerPrefs.Save();
    }


    public void StartGame()
    {
        vfxSource.PlayOneShot(clips[(int)Sounds.startClick]);
        Invoke("LoadMainMap", 1.5f);
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
        Button selectedButton = (isMain) ? newGameButton : backButton;
        selectedButton.Select();
    }

    public void TogglePP(bool enabled)
    {
        toneMap.active = enabled;
        vignette.active = enabled;
        filmGrain.active = enabled;
        depthOfField.active = enabled;
        if(enabled)
        {
            QualitySettings.antiAliasing = 8;
            PlayerPrefs.SetInt("playerPostProcessingEnabled", 1);
        }
        else
        {
            QualitySettings.antiAliasing = 0;
            PlayerPrefs.SetInt("playerPostProcessingEnabled", 0);
        }
    }

    public void SetGammaAlpha(float gammaAlpha) 
    {
         liftGammaGain.gamma.Override(new Vector4(1f, 1f, 1f, gammaAlpha));
         PlayerPrefs.SetFloat("playerBrightness", gammaAlpha);
    }

    public void SetGraphicsPreset(int preset) => print(preset);
    public void SetFullscreen(bool isFullscreen) => Screen.fullScreen = isFullscreen;
    public void StartHover() => vfxSource.PlayOneShot(clips[(int)Sounds.startHover]);
    public void GenericClick() => vfxSource.PlayOneShot(clips[(int)Sounds.genericClick]);

    private void EndGame() => Application.Quit();
    private void LoadMainMap() 
    {
        print("hey");
        SceneManager.LoadScene("MainMap", LoadSceneMode.Single); 
    } 
}