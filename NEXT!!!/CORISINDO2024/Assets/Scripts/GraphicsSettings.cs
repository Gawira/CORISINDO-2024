using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Rendering.PostProcessing;

public class GraphicsSettings : MonoBehaviour
{
    public Toggle postProcessingToggle;
    public Toggle motionBlurToggle;
    public Toggle vignetteToggle;
    public Toggle ambientOcclusionToggle;
    public Toggle colorGradingToggle;
    public Toggle bloomToggle;

    public PostProcessProfile postProcessProfile;

    private void Start()
    {
        // Load saved settings
        LoadSettings();

        // Add listeners to the toggles
        postProcessingToggle.onValueChanged.AddListener(delegate { TogglePostProcessing(postProcessingToggle.isOn); });
        motionBlurToggle.onValueChanged.AddListener(delegate { ToggleMotionBlur(motionBlurToggle.isOn); });
        vignetteToggle.onValueChanged.AddListener(delegate { ToggleVignette(vignetteToggle.isOn); });
        ambientOcclusionToggle.onValueChanged.AddListener(delegate { ToggleAmbientOcclusion(ambientOcclusionToggle.isOn); });
        colorGradingToggle.onValueChanged.AddListener(delegate { ToggleColorGrading(colorGradingToggle.isOn); });
        bloomToggle.onValueChanged.AddListener(delegate { ToggleBloom(bloomToggle.isOn); });

        // Ensure the initial state of the toggles is correct
        SetGraphicsTogglesInteractable(postProcessingToggle.isOn);
    }

    private void TogglePostProcessing(bool isOn)
    {
        SetGraphicsTogglesInteractable(isOn);

        if (isOn)
        {
            ApplySettings();
        }
        else
        {
            DisableAllPostProcessingEffects();
        }

        SaveSettings();
    }

    private void ToggleMotionBlur(bool isOn)
    {
        if (postProcessProfile.TryGetSettings(out MotionBlur motionBlur))
        {
            motionBlur.enabled.value = isOn;
        }
        SaveSettings();
    }

    private void ToggleVignette(bool isOn)
    {
        if (postProcessProfile.TryGetSettings(out Vignette vignette))
        {
            vignette.enabled.value = isOn;
        }
        SaveSettings();
    }

    private void ToggleAmbientOcclusion(bool isOn)
    {
        if (postProcessProfile.TryGetSettings(out AmbientOcclusion ambientOcclusion))
        {
            ambientOcclusion.enabled.value = isOn;
        }
        SaveSettings();
    }

    private void ToggleColorGrading(bool isOn)
    {
        if (postProcessProfile.TryGetSettings(out ColorGrading colorGrading))
        {
            colorGrading.enabled.value = isOn;
        }
        SaveSettings();
    }

    private void ToggleBloom(bool isOn)
    {
        if (postProcessProfile.TryGetSettings(out Bloom bloom))
        {
            bloom.enabled.value = isOn;
        }
        SaveSettings();
    }

    private void SaveSettings()
    {
        PlayerPrefs.SetInt("PostProcessing", postProcessingToggle.isOn ? 1 : 0);
        PlayerPrefs.SetInt("MotionBlur", motionBlurToggle.isOn ? 1 : 0);
        PlayerPrefs.SetInt("Vignette", vignetteToggle.isOn ? 1 : 0);
        PlayerPrefs.SetInt("AmbientOcclusion", ambientOcclusionToggle.isOn ? 1 : 0);
        PlayerPrefs.SetInt("ColorGrading", colorGradingToggle.isOn ? 1 : 0);
        PlayerPrefs.SetInt("Bloom", bloomToggle.isOn ? 1 : 0);
        PlayerPrefs.Save(); // Ensure PlayerPrefs are saved
    }

    private void LoadSettings()
    {
        bool postProcessingEnabled = PlayerPrefs.GetInt("PostProcessing", 1) == 1;
        postProcessingToggle.isOn = postProcessingEnabled;

        motionBlurToggle.isOn = PlayerPrefs.GetInt("MotionBlur", 1) == 1 && postProcessingEnabled;
        vignetteToggle.isOn = PlayerPrefs.GetInt("Vignette", 1) == 1 && postProcessingEnabled;
        ambientOcclusionToggle.isOn = PlayerPrefs.GetInt("AmbientOcclusion", 1) == 1 && postProcessingEnabled;
        colorGradingToggle.isOn = PlayerPrefs.GetInt("ColorGrading", 1) == 1 && postProcessingEnabled;
        bloomToggle.isOn = PlayerPrefs.GetInt("Bloom", 1) == 1 && postProcessingEnabled;

        ApplySettings();
    }

    private void SetGraphicsTogglesInteractable(bool isInteractable)
    {
        motionBlurToggle.interactable = isInteractable;
        vignetteToggle.interactable = isInteractable;
        ambientOcclusionToggle.interactable = isInteractable;
        colorGradingToggle.interactable = isInteractable;
        bloomToggle.interactable = isInteractable;
    }

    private void ApplySettings()
    {
        if (postProcessingToggle.isOn)
        {
            ToggleMotionBlur(motionBlurToggle.isOn);
            ToggleVignette(vignetteToggle.isOn);
            ToggleAmbientOcclusion(ambientOcclusionToggle.isOn);
            ToggleColorGrading(colorGradingToggle.isOn);
            ToggleBloom(bloomToggle.isOn);
        }
        else
        {
            DisableAllPostProcessingEffects();
        }
    }

    private void DisableAllPostProcessingEffects()
    {
        if (postProcessProfile.TryGetSettings(out MotionBlur motionBlur))
        {
            motionBlur.enabled.value = false;
        }

        if (postProcessProfile.TryGetSettings(out Vignette vignette))
        {
            vignette.enabled.value = false;
        }

        if (postProcessProfile.TryGetSettings(out AmbientOcclusion ambientOcclusion))
        {
            ambientOcclusion.enabled.value = false;
        }

        if (postProcessProfile.TryGetSettings(out ColorGrading colorGrading))
        {
            colorGrading.enabled.value = false;
        }

        if (postProcessProfile.TryGetSettings(out Bloom bloom))
        {
            bloom.enabled.value = false;
        }
    }
}
