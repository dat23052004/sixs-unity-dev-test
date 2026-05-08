using UnityEngine;

public class AudioManager : Singleton<AudioManager>
{
    [Header("Sources")]
    [SerializeField] private AudioSource _bgmSource;
    [SerializeField] private AudioSource _sfxSource;

    [Header("BGM")]
    [SerializeField] private AudioClip _bgm;

    [Header("SFX")]
    [SerializeField] private AudioClip _shootSFX;
    [SerializeField] private AudioClip _winSFX;
    [SerializeField] private AudioClip _confettiSFX;

    protected override void Awake()
    {
        base.Awake();
        if (_bgm != null)
        {
            _bgmSource.clip = _bgm;
            _bgmSource.loop = true;
            _bgmSource.Play();
        }
    }

    public void PlayShootSFX()    => _sfxSource.PlayOneShot(_shootSFX);
    public void PlayWinSFX()      => _sfxSource.PlayOneShot(_winSFX);
    public void PlayConfettiSFX() => _sfxSource.PlayOneShot(_confettiSFX);

    public void SetBGMVolume(float v) => _bgmSource.volume = Mathf.Clamp01(v);
    public void SetSFXVolume(float v) => _sfxSource.volume = Mathf.Clamp01(v);
}
