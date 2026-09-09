using Cysharp.Threading.Tasks;
using NaughtyAttributes;
using System;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    [BoxGroup("BGM"), SerializeField] private AudioSource bgmAudioSource;
    [BoxGroup("SE"), SerializeField] private AudioSource seAudioSource;
    [BoxGroup("SE"), SerializeField] private SeDatabase seDatabase;

    static public AudioManager Instance;

    private void Awake()
    {
        if(Instance != null && Instance != this)
        {
            Destroy(this);
            return;
        }
        Instance = this;
    }

    public void StartPlayingBgm(AudioClip clip)
    {
        if (bgmAudioSource.clip == clip)
            return;

        bgmAudioSource.clip = clip;
        bgmAudioSource.Play();
    }

    public void StopBgm()
    {
        bgmAudioSource.Stop();
    }

    public void PlayOneShotSe(string seClipName)
    {
        AudioClip seClip = seDatabase.GetSeClip(seClipName);
        if (seClip == null)
            return;

        seAudioSource.PlayOneShot(seClip);
    }

    public void PlayMultipleShotSe(string seClipName, int loopNum, float intervalRate = 1.0f)
    {
        PlayMultipleShotSeAsync(seClipName, loopNum, intervalRate).Forget();
    }

    private async UniTask PlayMultipleShotSeAsync(string seClipName, int loopNum, float intervalRate)
    {
        var cancellationToken = this.GetCancellationTokenOnDestroy();
        AudioClip seClip = seDatabase.GetSeClip(seClipName);

        for (int i = 0; i < loopNum; i++)
        {
            PlayOneShotSe(seClipName);
            await UniTask.Delay(TimeSpan.FromSeconds(seClip.length * intervalRate), cancellationToken: cancellationToken);
        }
    }
}
