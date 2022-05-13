using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class CooldownManager : MonoBehaviour
{
    public Sprite[] spellIcons;
    public Image[] iconSlots;
    public Slider[] cooldownSliders;

    private float[] cooldownPercent = new float[3];
    private int[] cooldownStage = new int[3];

    private int pClass, slotClassOffset, slotSpellOffset, attackOffset;

    private void Update()
    {
        pClass = playerController.playerClass;
        slotClassOffset = pClass switch
        {
            1 => 0,
            2 => 9,
            3 => 18,
            _ => 0
        };
        attackOffset = pClass switch
        {
            1 => 0,
            2 => 3,
            3 => 6,
            _ => 0
        };

        for(int i = 0; i < 3; i++)
        {
            cooldownPercent[i] = ((PlayerAttackSystem.attackCooldown[attackOffset + i]) / (PlayerAttackSystem.maxCooldown[attackOffset + i])) * 100;

            if(cooldownPercent[i] < 0)
                cooldownPercent[i] = 0;

            if(cooldownPercent[i] == 0)
                cooldownStage[i] = 2;

            else if(cooldownPercent[i] > 0 && cooldownPercent[i] < 50)
                cooldownStage[i] = 1;

            else if(cooldownPercent[i] > 50)
                cooldownStage[i] = 0;

            iconSlots[i].sprite = spellIcons[slotClassOffset + (i * 3) + cooldownStage[i]];
            cooldownSliders[i].value = cooldownPercent[i];
        }
    }
}
