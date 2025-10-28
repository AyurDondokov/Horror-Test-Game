using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class PostFXController : MonoBehaviour
{
    [Header("Volume Reference")]
    [SerializeField] private Volume globalVolume;

    private Vignette vignette;
    private ChromaticAberration chromatic;
    private Bloom bloom;
    private ColorAdjustments colorAdjust;

    private void Awake()
    {
        if (globalVolume == null)
            globalVolume = FindObjectOfType<Volume>();

        // Получаем эффекты из VolumeProfile (если есть)
        if (globalVolume != null && globalVolume.profile != null)
        {
            globalVolume.profile.TryGet(out vignette);
            globalVolume.profile.TryGet(out chromatic);
            globalVolume.profile.TryGet(out bloom);
            globalVolume.profile.TryGet(out colorAdjust);
        }
    }

    // ========== VIGNETTE ==========
    public void SetVignetteIntensity(float value)
    {
        if (vignette != null)
            FadeVignette(vignette.intensity.value, value, 1f);
    }
    public void SetVignetteColor(Color color)
    {
        vignette.color.value = color;
    }
    public void SetVignetteColor(string color)
    {
        if (color.ToLower() == "red")
            vignette.color.value = Color.red;
        else
            vignette.color.value = Color.black;
    }
    public void FadeVignette(float from, float to, float duration)
    {
        StartCoroutine(FadeRoutine(vignette, from, to, duration));
    }

    private System.Collections.IEnumerator FadeRoutine(Vignette v, float from, float to, float duration)
    {
        if (v == null) yield break;
        float t = 0f;
        v.intensity.value = from;

        while (t < duration)
        {
            t += Time.deltaTime;
            v.intensity.value = Mathf.Lerp(from, to, t / duration);
            yield return null;
        }

        v.intensity.value = to;
    }

    // ========== CHROMATIC ABERRATION ==========
    public void SetChromatic(float value)
    {
        if (chromatic != null)
            chromatic.intensity.value = Mathf.Clamp01(value);
    }

    // ========== BLOOM ==========
    public void SetBloom(float intensity)
    {
        if (bloom != null)
            bloom.intensity.value = intensity;
    }

    // ========== COLOR ADJUSTMENTS ==========
    public void SetExposure(float value)
    {
        if (colorAdjust != null)
            colorAdjust.postExposure.value = value;
    }

    public void SetSaturation(float value)
    {
        if (colorAdjust != null)
            colorAdjust.saturation.value = value;
    }
}
