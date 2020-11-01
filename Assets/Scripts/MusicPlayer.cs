using UnityEngine;
using System.Collections;

public class MusicPlayer : MonoBehaviour
{
    [SerializeField] private AudioSource _audioSource = null;

    public bool Active { get; private set; } = false;

    public void Play()
    {
        Active = true;
        _audioSource.Play();
    }

    public void Stop()
    {
        Active = false;
        _audioSource.Stop();
    }
}
