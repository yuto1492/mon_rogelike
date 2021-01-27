using Consts;
using DG.Tweening;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

namespace Dungeon.View
{
    public class DungeonEffectView
    {
        private GameObject _transitionOpen = GameObject.Find("EffectCanvas/TransitionOpen");
        private GameObject _transitionClose = GameObject.Find("EffectCanvas/TransitionClose");

        public AsyncSubject<Unit> OpenTransition()
        {
            AsyncSubject<Unit> subject = new AsyncSubject<Unit>();
            var image = _transitionOpen.GetComponent<Image>();
            image.enabled = true;
            image.material.SetFloat(ShaderProperties.FadeAmount, 1f);
            DOTween.To(() => image.material.GetFloat(ShaderProperties.FadeAmount),
                (x) => image.material.SetFloat(ShaderProperties.FadeAmount, x), -0.1f, 1f).Play().OnComplete(() =>
            {
                image.enabled = false;
                subject.OnNext(Unit.Default);
                subject.OnCompleted();
            });
            return subject;
        }

        public  AsyncSubject<Unit> HideTransition()
        {
            AsyncSubject<Unit> subject = new AsyncSubject<Unit>();
            var transitionHideImage = _transitionClose.GetComponent<Image>();
            transitionHideImage.enabled = true;
            transitionHideImage.material.SetFloat(ShaderProperties.FadeAmount, 0f);
            DOTween.To(() => transitionHideImage.material.GetFloat(ShaderProperties.FadeAmount),
                (x) => transitionHideImage.material.SetFloat(ShaderProperties.FadeAmount, x), 
                1f, 1f).Play().OnComplete(() =>
            {
                transitionHideImage.enabled = false;
                subject.OnNext(Unit.Default);
                subject.OnCompleted();
            });
            return subject;
        }
    }
}