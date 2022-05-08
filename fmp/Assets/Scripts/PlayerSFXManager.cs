using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class PlayerSFXManager : MonoBehaviour
{
    public AudioClip[] playerDamagedSounds, playerAttackSounds, newRoundSounds;
    public AudioClip doorOpenSound, doorDenySound;
    public AudioSource vfxSource;

    public static bool damageSFX, attackSFX, newRoundSFX, doorInteractSFX, doorOpenSFX;
    public static int attackIndexSFX;

    void Update()
    {
        if(damageSFX)
            PlayDamageSounds(damageSFX);
        if(attackSFX)
            PlayAttackSounds(attackSFX, attackIndexSFX);
        if(newRoundSFX)
            PlayNewRoundSound(newRoundSFX);
        if(doorInteractSFX)
            PlayDoorSound(doorOpenSFX);
    }

    private void PlayDamageSounds(bool damaged)
    {
        int i = playerDamagedSounds.Length;
        vfxSource.PlayOneShot(playerDamagedSounds[Random.Range(0,i)]);
        damageSFX = false;
    }

    private void PlayAttackSounds(bool attacking, int index)
    {
        vfxSource.PlayOneShot(playerAttackSounds[index]);
    }

    private void PlayNewRoundSound(bool newRound)
    {

    }

    private void PlayDoorSound(bool openable)
    {

    }
}
