using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;
public class PlayerVFXManager : MonoBehaviour
{
    public GameObject player;
    public VisualEffect[] vfx;
    private int vfxIndex = 0;
    private float waitDuration;
    public static bool isPlaying = false;

    private void Start()
    {
        for (int i = 0; i < vfx.Length; i++)
            vfx[i].Stop();
    }

    public void StartAttackVFX(float duration) 
    {
        waitDuration = duration;
        StartCoroutine(AttackVFX());
    } 

    IEnumerator AttackVFX() 
    {
        isPlaying = true;

        if (playerController.attackIndex > 0)
            vfxIndex = playerController.attackIndex - 1;

        vfx[vfxIndex].transform.position = player.transform.position;
        vfx[vfxIndex].transform.rotation = player.transform.rotation;
        vfx[vfxIndex].Reinit();
        vfx[vfxIndex].Play();
        print(waitDuration);
        yield return new WaitForSeconds(waitDuration + 0.2f);
        print("stop");
        vfx[vfxIndex].Stop();
        isPlaying = false;
    }
}
