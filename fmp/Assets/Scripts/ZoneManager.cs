using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ZoneManager : MonoBehaviour
{
    public int zoneNumber;
    public static int activeZone;
    void OnTriggerStay(Collider other)
    {
        if (other.attachedRigidbody)
            activeZone = zoneNumber;
    }
}
