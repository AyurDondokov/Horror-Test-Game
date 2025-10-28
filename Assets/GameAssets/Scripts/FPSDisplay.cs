using UnityEngine;
using TMPro;

public class FPSDisplay : MonoBehaviour
{
    [Header("Настройки отображения")]
    [SerializeField] private TextMeshProUGUI fpsText;
    [SerializeField] private float updateInterval = 0.3f;

    private float timer;
    private int frames;
    private float fps;


    private void Update()
    {
        frames++;
        timer += Time.unscaledDeltaTime;

        if (timer >= updateInterval)
        {
            fps = frames / timer;
            frames = 0;
            timer = 0f;

            UpdateDisplay();
        }
    }

    private void UpdateDisplay()
    {
        if (fpsText == null) return;
        fpsText.text = $"{fps:F0}";
    }
}
