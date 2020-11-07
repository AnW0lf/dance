using UnityEngine;
using System.Collections;

public class MusicPlayer : MonoBehaviour
{
    [SerializeField] private AudioSource _audioSource = null;
    [SerializeField] private AudioClip[] _tracks;

    private AudioClip _currentTrack;
    public bool Active { get; private set; } = false;

    public void Play()
    {
        _currentTrack = _tracks[(int)Random.Range(0f, _tracks.Length)];
        _audioSource.clip = _currentTrack;

        Active = true;
        _audioSource.Play();
    }

    public void Stop()
    {
        Active = false;
        _audioSource.Stop();
    }
}
