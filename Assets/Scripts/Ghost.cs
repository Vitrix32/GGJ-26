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

    [SerializeField]
    private GhostName g_name;
    public GhostName Name
    {
        get
        {
            return g_name;
        }
        set
        {
            if (g_name == value) return;
            g_name = value;
        }
    }
}
