using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;
using UnityEngine.Audio;
using UnityEngine.Rendering;
using UnityEngine.UI;
using TMPro;

public class MapManager : MonoBehaviour
{
    #region Audio Settings
    [Header("Audio")]
    public AudioMixer audioMixer;
    public AudioSource musicSource, vfxSource;
    public Slider masterSlider, musicSlider, vfxSlider;
    public AudioClip[] clips;
    #endregion

    #region Graphics Settings
    [Header("Post Processing")]
    public Volume renderingVolume;
    public LiftGammaGain liftGammaGain;
    public Tonemapping toneMap;
    public FilmGrain filmGrain;
    public Vignette vignette;
    public DepthOfField depthOfField;
    public ChromaticAberration chromaticAberration;

    [Header("Graphics Quality")]
    public Slider brightnessSlider;
    public Toggle fullscreenToggle, postProcessingToggle;
    public TMP_Dropdown distanceDropdown, densityDropdown, qualityDropdown;

    [Header("Nature")]
    public Terrain terrain;
    public float grassDistance, grassDensity;
    #endregion

    #region Player Preferences
    [Header("Player Preferences")]
    public bool postProcessingEnabled;
    private int pIsFullscreen, pGraphicsPreset, pPostProcessingEnabled, pDensityLevel, pDistanceLevel;
    public float pMasterVolume, pMusicVolume, pVfxVolume, pBrightness;
    #endregion

    private float chromaIntensity = 0f;

    void Start() => SetPlayerPreferences();
    private void SetPlayerPreferences()
    {
        // Gets all of the variables from the Player Prefs
        pPostProcessingEnabled = PlayerPrefs.GetInt("playerPostProcessingEnabled", 1);
        pGraphicsPreset = PlayerPrefs.GetInt("playerGraphicsPreset", 1);
        pIsFullscreen = PlayerPrefs.GetInt("playerFullscreen", 0);
        pBrightness = PlayerPrefs.GetFloat("playerBrightness", 0.75f);
        pMasterVolume = PlayerPrefs.GetFloat("playerMasterVolume", 0);
        pMusicVolume = PlayerPrefs.GetFloat("playerMusicVolume", 0);
        pVfxVolume = PlayerPrefs.GetFloat("playerVfxVolume", 0);
        pDensityLevel = PlayerPrefs.GetInt("playerDensityLevel", 1);
        pDistanceLevel = PlayerPrefs.GetInt("playerDistanceLevel", 1);

        // Gets the post processing volume profiles
        if (!renderingVolume.profile.TryGet(out liftGammaGain)) throw new System.NullReferenceException(nameof(liftGammaGain));
        if (!renderingVolume.profile.TryGet(out toneMap)) throw new System.NullReferenceException(nameof(toneMap));
        if (!renderingVolume.profile.TryGet(out filmGrain)) throw new System.NullReferenceException(nameof(filmGrain));
        if (!renderingVolume.profile.TryGet(out vignette)) throw new System.NullReferenceException(nameof(vignette));
        if (!renderingVolume.profile.TryGet(out depthOfField)) throw new System.NullReferenceException(nameof(depthOfField));
        if (!renderingVolume.profile.TryGet(out chromaticAberration)) throw new System.NullReferenceException(nameof(chromaticAberration));

        // Setting the volume on the audio mixer channels
        audioMixer.SetFloat("masterVolume", pMasterVolume);
        audioMixer.SetFloat("musicVolume", pMusicVolume);
        audioMixer.SetFloat("vfxVolume", pVfxVolume);

        // Toggling post processing and fullscreen based on player prefs
        postProcessingEnabled = (pPostProcessingEnabled == 0) ? false : true;
        postProcessingToggle.isOn = (pPostProcessingEnabled == 0) ? false : true;
        Screen.fullScreen = (pIsFullscreen == 0) ? false : true;
        fullscreenToggle.isOn = (pIsFullscreen == 0) ? false : true;

        // Setting the menu sliders to correspond to the saved Player Pref values
        masterSlider.value = pMasterVolume;
        musicSlider.value = pMusicVolume;
        vfxSlider.value = pVfxVolume;
        brightnessSlider.value = pBrightness;
        distanceDropdown.value = pDistanceLevel;
        densityDropdown.value = pDensityLevel;
        qualityDropdown.value = pGraphicsPreset;

        // Setting the brightness, post processing, foliage settings and volume settings
        SetGammaAlpha(pBrightness);
        TogglePP(postProcessingEnabled);
        SetGrassDensity(pDensityLevel);
        SetGrassDistance(pDistanceLevel);
        SetVolumeMaster(pMasterVolume);
        SetVolumeMusic(pMusicVolume);
        SetVolumeVfx(pVfxVolume);
    }

    private void Update()
    {
        if(playerController.isSliding && chromaIntensity < 1f)
            chromaIntensity += 0.01f;
        else if(!playerController.isSliding && chromaIntensity > 0f)
            chromaIntensity -= 0.01f;
        chromaticAberration.intensity.value = chromaIntensity;
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

    public void SetQualityLevel(int qualityLevel)
    {
        QualitySettings.SetQualityLevel(qualityLevel);
        PlayerPrefs.SetInt("playerGraphicsPreset", qualityLevel);
    }

    public void SetGrassDensity(int densityLevel) 
    {
        grassDensity = densityLevel switch
        {
            0 => 0.15f,
            1 => 0.3f,
            2 => 0.5f,
            _ => 0.3f
        };
        Terrain.activeTerrain.detailObjectDensity = grassDensity;
        PlayerPrefs.SetInt("playerDensityLevel", densityLevel);
    } 
    public void SetGrassDistance(int distanceLevel) 
    {
        grassDistance = distanceLevel switch
        {
            0 => 10,
            1 => 35,
            2 => 50,
            _ => 35
        };
        Terrain.activeTerrain.detailObjectDistance = grassDistance;
        PlayerPrefs.SetInt("playerDistanceLevel", distanceLevel);
    }
    public void SetFullscreen(bool isFullscreen) 
    {
        Screen.fullScreen = isFullscreen;
        int fullscreenInt = (isFullscreen) ? 1 : 0;
        PlayerPrefs.SetInt("playerFullscreen", fullscreenInt);
    }
}
