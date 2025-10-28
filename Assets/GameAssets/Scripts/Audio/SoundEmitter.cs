using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class SoundEmitter : MonoBehaviour
{
    private AudioSource source;

    public void Play(AudioClip clip, float volume = 1f)
    {
        source = GetComponent<AudioSource>();
        source.spatialBlend = 1f; // 3D звук
        source.clip = clip;
        source.volume = volume;
        source.Play();

        Destroy(gameObject, clip.length + 0.2f);
    }
}
