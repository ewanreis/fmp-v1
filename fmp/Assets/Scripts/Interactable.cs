using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public enum AltarType
{
    Void,
    Sun,
    Chaos,
    None
}
public class Interactable: MonoBehaviour 
{
    public UnityEvent onInteract;
    public int cost;
    public AltarType altarType;
}