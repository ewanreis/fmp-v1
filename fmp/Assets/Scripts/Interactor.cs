using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using TMPro;

public class Interactor : MonoBehaviour
{
    public LayerMask doorLayer, altarLayer;
    public Camera cam;
    public TMP_Text interactText;

    private int doorCost, upgradeCost;
    private AltarType altarType;

    UnityEvent onInteract;

    void Update()
    {
        RaycastHit hit;

        if(Physics.Raycast(cam.transform.position, cam.transform.forward, out hit, 3f, doorLayer))
        {
            doorCost = hit.collider.GetComponent<Interactable>().cost;
            interactText.text = $"Press F to Unlock Door for ${doorCost}";
            onInteract = hit.collider.GetComponent<Interactable>().onInteract;

            if(Input.GetKeyDown(KeyCode.F) && playerController.playerMoney < doorCost)
                PlayerSFXManager.denySFX = true;

            if(Input.GetKeyDown(KeyCode.F) && playerController.playerMoney >= doorCost)
            {
                PlayerSFXManager.doorOpenSFX = true;
                playerController.playerMoney -= doorCost;
                onInteract.Invoke();
            }
            
        }
        else if(Physics.Raycast(cam.transform.position, cam.transform.forward, out hit, 5f, altarLayer))
        {
            
            onInteract = hit.collider.GetComponent<Interactable>().onInteract;
            altarType = hit.collider.GetComponent<Interactable>().altarType;
            upgradeCost = hit.collider.GetComponent<Interactable>().cost;
            interactText.text = $"Press F to Change Class to {altarType} for ${upgradeCost}";
            if(Input.GetKeyDown(KeyCode.F) && playerController.playerMoney >= upgradeCost)
            {
                playerController.playerClass = (int)altarType + 1;
                PlayerSFXManager.doorOpenSFX = true;
                onInteract.Invoke();
            }
            if(Input.GetKeyDown(KeyCode.F) && playerController.playerMoney < upgradeCost)
                PlayerSFXManager.denySFX = true;
        }
        else
        {
            interactText.text = "";
        }
        
    }
}
