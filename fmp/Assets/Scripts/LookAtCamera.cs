using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
