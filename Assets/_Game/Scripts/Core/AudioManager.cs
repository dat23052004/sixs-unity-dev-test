using System.Collections.Generic;
using UnityEngine;

public class AudioManager : Singleton<AudioManager>
{
        [Header("Audio Sources")]
        [SerializeField] private AudioSource _bgmSource;
        [SerializeField] private AudioSource _sfxSource;

        [Header("BGM")]
        [SerializeField] private AudioClip _mainMenuBGM;
        [SerializeField] private AudioClip _gameplayBGM;

        [Header("SFX")]
        [SerializeField] private List<AudioClip> _sfxClips = new();

        private readonly Dictionary<string, AudioClip> _sfxMap = new();

        protected override void Awake()
        {
            base.Awake();
            foreach (var clip in _sfxClips)
                if (clip != null && !_sfxMap.ContainsKey(clip.name))
                    _sfxMap[clip.name] = clip;
        }

        void OnEnable()  => GameManager.OnGameStateChanged += HandleStateChanged;
        void OnDisable() => GameManager.OnGameStateChanged -= HandleStateChanged;

        private void HandleStateChanged(GameState state)
        {
            switch (state)
            {
                case GameState.MainMenu:  PlayBGM(_mainMenuBGM);  break;
                case GameState.Playing:   PlayBGM(_gameplayBGM);  break;
                case GameState.Paused:    _bgmSource.Pause();     break;
                case GameState.GameOver:  _bgmSource.Stop();      break;
            }
        }

        public void PlayBGM(AudioClip clip)
        {
            if (clip == null || _bgmSource.clip == clip) return;
            _bgmSource.clip = clip;
            _bgmSource.loop = true;
            _bgmSource.Play();
        }

        public void PlaySFX(string clipName)
        {
            if (_sfxMap.TryGetValue(clipName, out var clip))
                _sfxSource.PlayOneShot(clip);
            else
                Debug.LogWarning($"[AudioManager] SFX not found: {clipName}");
        }

        public void PlaySFX(AudioClip clip)
        {
            if (clip != null) _sfxSource.PlayOneShot(clip);
        }

        public void SetBGMVolume(float volume) => _bgmSource.volume = Mathf.Clamp01(volume);
        public void SetSFXVolume(float volume) => _sfxSource.volume = Mathf.Clamp01(volume);

        public void StopBGM()                  => _bgmSource.Stop();
        public void MuteAll(bool mute)
        {
            _bgmSource.mute = mute;
            _sfxSource.mute = mute;
        }
}
