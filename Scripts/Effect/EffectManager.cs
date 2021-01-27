using System;
using System.Collections.Generic;
using System.Linq;
using Consts;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using Dictionary;
using Serializable;
using TMPro;
using UniRx;
using UniRx.Triggers;
using UnityEngine;
using UnityEngine.Events;
using Utils;
using View.Battle;
using View.Skill;
using Random = UnityEngine.Random;

namespace View
{
    public class EffectManager : SingletonMonoBehaviour<EffectManager>
    {
        private Vector3 cameraDefaultPos;
        private float cameraDefaultSize;
        private int _popupCount;
        override protected void Awake()
        {
            base.Awake();
        }

        public AsyncSubject<Unit> SkillToTarget(string skillId, List<SkillDamages> damageses)
        {
            AsyncSubject<Unit> subject = new AsyncSubject<Unit>();
            var skill = SkillsDicionary.GetSkillById(skillId);
            var effect = Instantiate((GameObject) Resources.Load("Prefabs/Skills/" + skill.effectAnimationId));
            effect.GetComponent<SkillBehavior>().Play(damageses);
            //演出が終わったタイミングではなく
            effect.GetComponent<ParticleSystem>().OnDestroyAsObservable().Subscribe(_ =>
            {
                //subject.OnNext(Unit.Default);
                //subject.OnCompleted();
            });
            Observable.Timer(TimeSpan.FromMilliseconds(400)).Subscribe(_ => 
            {
                subject.OnNext(Unit.Default);
                subject.OnCompleted();
            });
            return subject;
        }

        public AsyncSubject<Unit> SkillToAll(string skillId, List<SkillDamages> damageses)
        {
            AsyncSubject<Unit> subject = new AsyncSubject<Unit>();
            var skill = SkillsDicionary.GetSkillById(skillId);

            var effect = Instantiate((GameObject) Resources.Load("Prefabs/Skills/" + skill.effectAnimationId));
            effect.GetComponent<Transform>().position = new Vector3(0,0,0);
            effect.GetComponent<SkillBehavior>().Play(damageses);
            //演出が終わったタイミングではなく
            effect.GetComponent<ParticleSystem>().OnDestroyAsObservable().Subscribe(_ =>
            {
                //subject.OnNext(Unit.Default);
                //subject.OnCompleted();
            });
            Observable.Timer(TimeSpan.FromMilliseconds(400)).Subscribe(_ => 
            {
                subject.OnNext(Unit.Default);
                subject.OnCompleted();
            });
            return subject;
        }

        public AsyncSubject<Unit> TargetCamera(GameObject targetObject)
        {
            AsyncSubject<Unit> subject = new AsyncSubject<Unit>();
            
            Camera camera = GameObject.Find("MainCamera").GetComponent<Camera>();
            cameraDefaultSize = camera.orthographicSize;
            cameraDefaultPos = camera.transform.localPosition;
            Vector3 targetPos = targetObject.GetComponent<Transform>().localPosition;
            targetPos.z = cameraDefaultPos.z;
            Func<UniTask> asyncFunc = async () =>
            {
                await new DOTweenList(
                    DOTween.To(() => camera.orthographicSize,
                        (x) => camera.orthographicSize = x, 3.5f, 0.3f).Play(),
                    DOTween.To(() => camera.transform.localPosition,
                        (x) => camera.transform.localPosition = x, targetPos, 0.3f).Play()
                );
                subject.OnNext(Unit.Default);
                subject.OnCompleted();
            };
            asyncFunc();
            
            return subject;
        }
        
        public AsyncSubject<Unit> RefreshCamera()
        {
            AsyncSubject<Unit> subject = new AsyncSubject<Unit>();
            
            Camera camera = GameObject.Find("MainCamera").GetComponent<Camera>();
            Func<UniTask> asyncFunc = async () =>
            {
                await new DOTweenList(
                    DOTween.To(() => camera.orthographicSize,
                        (x) => camera.orthographicSize = x, cameraDefaultSize, 0.3f).Play(),
                    DOTween.To(() => camera.transform.localPosition,
                        (x) => camera.transform.localPosition = x, cameraDefaultPos, 0.3f).Play()
                );
                subject.OnNext(Unit.Default);
                subject.OnCompleted();
            };
            asyncFunc();
            
            return subject;
        }

        public AsyncSubject<Unit> PoisonEffect(int uniqId, List<SkillDamages> damageses)
        {
            var targetTransform = BattleDictionary.GetTransformByUniqId(uniqId);
            AsyncSubject<Unit> subject = new AsyncSubject<Unit>();
            var poison = Instantiate((GameObject) Resources.Load("Prefabs/Status/Poison"));
            var poisonEffect = poison.GetComponent<SkillBehavior>();
            poisonEffect.Play(damageses);
            poison.transform.localPosition = targetTransform.localPosition;
            ObservableUtils.Timer(200).Subscribe(_ =>
            {
                subject.OnNext(Unit.Default);
                subject.OnCompleted();
            });
            return subject;
        }

        public AsyncSubject<Unit> SleepEffect(int uniqId)
        {
            AsyncSubject<Unit> subject = new AsyncSubject<Unit>();
            var sleep = Instantiate((GameObject) Resources.Load("Prefabs/Status/Sleep"));
            var skillBehavior = sleep.GetComponent<SkillBehavior>();
            skillBehavior.EffectPlay(uniqId);
            ObservableUtils.Timer(200).Subscribe(_ =>
            {
                subject.OnNext(Unit.Default);
                subject.OnCompleted();
            });
            return subject;
        }

    }
}