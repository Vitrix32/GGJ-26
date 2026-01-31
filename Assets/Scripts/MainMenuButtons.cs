using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuButtons : MonoBehaviour
{
    [SerializeField]
    private GameObject settingsPanel;

    [SerializeField]
    private GameObject creditsPanel;

    public void StartButton()
    {
        SceneManager.LoadScene("GameScene");
    }
    public void SettingsButton()
    {
        settingsPanel.SetActive(true);
    }
    public void CreditsButton()
    {
        creditsPanel.SetActive(true);
    }
    public void QuitButton()
    {
        Application.Quit();

        #if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
        #endif
    }
    public void CloseSettings()
    {
        settingsPanel.SetActive(false);
    }
    public void CloseCredits()
    {
        creditsPanel.SetActive(false);
    }
    public void OnMasterVolumeChanged(float val)
    {
        GlobalSettings.Instance.MasterVolume = val;
    }
    public void OnMusicVolumeChanged(float val)
    {
        GlobalSettings.Instance.MusicVolume = val;
    }
}
