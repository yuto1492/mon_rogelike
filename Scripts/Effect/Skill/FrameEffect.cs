using System;
using DG.Tweening;
using TMPro.SpriteAssetUtilities;
using UniRx;
using UnityEngine;

namespace View.Skill
{
    public class FrameEffect : MonoBehaviour
    {
        public void Start()
        {
            gameObject.transform.position += new Vector3(-8f, 0.2f, 0);
            gameObject.transform.DOMoveX(9f, 0.8f).Play().OnComplete(() =>
            {
                var _mainModule = gameObject.GetComponent<ParticleSystem>().main;
                _mainModule.loop = false;
            });
        }

    }
}