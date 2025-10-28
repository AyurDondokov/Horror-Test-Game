using UnityEngine;

public class SoundPlayer : MonoBehaviour
{
    [SerializeField] private AudioClip sound;
    [SerializeField] private float volume;

    public void Play()
    {
        AudioManager.Instance.PlaySoundAt(sound, transform.position, volume);
    }
}
