using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class SoundSettings : MonoBehaviour
{
    [SerializeField] private AudioMixer mixer;

    [Header("UI")]
    [SerializeField] private Slider masterSlider;
    [SerializeField] private Slider sfxSlider;
    [SerializeField] private Slider carSlider;
    [SerializeField] private Slider musicSlider;

    const string MASTER = "MasterVolume";
    const string SFX = "SFXVolume";
    const string CAR = "CarVolume";
    const string MUSIC = "MusicVolume";

    bool initialized;

    void Start()
    {
        initialized = false;

        masterSlider.SetValueWithoutNotify(
            PlayerPrefs.GetFloat(MASTER, 1f));

        sfxSlider.SetValueWithoutNotify(
            PlayerPrefs.GetFloat(SFX, 1f));

        carSlider.SetValueWithoutNotify(
            PlayerPrefs.GetFloat(CAR, 1f));

        musicSlider.SetValueWithoutNotify(
            PlayerPrefs.GetFloat(MUSIC, 1f));

        SetMaster(masterSlider.value);
        SetSFX(sfxSlider.value);
        SetCar(carSlider.value);
        SetMusic(musicSlider.value);

        initialized = true;
    }

    public void SetMaster(float value)
    {
        if (!initialized)
            return;

        SetVolume(MASTER, value);
    }

    public void SetSFX(float value)
    {
        if (!initialized)
            return;

        SetVolume(SFX, value);
    }

    public void SetCar(float value)
    {
        if (!initialized)
            return;

        SetVolume(CAR, value);
    }

    public void SetMusic(float value)
    {
        if (!initialized)
            return;

        SetVolume(MUSIC, value);
    }

    void ApplyVolume(string key, float value)
    {
        float dB =
            value <= 0.0001f
                ? -80f
                : Mathf.Log10(value) * 20f;

        mixer.SetFloat(key, dB);
    }

    void SetVolume(string key, float value)
    {
        ApplyVolume(key, value);

        PlayerPrefs.SetFloat(key, value);
        PlayerPrefs.Save();
    }
}