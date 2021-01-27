using UnityEngine;

namespace Sound
{
    public class PlaySe
    {
        private static PlaySe _singleton;
        private AudioSource _audioSource;
        
        public static PlaySe GetInstance()
        {
            if (_singleton == null)
            {
                _singleton = new PlaySe();
                _singleton.Initialize();
            }
            return _singleton;
        }

        private void Initialize()
        {
            _audioSource = GameObject.Find("AudioSource").GetComponent<AudioSource>();
        }
        
        public void Play(string path, float volume = 0)
        {
            var sound = Resources.Load(path) as AudioClip;
            if (volume != 0)
            {
                _audioSource.volume = volume;
            }
            _audioSource.PlayOneShot(sound);
        }
    }
}