using UnityEngine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Consts;
using DG.Tweening;
using Dictionary;
using Extensions;
using Serializable;
using UniRx;
using UnityEngine.UI;

namespace Utils
{
    public struct AnnounceTextStruct
    {
        public List<GameObject> Texts;
        public int Speed;
    }
    /// <summary>
    /// アナウンステキスト処理クラス
    /// TODO 処理があんまりよくないので作り直したい
    /// </summary>
    public class AnnounceTextView : MonoSingleton<AnnounceTextView>
    {
        //private bool _isAnimation;
        private bool _isTextScroll;
        private List<GameObject> _texts = new List<GameObject>();
        private List<int> _queue = new List<int>();
        private bool _isFirst = true;

        public void AddText(List<string> texts, int speed = 1500)
        {
            if (_queue.Count == 0)
                _isFirst = true;
            foreach (var (text, index) in texts.Select((x, index) => (x, index)))
            {
                Observable.Timer(TimeSpan.FromMilliseconds(index * 100)).Subscribe(_ =>
                {
                    var textGameObject = Instantiate((GameObject) Resources.Load("Prefabs/Text/AnnounceTextContent"),
                        Vector3.zero, Quaternion.identity, transform);
                    textGameObject.transform.Find("Image/Text").GetComponent<Text>().text = text;
                    DOTween.To(() => textGameObject.GetComponent<CanvasGroup>().alpha,
                            (x) => textGameObject.GetComponent<CanvasGroup>().alpha = x, 1f, 0.3f).SetEase(Ease.OutSine)
                        .Play();
                    var background = textGameObject.transform.Find("Image");
                    background.GetComponent<RectTransform>().DOMoveX(0, 0.3f).SetEase(Ease.OutSine).Play();
                    _texts.Add(textGameObject);
                });
            }
            _queue.Add(texts.Count);
            ScrollAnimation();
        }

        private void ScrollAnimation()
        {
            if (_queue.Count != 0 && _isTextScroll == false)
            {
                _isTextScroll = true;
                if (_texts.Count < 7)
                {
                    //最初の一個はスクロールするまでの時間の猶予が長め
                    int speed = 1500;
                    if (_isFirst)
                    {
                        speed = 3000;
                        _isFirst = false;
                    }
                    Observable.Timer(TimeSpan.FromMilliseconds(speed)).Subscribe(_ => { Scroll(); });
                }
                //テキストが一定の数なら即スクロール
                else
                {
                    Scroll();
                }
            }
        }

        private void Scroll()
        {
            var size = _queue.First();
            var rect = transform.GetComponent<RectTransform>();
            var targetPos = new Vector2(0, rect.anchoredPosition.y + (32f * size));
            DOTween.To(() => rect.anchoredPosition, (x) => rect.anchoredPosition = x, targetPos, 0.3f)
                .SetEase(Ease.OutSine)
                .Play().OnComplete(() =>
                {
                    rect.anchoredPosition = new Vector2(0, 0);
                    _texts.GetRange(0, size).ForEach(x => { Destroy(x); });
                    _texts.RemoveRange(0, size);
                    _isTextScroll = false;
                    _queue.Remove(_queue.First());
                    ScrollAnimation();
                });
        }

        public void AddDamageText(BattlerSerializable fromBattler, string skillName, List<SkillDamages> damageses)
        {
            List<string> texts = new List<string>();
            StringBuilder sb = new StringBuilder();

            var fromColor = GuiDictionary.GetBattlerNameColor(fromBattler.battlerType);
            sb.Append($"<color={fromColor}>").Append(fromBattler.name).Append("</color>の").Append(skillName)
                .Append("！");
            texts.Add(sb.ToString());
            List<BattlerSerializable> deadBattlers = new List<BattlerSerializable>();
            damageses.ForEach(x =>
            {
                sb.Clear();
                var toBattler =
                    BattlerDictionary.GetBattlerByUniqId(x.targetUniqId);
                var toColor = GuiDictionary.GetBattlerNameColor(toBattler.battlerType);
                if (x.isHit)
                {
                    x.SkillDamage.ForEach(damage =>
                    {
                        var targetText = SkillsDicionary.SkillAnnounceValueTargetText(damage.valueTarget);
                        if (SkillsDicionary.IsDamageSkill(damage.valueTarget))
                        {
                            sb.Clear();
                            var damageColor = SkillsDicionary.SkillColor(damage.valueTarget);
                            sb.Append($"<color={toColor}>").Append(toBattler.name).Append("</color>に")
                                .Append($"<color={damageColor}>").Append(damage.damage)
                                .Append($"</color>の{targetText}");
                            texts.Add(sb.ToString());
                        }
                        else
                        {
                            sb.Clear();
                            if (damage.damage != 0)
                            {
                                sb.Append($"<color={toColor}>").Append(toBattler.name).Append("</color>は")
                                    .Append($"{targetText}");
                                texts.Add(sb.ToString());
                            }
                            else
                            {
                                var missText = SkillsDicionary.SkillAnnounceStateMissText(damage.valueTarget);
                                sb.Append($"<color={toColor}>").Append(toBattler.name).Append("</color>は")
                                    .Append($"{missText}");
                                texts.Add(sb.ToString());
                            }
                        }
                    });
                }
                else
                {
                    sb.Append($"<color={toColor}>").Append(toBattler.name).Append("</color>は回避した！");
                    texts.Add(sb.ToString());
                }

                if (x.isDead && deadBattlers.Find(battler => battler == toBattler) == null)
                    deadBattlers.Add(toBattler);
            });
            deadBattlers.ForEach(battler => { texts.Add(DeadText(battler)); });
            AddText(texts);
        }

        private string DeadText(BattlerSerializable battler)
        {
            var color = GuiDictionary.GetBattlerNameColor(battler.battlerType);
            StringBuilder sb = new StringBuilder();
            sb.Append($"<color={color}>{battler.name}</color>は死亡した");
            return sb.ToString();
        }

        public void PoisonText(BattlerSerializable battler, int value, bool isDead)
        {
            StringBuilder sb = new StringBuilder();
            var nameColor = GuiDictionary.GetBattlerNameColor(battler.battlerType);
            var skillColor = SkillsDicionary.SkillColor(SkillValueTarget.TARGET_POISON);
            var texts = new List<string>();
            sb.Append($"<color={nameColor}>{battler.name}は</color><color={skillColor}>{value}</color>の毒ダメージを受けた");
            texts.Add(sb.ToString());
            if (isDead)
                texts.Add(DeadText(battler));
            AddText(texts);
        }

        public void SleepText(BattlerSerializable battler)
        {
            StringBuilder sb = new StringBuilder();
            var nameColor = GuiDictionary.GetBattlerNameColor(battler.battlerType);
            var texts = new List<string>();
            sb.Append($"<color={nameColor}>{battler.name}</color>は寝ている");
            texts.Add(sb.ToString());
            AddText(texts);
        }

        public void WakeUpText(BattlerSerializable battler)
        {
            StringBuilder sb = new StringBuilder();
            var nameColor = GuiDictionary.GetBattlerNameColor(battler.battlerType);
            var texts = new List<string>();
            sb.Append($"<color={nameColor}>{battler.name}</color>は目を覚ました");
            texts.Add(sb.ToString());
            AddText(texts);
        }

        public void TurnStartText(int uniqId)
        {
            StringBuilder sb = new StringBuilder();
            BattlerSerializable battler = BattlerDictionary.GetBattlerByUniqId(uniqId);
            var nameColor = GuiDictionary.GetBattlerNameColor(battler.battlerType);
            sb.Append($"<color={nameColor}>{battler.name}</color>のターン");
            var texts = new List<string>();
            texts.Add(sb.ToString());
            AddText(texts);
        }

    }
}