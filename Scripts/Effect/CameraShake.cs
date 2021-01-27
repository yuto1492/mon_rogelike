using System;
using DG.Tweening;
using UnityEngine;

namespace View
{
    public class CameraShake : MonoBehaviour
    {
        public float duration;
        public float strength;

        public void Start()
        {
            var camera = GameObject.Find("MainCamera");
            camera.transform.DOShakePosition(duration,strength)
                .Play();
        }
    }
}