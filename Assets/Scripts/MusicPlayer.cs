using UnityEngine;
using System.Collections;

public class MusicPlayer : MonoBehaviour
{
    [SerializeField] private AudioSource _audioSource = null;
    [SerializeField] private AudioClip[] _tracks;

    private AudioClip _currentTrack;
    private Coroutine _fadeSound = null;
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

    public void FadeSound(float from, float to)
    {
        if (IsFading) StopCoroutine(_fadeSound);
        _fadeSound = StartCoroutine(FadeSound(from, to, 1f));
    }

    private IEnumerator FadeSound(float from, float to, float duration)
    {
        float timer = 0f;
        while(timer <= duration)
        {
            timer += Time.deltaTime;
            _audioSource.volume = Mathf.Lerp(from, to, timer / duration);
            yield return null;
        }
        _fadeSound = null;
    }

    public bool IsFading => _fadeSound != null;
}
