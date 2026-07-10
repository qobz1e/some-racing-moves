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

    bool initialized = false;

    void Start()
    {
        float master =
            PlayerPrefs.GetFloat(MASTER, 0.75f);

        float sfx =
            PlayerPrefs.GetFloat(SFX, 0.75f);

        float car =
            PlayerPrefs.GetFloat(CAR, 0.75f);

        float music =
            PlayerPrefs.GetFloat(MUSIC, 0.75f);

        masterSlider.SetValueWithoutNotify(master);
        sfxSlider.SetValueWithoutNotify(sfx);
        carSlider.SetValueWithoutNotify(car);
        musicSlider.SetValueWithoutNotify(music);

        ApplyVolume(MASTER, master);
        ApplyVolume(SFX, sfx);
        ApplyVolume(CAR, car);
        ApplyVolume(MUSIC, music);

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