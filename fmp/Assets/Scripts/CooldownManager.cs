using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class CooldownManager : MonoBehaviour
{
    public Sprite[] spellIcons;
    public Image[] iconSlots;
    private int[] cooldownStage = new int[3];
    private int pClass, slotClassOffset, slotSpellOffset;
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

        for(int i = 0; i < 3; i++)
        {

            iconSlots[i].sprite = spellIcons[slotClassOffset + (i * 3) + cooldownStage[i]];
        }
    }
}
