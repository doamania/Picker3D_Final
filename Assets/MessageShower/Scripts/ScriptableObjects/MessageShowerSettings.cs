using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "MessageShower Settings", menuName = "MessageShower/Create New Settings", order = 1)]
public class MessageShowerSettings : ScriptableObject
{
    public float upRange;
    public float duration;
}
