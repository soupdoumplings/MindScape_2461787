using System;
using StarterAssets;
using UnityEngine;

/// <summary>Stores the player's basic options on this device.</summary>
public sealed class GameSettings : MonoBehaviour
{
    private const string VolumeKey = "settings.volume";
    private const string SensitivityKey = "settings.sensitivity";
    private const string FrameRateKey = "settings.frameRate";

    public static GameSettings Instance { get; private set; }
    public float Volume { get; private set; }
    public float Sensitivity { get; private set; }
    public int FrameRate { get; private set; }
    public event Action Changed;

    private float baseRotationSpeed = -1f;

    private void Awake()
    {
        Instance = this;
        Volume = PlayerPrefs.GetFloat(VolumeKey, 1f);
        Sensitivity = PlayerPrefs.GetFloat(SensitivityKey, 1f);
        FrameRate = PlayerPrefs.GetInt(FrameRateKey, 60);
        Apply();
    }

    private void Update()
    {
        FirstPersonController controller = FindFirstObjectByType<FirstPersonController>();
        if (controller != null)
        {
            if (baseRotationSpeed < 0f) baseRotationSpeed = controller.RotationSpeed;
            controller.RotationSpeed = baseRotationSpeed * Sensitivity;
        }
    }

    public void SetVolume(float value) { Volume = Mathf.Clamp01(value); SaveAndApply(); }
    public void SetSensitivity(float value) { Sensitivity = Mathf.Clamp(value, 0.5f, 2f); SaveAndApply(); }
    public void SetFrameRate(int value) { FrameRate = value; SaveAndApply(); }

    private void SaveAndApply()
    {
        PlayerPrefs.SetFloat(VolumeKey, Volume);
        PlayerPrefs.SetFloat(SensitivityKey, Sensitivity);
        PlayerPrefs.SetInt(FrameRateKey, FrameRate);
        PlayerPrefs.Save();
        Apply();
        Changed?.Invoke();
    }

    private void Apply()
    {
        AudioListener.volume = Volume;
        Application.targetFrameRate = FrameRate;
    }
}
