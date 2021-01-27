using DG.Tweening;
using UnityEngine;

namespace Sound
{
    public class PlayBgm
    {
        private static PlayBgm _singleton;
        private AudioSource _audioSource;
        
        public static PlayBgm GetInstance()
        {
            if (_singleton == null)
            {
                _singleton = new PlayBgm();
                _singleton.Initialize();
            }
            return _singleton;
        }

        private void Initialize()
        {
            _audioSource = GameObject.Find("BGMSource").GetComponent<AudioSource>();
        }
        
        public void Play(string path)
        {
            var sound = Resources.Load(path) as AudioClip;
        }

        public void DoVolume(float endValue, float duration = 2f)
        {
            _audioSource.DOFade(endValue, duration).Play();
        }
    }
}