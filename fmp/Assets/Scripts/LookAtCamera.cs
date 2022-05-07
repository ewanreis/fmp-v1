using System.Collections;
using System.Collections.Generic;
using UnityEngine;
// Used for the ingame flame sprites on the candles to face the camera
public class LookAtCamera : MonoBehaviour
{
    public Camera cam;
    public Transform head;
    void Start() => cam = Camera.main;
    void LateUpdate()
     {
        transform.LookAt(cam.transform);
        transform.rotation = Quaternion.LookRotation(cam.transform.forward);
        transform.position = head.transform.position;
     }
}
