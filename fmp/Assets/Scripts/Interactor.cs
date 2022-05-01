using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using TMPro;

public class Interactor : MonoBehaviour
{
    public LayerMask interacableLayer;
    public Camera cam;
    UnityEvent onInteract;
    public TMP_Text interactText;
    private int cost;

    void Update()
    {
        RaycastHit hit;
        if(Physics.Raycast(cam.transform.position, cam.transform.forward, out hit, 3f, interacableLayer))
        {
            cost = hit.collider.GetComponent<Interactable>().doorOpenCost;
            interactText.text = $"Press E to Unlock Door for ${cost}";
            onInteract = hit.collider.GetComponent<Interactable>().onInteract;
            if(Input.GetKeyDown(KeyCode.E) && playerController.playerMoney > cost)
            {
                playerController.playerMoney -= cost;
                onInteract.Invoke();
            }
        }
        else
        {
            interactText.text = "";
        }
        
    }
}
