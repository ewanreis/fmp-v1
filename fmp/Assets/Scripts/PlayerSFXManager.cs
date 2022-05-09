using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class PlayerSFXManager : MonoBehaviour
{
    public AudioClip[] playerDamagedSounds, playerAttackSounds, newRoundSounds;
    public AudioClip doorOpenSound, doorDenySound;
    public AudioSource vfxSource;

    public static bool damageSFX, attackSFX, newRoundSFX, doorOpenSFX, doorDenySFX;
    public static int attackIndexSFX;

    void Update()
    {
        if(damageSFX)
            PlayDamageSounds(damageSFX);
        if(attackSFX)
            PlayAttackSounds(attackSFX, attackIndexSFX);
        if(newRoundSFX)
            PlayNewRoundSound(newRoundSFX);
        if(doorOpenSFX)
            PlayDoorOpen();
        if(doorDenySFX)
            PlayDoorDeny();
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
        attackSFX = false;
    }

    private void PlayNewRoundSound(bool newRound)
    {
        vfxSource.PlayOneShot(newRoundSounds[Random.Range(0,1)]);
        newRoundSFX = false;
    }

    private void PlayDoorOpen()
    {
        vfxSource.PlayOneShot(doorOpenSound);
        doorOpenSFX = false;
    }
    private void PlayDoorDeny()
    {
        vfxSource.PlayOneShot(doorDenySound);
        doorDenySFX = false;
    }
}
