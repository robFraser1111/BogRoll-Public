using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Menu : MonoBehaviour
{
    [SerializeField] private ScoreTracker _scoreTracker;

    [SerializeField] private Slider volumeSlider;

    [SerializeField] private Toggle fullscreenToggle;

    [SerializeField] private Dropdown resolutionsDropdown;

    [SerializeField] private Dropdown qualityDropdown;

    private AudioMixer audioMixer;

    private Resolution[] resolutions;

    private void Start()
    {
        SetSavedResolutions();

        SetSavedVolume();

        SetSavedFullscreen();

        SetSavedQuality();
    }

    private void OnApplicationQuit()
    {
        SavePrefs();
    }

    private void SetSavedResolutions()
    {
        resolutions = Screen.resolutions;

        resolutionsDropdown.ClearOptions();

        var options = new List<string>();

        var currentResolutionIndex = 0;

        for (var i = 0; i < resolutions.Length; i++)
        {
            var option = resolutions[i].width + " x " + resolutions[i].height;
            options.Add(option);

            if (resolutions[i].width == Screen.currentResolution.width &&
                resolutions[i].height == Screen.currentResolution.height) currentResolutionIndex = i;
        }

        resolutionsDropdown.AddOptions(options);
        var savedResolution = PlayerPrefs.GetInt("resolution", currentResolutionIndex);
        resolutionsDropdown.value = savedResolution;
        Screen.SetResolution(resolutions[savedResolution].width, resolutions[savedResolution].height,
            Screen.fullScreen);
        resolutionsDropdown.RefreshShownValue();
    }

    private void SetSavedVolume()
    {
        var savedVolume = PlayerPrefs.GetFloat("volume", 0f);
        audioMixer = Resources.Load<AudioMixer>("MainAudioMixer");
        audioMixer.SetFloat("masterVolume", savedVolume);
        volumeSlider.value = savedVolume;
    }

    private void SetSavedFullscreen()
    {
        var savedFullscreen = PlayerPrefs.GetString("fullscreen", "true");

        Screen.fullScreen = savedFullscreen == "true";
        fullscreenToggle.isOn = savedFullscreen == "true";
    }

    private void SetSavedQuality()
    {
        var savedQuality = PlayerPrefs.GetInt("quality", 0);
        qualityDropdown.value = savedQuality;
    }

    private void SavePrefs()
    {
        PlayerPrefs.Save();
    }

    public void NewGame()
    {
        SaveSystem.DeleteSaveFiles();
        _scoreTracker.ResetScores();
        SceneManager.LoadScene("Map");
    }

    public void ContinueGame()
    {
        _scoreTracker.InitialiseRolls();
        _scoreTracker.InitialiseShungites();
        SceneManager.LoadScene("Map");
    }

    public void QuitGame()
    {
        SavePrefs();
        SaveSystem.SaveRolls(_scoreTracker.Scores);
        SaveSystem.SaveShungites(_scoreTracker.ShungiteScores);
        Application.Quit();
    }
}