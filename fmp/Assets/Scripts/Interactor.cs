using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using TMPro;

public class Interactor : MonoBehaviour
{
    public LayerMask doorLayer, altarLayer, extraHealthLayer, extraDamageLayer;
    public Camera cam;
    public TMP_Text interactText;

    private int cost;
    private AltarType altarType;

    UnityEvent onInteract;

    void Update()
    {
        RaycastHit hit;

        if(Physics.Raycast(cam.transform.position, cam.transform.forward, out hit, 3f, doorLayer))
        {
            cost = hit.collider.GetComponent<Interactable>().cost;
            interactText.text = $"Press F to Unlock Door for ${cost}";
            onInteract = hit.collider.GetComponent<Interactable>().onInteract;

            if(Input.GetKeyDown(KeyCode.F) && playerController.playerMoney < cost)
                PlayerSFXManager.denySFX = true;

            if(Input.GetKeyDown(KeyCode.F) && playerController.playerMoney >= cost)
            {
                PlayerSFXManager.doorOpenSFX = true;
                playerController.playerMoney -= cost;
                onInteract.Invoke();
            }
            
        }
        else if(Physics.Raycast(cam.transform.position, cam.transform.forward, out hit, 5f, altarLayer))
        {
            
            onInteract = hit.collider.GetComponent<Interactable>().onInteract;
            altarType = hit.collider.GetComponent<Interactable>().altarType;
            cost = hit.collider.GetComponent<Interactable>().cost;
            interactText.text = $"Press F to Change Class to {altarType} for ${cost}";
            if(Input.GetKeyDown(KeyCode.F) && playerController.playerMoney >= cost)
            {
                playerController.playerClass = (int)altarType + 1;
                playerController.playerMoney -= cost;
                PlayerSFXManager.doorOpenSFX = true;
                onInteract.Invoke();
            }
            if(Input.GetKeyDown(KeyCode.F) && playerController.playerMoney < cost)
                PlayerSFXManager.denySFX = true;
        }
        else if(Physics.Raycast(cam.transform.position, cam.transform.forward, out hit, 5f, extraHealthLayer))
        {
            cost = hit.collider.GetComponent<Interactable>().cost;
            interactText.text = $"Press F to gain an extra 10 health for ${cost}";
            if(Input.GetKeyDown(KeyCode.F) && playerController.playerMoney >= cost)
            {
                playerController.playerMoney -= cost;
                playerController.maxHealth += 10;
                PlayerSFXManager.doorOpenSFX = true;
            }
            if(Input.GetKeyDown(KeyCode.F) && playerController.playerMoney < cost)
                PlayerSFXManager.denySFX = true;
        }
        else if(Physics.Raycast(cam.transform.position, cam.transform.forward, out hit, 5f, extraDamageLayer))
        {
            cost = hit.collider.GetComponent<Interactable>().cost;
            interactText.text = $"Press F to gain an extra 5 damage on all attacks for ${cost}";
            if(Input.GetKeyDown(KeyCode.F) && playerController.playerMoney >= cost)
            {
                playerController.playerMoney -= cost;
                PlayerAttackSystem.damageModifier += 5;
                PlayerSFXManager.doorOpenSFX = true;
            }
            if(Input.GetKeyDown(KeyCode.F) && playerController.playerMoney < cost)
                PlayerSFXManager.denySFX = true;
        }
        else
        {
            interactText.text = "";
        }
        
    }
}
