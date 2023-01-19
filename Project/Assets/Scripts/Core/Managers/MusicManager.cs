using UnityEngine;
using Utility;

namespace Core.Managers
{
    [RequireComponent(typeof(AudioSource))]
    public class MusicManager : Singleton<MusicManager>
    {
        #region Fields
        [Header("Musics"), Space]
        [SerializeField] private AudioClip[] _musics;
        private AudioSource _audioSource;
        #endregion

        #region Enumerations
        public enum Music
        {
            General = 0,
            Victory = 1
        }
        #endregion

        #region Methods
        private void Start()
        {
            _audioSource = GetComponent<AudioSource>();
        }
        public void PlayMusic(Music music)
        {
            _audioSource.clip = _musics[(int)music];
            _audioSource.Play();
        }
        public bool IsPlayingSameMusic(Music music) => _audioSource.clip == _musics[(int)music];

        public void PauseOrContinueMusic()
        {
            if (_audioSource.isPlaying)
                _audioSource.Pause();
            else
                _audioSource.UnPause();
        }
        #endregion
    }
}