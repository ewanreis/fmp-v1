using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class PlayerSFXManager : MonoBehaviour
{
    public AudioClip[] playerDamagedSounds, playerAttackSounds, newRoundSounds;
    public AudioClip doorOpenSound, denySound, loseSound;
    public AudioSource vfxSource;

    public static bool damageSFX, attackSFX, newRoundSFX, doorOpenSFX, denySFX, deadSFX;
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
        if(denySFX)
            PlayDeny();
        if(deadSFX)
            PlayLose();
    }

    private void PlayLose()
    {
        vfxSource.PlayOneShot(loseSound);
        deadSFX = false;
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
        vfxSource.PlayOneShot(newRoundSounds[Random.Range(0,2)]);
        newRoundSFX = false;
    }

    private void PlayDoorOpen()
    {
        vfxSource.PlayOneShot(doorOpenSound);
        doorOpenSFX = false;
    }
    private void PlayDeny()
    {
        vfxSource.PlayOneShot(denySound);
        denySFX = false;
    }
}
