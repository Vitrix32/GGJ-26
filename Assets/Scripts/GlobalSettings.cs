using System;
using UnityEngine;

public class GlobalSettings : MonoBehaviour
{
    public static GlobalSettings Instance {get; private set;}

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public Action MasterVolumeModified;
    private float masterVolume = 0.5f;
    public float MasterVolume
    {
        get
        {
            return masterVolume;
        }
        set
        {
            if (masterVolume == value) return;
            masterVolume = value;
            MasterVolumeModified.Invoke(); 
        }
    }
    public Action MusicVolumeModified;
    private float musicVolume = 0.5f;
    public float MusicVolume
    {
        get
        {
            return musicVolume;
        }
        set
        {
            if (musicVolume == value) return;
            musicVolume = value;
            MusicVolumeModified.Invoke();
        }
    }

}
