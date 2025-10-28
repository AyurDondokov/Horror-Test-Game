using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance { get; private set; }

    [Header("Main Sources")]
    [SerializeField] private AudioSource ambientSource;

    [Header("Settings")]
    [SerializeField] private float ambientFadeTime = 1.5f;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
    }

    /// <summary>
    /// ������������� ����� ������� (� ������� ������).
    /// </summary>
    public void PlayAmbient(AudioClip clip)
    {
        if (ambientSource.isPlaying)
            StartCoroutine(FadeToNewAmbient(clip));
        else
        {
            ambientSource.clip = clip;
            ambientSource.Play();
        }
    }

    private System.Collections.IEnumerator FadeToNewAmbient(AudioClip newClip)
    {
        float startVol = ambientSource.volume;
        float t = 0f;

        // ������� ���������
        while (t < ambientFadeTime)
        {
            t += Time.deltaTime;
            ambientSource.volume = Mathf.Lerp(startVol, 0f, t / ambientFadeTime);
            yield return null;
        }

        ambientSource.clip = newClip;
        ambientSource.Play();

        // ������� ���������� ���������
        t = 0f;
        while (t < ambientFadeTime)
        {
            t += Time.deltaTime;
            ambientSource.volume = Mathf.Lerp(0f, startVol, t / ambientFadeTime);
            yield return null;
        }
    }

    /// <summary>
    /// ����������� ��������� ���� � ������������ (�������� ��������� ��������).
    /// </summary>
    public void PlaySoundAt(AudioClip clip, Vector3 position, float volume = 1f)
    {
        if (clip == null) return;

        GameObject soundObj = new GameObject("SoundEmitter_" + clip.name);
        soundObj.transform.position = position;

        var emitter = soundObj.AddComponent<SoundEmitter>();
        emitter.Play(clip, volume);
    }

    /// <summary>
    /// ����������� ���� ��� �������� � ������� (��������, UI ��� jumpscare).
    /// </summary>
    public void PlaySoundGlobal(AudioClip clip, float volume = 1f)
    {
        PlaySoundAt(clip, Camera.main.transform.position, volume);
    }
}
