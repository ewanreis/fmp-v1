using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class MusicManager : MonoBehaviour
{
    public AudioSource musicSource;
    public AudioClip[] musicClips;
    private void Start() => StartCoroutine(PlayMusic());
    private void Update()
    {
        if(PauseMenu.isPaused)
            musicSource.Pause();
        if(!PauseMenu.isPaused)
            musicSource.UnPause();
    }
    IEnumerator PlayMusic()
    {
        musicSource.PlayOneShot(musicClips[0]);
        yield return new WaitForSeconds(55);
        musicSource.Stop();
        musicSource.PlayOneShot(musicClips[1]);
        yield return new WaitForSeconds(125);
        musicSource.Stop();
        musicSource.PlayOneShot(musicClips[2]);
        yield return new WaitForSeconds(183);
        musicSource.Stop();
        musicSource.PlayOneShot(musicClips[3]);
        yield return new WaitForSeconds(82);
        musicSource.Stop();
        StartCoroutine(PlayMusic());
        yield return null;
    }
}
