using UnityEngine;
using UnityEngine.Audio;

public class SettingsMenu : MonoBehaviour
{
    public AudioMixer audioMixer;

    public void SetVolume(float volume)
    {
        audioMixer.SetFloat("masterVolume", volume);

        PlayerPrefs.SetFloat("volume", volume);
    }

    public void SetFullscreen(bool isFullscreen)
    {
        Screen.fullScreen = isFullscreen;

        var fullscreen = isFullscreen ? "true" : "false";
        PlayerPrefs.SetString("fullscreen", fullscreen);
    }

    public void SetResolution(int resolutionIndex)
    {
        var resolution = Screen.resolutions[resolutionIndex];
        Screen.SetResolution(resolution.width, resolution.height, Screen.fullScreen);

        PlayerPrefs.SetInt("resolution", resolutionIndex);
    }

    public void SetGraphics(int qualityIndex)
    {
        PlayerPrefs.SetInt("quality", qualityIndex);

        if (qualityIndex > 4) FindObjectOfType<Steam>().Psycho();
    }
}