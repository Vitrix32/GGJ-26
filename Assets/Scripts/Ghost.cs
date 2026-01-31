using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ghost : MonoBehaviour
{
    public enum GhostName
    {
        Eugene,
        Claire,
        Mark,
        Amanda,
        Tilda,
        Ed
    }

    public GhostName Name {get; set;}
}
