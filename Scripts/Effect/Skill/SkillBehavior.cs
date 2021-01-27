using System;
using System.Collections.Generic;
using System.Linq;
using Battle;
using Consts;
using Container;
using DG.Tweening;
using Dictionary;
using Serializable;
using TMPro;
using UniRx;
using UnityEngine;
using Utils;
using Random = UnityEngine.Random;

namespace View.Skill
{
    [Serializable]
    public class SoundData
    {
        public AudioClip sound;
        public float frame = 0;
        public float pitch = 1;
        public float volume = 1;
    }

    public class SkillBehavior : MonoBehaviour
    {
        public List<SoundData> hitSound;
        public List<SoundData> sound;
        public float frame;
        private AudioSource _audioSource;
        public Color hitColor = Color.red;
        private int _popupCount = 0;
        public GameObject effectPrefab;
        public bool isAll;
        
        //TODO コンフィグで
        private float _volumeRate = 0.1f;
        
        /// <summary>
        /// 一つのパーティクルで済ませる場合はtrue
        /// </summary>
        public bool isAllOneEffect;

        public void Start()
        {
            _audioSource = GameObject.Find("AudioSource").GetComponent<AudioSource>();
        }

        public void EffectPlay(int targetUniqId)
        {
            Instantiate(effectPrefab, BattleDictionary.GetTransformByUniqId(targetUniqId).localPosition,
                gameObject.transform.rotation);
            PlaySound();
            //TODO
            ObservableUtils.Timer(400).Subscribe(_ => { Destroy(gameObject); });
        }

        public void Play(List<SkillDamages> damageses)
        {
            SubjectContainer container = new SubjectContainer();
            
            if (isAll)
            {
                if (isAllOneEffect)
                {
                    Instantiate(effectPrefab,
                        BattleDictionary.GetTransformByUniqId(damageses.First().targetUniqId).localPosition,
                        gameObject.transform.rotation);
                }
                else
                {
                    damageses.ForEach(damage =>
                    {
                        if (BattlerDictionary.IsDead(damage.targetUniqId) == false)
                        {
                            Instantiate(effectPrefab,
                                BattleDictionary.GetTransformByUniqId(damage.targetUniqId).localPosition,
                                gameObject.transform.rotation);
                        }
                    });
                }
            }

            var maxTime = damageses.Count * 300;
            foreach (var (x, index) in damageses.Select((x, index) => (x, index)))
            {
                ObservableUtils.Timer(300 * index).Subscribe(_ =>
                {
                    if (isAll == false)
                    {
                        Instantiate(effectPrefab, BattleDictionary.GetTransformByUniqId(damageses.First().targetUniqId).localPosition,
                            gameObject.transform.rotation);
                    }

                    if (x.isHit)
                    {
                        HitEffect(x.targetUniqId);
                        x.SkillDamage.ForEach(damage =>
                        {
                            DamagePopup(BattleDictionary.GetTransformByUniqId(damageses.First().targetUniqId), damage, x.targetUniqId);
                        });
                        hitSound.ForEach(item =>
                        {
                            ObservableUtils.Timer(frame * 100f).Subscribe(__ =>
                            {
                                if (item.pitch == 0)
                                    item.pitch = 1;
                                if (item.volume == 0)
                                    item.volume = 1;
                                item.volume = item.volume * _volumeRate;
                                _audioSource.pitch = item.pitch;
                                _audioSource.volume = item.volume;
                                _audioSource.PlayOneShot(item.sound);
                            });
                        });
                    }
                    else
                    {
                        DodgePopup(BattleDictionary.GetTransformByUniqId(damageses.First().targetUniqId));
                    }
                });
            }
            if (maxTime < sound.Count * 100)
                maxTime = sound.Count * 100;
            PlaySound();
            //TODO
            ObservableUtils.Timer(maxTime + 100).Subscribe(_ =>
            {
                Destroy(gameObject);
            });
        }

        private void PlaySound()
        {

            sound.ForEach(x =>
            {
                ObservableUtils.Timer(frame * 100).Subscribe(__ =>
                {
                    if (x.pitch == 0)
                        x.pitch = 1;
                    if (x.volume == 0)
                        x.volume = 1 * _volumeRate;
                    x.volume = x.volume * _volumeRate;
                    _audioSource.pitch = x.pitch;
                    _audioSource.volume = x.volume;
                    _audioSource.PlayOneShot(x.sound);
                });
            });
        }

        private void DamagePopup(Transform target, SkillDamage damage, int uniqId)
        {
            if (damage.isResist == false)
            {
                switch (damage.valueTarget)
                {
                    case SkillValueTarget.SLEEP:
                        SleepPopup(uniqId);
                        break;
                    default:
                        string color = SkillsDicionary.SkillColor(damage.valueTarget);
                        string damageText = $"<color={color}>{damage.damage}";
                        GameObject text =
                            Instantiate(Resources.Load("Prefabs/Text/DamageText", typeof(GameObject)) as GameObject);
                        var pos = target.GetComponent<Transform>().localPosition;
                        pos = PopupPos(pos);

                        text.GetComponent<Transform>().localPosition = pos;
                        var tMesh = text.GetComponent<TextMeshPro>();
                        tMesh.rectTransform.localScale = new Vector3(1.4f, 1.4f);
                        tMesh.rectTransform.pivot = new Vector2(0, 0);
                        tMesh.color = new Color(tMesh.color.r, tMesh.color.g, tMesh.color.b, 0);
                        tMesh.text = damageText;
                        TextPopUpFadeInOut(tMesh, text);
                        break;
                }
            }
            else
            {
                ResistPopup(target);
            }
        }

        private void SleepPopup(int uniqId)
        {
            EffectManager.Instance.SleepEffect(uniqId);
        }

        private void ResistPopup(Transform target)
        {
            GameObject text = Instantiate(Resources.Load("Prefabs/Text/DodgeText", typeof(GameObject)) as GameObject);
            var pos = target.GetComponent<Transform>().localPosition;
            pos = PopupPos(pos);
            text.GetComponent<Transform>().localPosition = pos;
            var tMesh = text.GetComponent<TextMeshPro>();
            tMesh.text = "Resist";
            TextPopUpFadeInOut(tMesh, text);
        }
        
        private void DodgePopup(Transform target)
        {
            GameObject text = Instantiate(Resources.Load("Prefabs/Text/DodgeText", typeof(GameObject)) as GameObject);
            var pos = target.GetComponent<Transform>().localPosition;
            pos = PopupPos(pos);
            text.GetComponent<Transform>().localPosition = pos;
            var tMesh = text.GetComponent<TextMeshPro>();
            TextPopUpFadeInOut(tMesh, text);
        }

        private void HitEffect(int uniqId)
        {
            BattlePresenter.GetInstance().BattlerSpriteView.HitEffect(uniqId,hitColor);
        }

        private Vector3 PopupPos(Vector3 pos)
        {
            switch (_popupCount % 4)
            {
                case 0:
                    pos += new Vector3(Random.Range(0.5f, 1f), Random.Range(0f, 0.8f), 0);
                    break;
                case 1:
                    pos += new Vector3(Random.Range(-0.5f, -1), Random.Range(0f, 0.8f), 0);
                    break;
                case 2:
                    pos += new Vector3(Random.Range(0.5f, 1f), Random.Range(-1.2f, 0), 0);
                    break;
                case 3:
                    pos += new Vector3(Random.Range(-0.5f, -1), Random.Range(-1.2f, 0), 0);
                    break;
            }

            _popupCount++;
            return pos;
        }

        private void TextPopUpFadeInOut(TextMeshPro tMesh, GameObject textObject)
        {
            tMesh.rectTransform.DOScale(1f, 0.2f).Play();
            tMesh.DOFade(1f, 0.3f).Play().OnComplete(() =>
            {
                Vector3 targetPos = textObject.transform.localPosition + new Vector3(0, 1.5f, 0);
                tMesh.DOFade(0f, 0.8f).Play();
                textObject.transform.DOMove(targetPos, 0.8f).SetEase(Ease.OutQuart).Play().OnComplete(() =>
                {
                    Destroy(textObject);
                });
            });
        }
    }
}