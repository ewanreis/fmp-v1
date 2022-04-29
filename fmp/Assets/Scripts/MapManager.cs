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
    #region Audio
    [Header("Audio")]
    public AudioMixer audioMixer;
    public AudioSource musicSource, vfxSource;
    public Slider masterSlider, musicSlider, vfxSlider;
    public AudioClip[] clips;
    #endregion

    #region Graphics
    [Header("Post Processing")]
    public Volume renderingVolume;
    public LiftGammaGain liftGammaGain;
    public Tonemapping toneMap;
    public FilmGrain filmGrain;
    public Vignette vignette;
    public DepthOfField depthOfField;

    [Header("Graphics Quality")]
    public Toggle fullscreenToggle, postProcessingToggle;
    public Slider brightnessSlider;
    public TMP_Dropdown distanceDropdown, densityDropdown, qualityDropdown;

    // Nature
    [Header("Nature")]
    public Terrain terrain;
    public float grassDistance, grassDensity;
    #endregion

    #region Player Preferences
    [Header("Player Preferences")]
    private int pIsFullscreen, pGraphicsPreset, pPostProcessingEnabled, pDensityLevel, pDistanceLevel;
    public float pMasterVolume, pMusicVolume, pVfxVolume, pBrightness;
    public bool postProcessingEnabled;
    #endregion

    enum SFX
    {

    }

    enum Soundtracks
    {

    }

    void Start()
    {
        pPostProcessingEnabled = PlayerPrefs.GetInt("playerPostProcessingEnabled", 1);
        pGraphicsPreset = PlayerPrefs.GetInt("playerGraphicsPreset", 1);
        pIsFullscreen = PlayerPrefs.GetInt("playerFullscreen", 0);
        pBrightness = PlayerPrefs.GetFloat("playerBrightness", 0.75f);
        pMasterVolume = PlayerPrefs.GetFloat("playerMasterVolume", 0);
        pMusicVolume = PlayerPrefs.GetFloat("playerMusicVolume", 0);
        pVfxVolume = PlayerPrefs.GetFloat("playerVfxVolume", 0);
        pDensityLevel = PlayerPrefs.GetInt("playerDensityLevel", 1);
        pDistanceLevel = PlayerPrefs.GetInt("playerDistanceLevel", 1);

        if (!renderingVolume.profile.TryGet(out liftGammaGain)) throw new System.NullReferenceException(nameof(liftGammaGain));
        if (!renderingVolume.profile.TryGet(out toneMap)) throw new System.NullReferenceException(nameof(toneMap));
        if (!renderingVolume.profile.TryGet(out filmGrain)) throw new System.NullReferenceException(nameof(filmGrain));
        if (!renderingVolume.profile.TryGet(out vignette)) throw new System.NullReferenceException(nameof(vignette));
        if (!renderingVolume.profile.TryGet(out depthOfField)) throw new System.NullReferenceException(nameof(depthOfField));

        audioMixer.SetFloat("masterVolume", pMasterVolume);
        audioMixer.SetFloat("musicVolume", pMusicVolume);
        audioMixer.SetFloat("vfxVolume", pVfxVolume);

        postProcessingEnabled = (pPostProcessingEnabled == 0) ? false : true;
        postProcessingToggle.isOn = (pPostProcessingEnabled == 0) ? false : true;
        Screen.fullScreen = (pIsFullscreen == 0) ? false : true;
        fullscreenToggle.isOn = (pIsFullscreen == 0) ? false : true;
        masterSlider.value = pMasterVolume;
        musicSlider.value = pMusicVolume;
        vfxSlider.value = pVfxVolume;
        brightnessSlider.value = pBrightness;
        distanceDropdown.value = pDistanceLevel;
        densityDropdown.value = pDensityLevel;
        qualityDropdown.value = pGraphicsPreset;

        SetGammaAlpha(pBrightness);
        TogglePP(postProcessingEnabled);
        SetGrassDensity(pDensityLevel);
        SetGrassDistance(pDistanceLevel);
        SetVolumeMaster(pMasterVolume);
        SetVolumeMusic(pMusicVolume);
        SetVolumeVfx(pVfxVolume);
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
        if(enabled)
        {
            toneMap.active = true;
            vignette.active = true;
            filmGrain.active = true;
            depthOfField.active = true;
            QualitySettings.antiAliasing = 8;
            PlayerPrefs.SetInt("playerPostProcessingEnabled", 1);
        }
        else
        {
            toneMap.active = false;
            vignette.active = false;
            filmGrain.active = false;
            depthOfField.active = false;
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
