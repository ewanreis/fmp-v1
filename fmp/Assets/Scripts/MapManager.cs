using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;
using UnityEngine.Audio;
using UnityEngine.Rendering;
using TMPro;

public class MapManager : MonoBehaviour
{
    #region Audio
    public AudioSource musicSource, vfxSource;
    public AudioMixer audioMixer;
    public AudioClip[] clips;
    #endregion

    #region Graphics
    // Post Processing
    public Volume renderingVolume;
    public LiftGammaGain liftGammaGain;
    public Tonemapping toneMap;
    public FilmGrain filmGrain;
    public Vignette vignette;
    public DepthOfField depthOfField;

    // Graphics Quality
    //public RenderPipelineAsset[] qualityLevels;
    public TMP_Dropdown qualityDropdown;

    // Nature
    public Terrain terrain;
    public float grassDistance, grassDensity;
    #endregion

    #region Player Preferences
    private int pIsFullscreen, pGraphicsPreset, pPostProcessingEnabled;
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
        if (!renderingVolume.profile.TryGet(out liftGammaGain)) throw new System.NullReferenceException(nameof(liftGammaGain));
        if (!renderingVolume.profile.TryGet(out toneMap)) throw new System.NullReferenceException(nameof(toneMap));
        if (!renderingVolume.profile.TryGet(out filmGrain)) throw new System.NullReferenceException(nameof(filmGrain));
        if (!renderingVolume.profile.TryGet(out vignette)) throw new System.NullReferenceException(nameof(vignette));
        if (!renderingVolume.profile.TryGet(out depthOfField)) throw new System.NullReferenceException(nameof(depthOfField));

        audioMixer.SetFloat("masterVolume", pMasterVolume);
        audioMixer.SetFloat("musicVolume", pMusicVolume);
        audioMixer.SetFloat("vfxVolume", pVfxVolume);

        postProcessingEnabled = (pPostProcessingEnabled == 0) ? false : true;
        Screen.fullScreen = (pIsFullscreen == 0) ? false : true;

        SetGammaAlpha(pBrightness);
        TogglePP(postProcessingEnabled);

    }
    /*void Update() 
    { 
        SetGammaAlpha(pBrightness);
        TogglePP(postProcessingEnabled);
        SetGrassDensity(grassDensity);
        SetGrassDistance(grassDistance);
    }*/

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
        //QualitySettings.renderPipeline = qualityLevels[qualityLevel];
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
    }
}
