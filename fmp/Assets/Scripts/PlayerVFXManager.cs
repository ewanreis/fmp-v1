using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;
public class PlayerVFXManager : MonoBehaviour
{
    public GameObject player;
    public VisualEffect[] vfx;
    private int vfxIndex;
    private bool isPlaying = false;

    private void Update()
    {
        if(PlayerAttackSystem.attackState == true && isPlaying == false)
            StartCoroutine(AttackVFX());
    }
    IEnumerator AttackVFX()
    {
        isPlaying = true;
        int vfxIndex = playerController.attackIndex - 1;
        float duration = PlayerAttackSystem.attackDuration;
        vfx[vfxIndex].transform.position = player.transform.position;
        vfx[vfxIndex].transform.rotation = player.transform.rotation;
        vfx[vfxIndex].Reinit();
        vfx[vfxIndex].Play();
        yield return new WaitForSeconds(duration);
        vfx[vfxIndex].Stop();
        isPlaying = false;
        yield return null;
    }
}
