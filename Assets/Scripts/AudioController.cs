using System;
using UnityEngine;

public class AudioController : MonoBehaviour
{
    private enum AudioType
    {
        Music,
        Other
    }

    [SerializeField]
    private AudioType audioType;

    private void Start()
    {
        GlobalSettings.Instance.MasterVolumeModified += SetVolume;
        GlobalSettings.Instance.MusicVolumeModified += SetVolume; 
    }

    private void SetVolume()
    {
        AudioSource audioSource = GetComponent<AudioSource>();
        if (audioType == AudioType.Music)
        {
            audioSource.volume = GlobalSettings.Instance.MasterVolume * GlobalSettings.Instance.MusicVolume;
        }
        else
        {
            audioSource.volume = GlobalSettings.Instance.MasterVolume;
        }
    }
}
