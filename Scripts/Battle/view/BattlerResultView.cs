using System.Collections.Generic;
using System.Linq;
using Battle;
using Battler.Actor;
using Consts;
using DG.Tweening;
using Dictionary;
using Extensions;
using Model;
using Serializable;
using Sound;
using Structs;
using TMPro;
using UniRx;
using UniRx.Triggers;
using UnityEngine;
using UnityEngine.UI;
using Utils;
using Object = UnityEngine.Object;

namespace View.Battle.GUI
{
    public class BattlerResultView
    {
        private Transform _itemTransform;
        private Transform _actorTransform;
        public AsyncSubject<Unit> ShowResult(List<LootItemStruct> loots)
        {
            AsyncSubject<Unit> subject = new AsyncSubject<Unit>();
            PlayBgm.GetInstance().DoVolume(0.2f);
            _itemTransform = BattleGuiManager.Instance.resultGameObject.transform.Find("Result/Items");
            _actorTransform = BattleGuiManager.Instance.resultGameObject.transform.Find("Result/Actors");
            _actorTransform.GetComponent<CanvasGroup>().alpha = 0;
            BattlePresenter.GetInstance().BattlerSpriteView.Hide().Subscribe(_ =>
            {
                BattlePresenter.GetInstance().DisableObservable();
                BattleGuiManager.Instance.resultGameObject.transform.GetComponent<CanvasGroup>().alpha = 0;
                BattleGuiManager.Instance.skillUiGameObject.transform.GetComponent<CanvasGroup>().DOFade(0f, 0.4f)
                    .Play();
                BattleGuiManager.Instance.timeLineGameObject.transform.GetComponent<CanvasGroup>().DOFade(0f, 0.4f)
                    .Play();
                BattleGuiManager.Instance.resultGameObject.SetActive(true);
                BattleGuiManager.Instance.resultGameObject.transform.GetComponent<CanvasGroup>().DOFade(1f, 0.6f)
                    .Play();
                BattleGuiManager.Instance.resultGameObject.transform.GetComponent<CanvasGroup>().DOFade(1, 1f).Play()
                    .OnComplete(
                        () =>
                        {
                            AddLoots(loots).Subscribe(__ =>
                            {
                                var result = BattleGuiManager.Instance.resultGameObject.transform.Find("ScreenDimmed");
                                var disposables = new CompositeDisposable();
                                result.GetComponent<Image>().OnMouseUpAsButtonAsObservable().Subscribe(___ =>
                                {
                                    subject.OnNext(Unit.Default);
                                    subject.OnCompleted();
                                }).AddTo(disposables);
                                /*
                                var result = BattleGuiManager.Instance.resultGameObject.transform.Find("ScreenDimmed");
                                var disposables = new CompositeDisposable();
                                result.GetComponent<Image>().OnMouseUpAsButtonAsObservable().Subscribe(___ =>
                                {
                                    AddExp(exp).Subscribe(____ =>
                                    {
                                        subject.OnNext(Unit.Default);
                                        subject.OnCompleted();
                                    });
                                    disposables.Dispose();
                                }).AddTo(disposables);
                                */
                            });
                        });
            });
            return subject;
        }

        /// <summary>
        /// 報酬追加
        /// </summary>
        /// <param name="loots"></param>
        /// <returns></returns>
        private AsyncSubject<Unit> AddLoots(List<LootItemStruct> loots)
        {
            AsyncSubject<Unit> subject = new AsyncSubject<Unit>();
            
            ObservableUtils.Timer(loots.Count * 100 + 400).Subscribe(_ =>
            {
                subject.OnNext(Unit.Default);
                subject.OnCompleted();
            });
            foreach (var (loot, index) in loots.Select((x, i) => (x, i)))
            {
                var pos = new Vector3(-500, -90 - (index * 60), 0);
                var itemObject = Object.Instantiate((GameObject) Resources.Load("Prefabs/GUI/ResultItem"),
                    Vector3.zero, Quaternion.identity, _itemTransform);
                itemObject.transform.localPosition = pos;
                itemObject.GetComponent<CanvasGroup>().alpha = 0;
                var sprite = ItemDictionary.GetLootSprite(loot);
                //var sprite = AssetDatabase.LoadAssetAtPath<Sprite>("Assets/Resources/Sprites/Items/" + item.imageData.spritePath);
                itemObject.transform.Find("Icon").GetComponent<Image>().sprite = sprite;
                var text = ItemDictionary.GetText(loot);
                itemObject.transform.Find("Text").GetComponent<Text>().text = text;
                itemObject.transform.Find("Text").GetComponent<Text>().color = ItemDictionary.GetColor(loot.ItemId);
                itemObject.transform.Find("Type").GetComponent<TextMeshProUGUI>().text = ItemDictionary.GetCategoryName(loot.ItemId);
                itemObject.transform.Find("Type").GetComponent<TextMeshProUGUI>().color =
                    ItemDictionary.GetColor(loot.ItemId);
                ObservableUtils.Timer(100 * index).Subscribe(_ =>
                {
                    itemObject.transform.DOLocalMoveX(0, 0.4f).Play();
                    itemObject.transform.GetComponent<CanvasGroup>().DOFade(1f,0.4f).Play();
                });
            }
            return subject;
        }

        
        /// <summary>
        /// 経験値追加
        /// TODO 使わないので仕様をfixしたら消す
        /// </summary>
        /// <param name="addExp"></param>
        /// <returns></returns>
        private AsyncSubject<Unit> AddExp(int addExp)
        {
            AsyncSubject<Unit> subject = new AsyncSubject<Unit>();
            _itemTransform.GetComponent<CanvasGroup>()
                .DOFade(0, 0.2f).Play();
            _actorTransform.GetComponent<CanvasGroup>().DOFade(1, 0.2f).Play();

            var actors = MemberDataModel.Instance.GetActorData();

            var maxDuration = 0f;
            actors.ForEach(actor =>
            {
                var duration = GetAddExpDuration(actor, addExp);
                if (duration > maxDuration)
                {
                    maxDuration = duration;
                }
            });
            
            var countUp = 0;
            DOVirtual.Float(0, 1, maxDuration, value =>
            {
                countUp++;
                if (countUp > 10)
                {
                    countUp = 0;
                    //TODO SEないほうがいいかも
                    //PlaySe.GetInstance().Play("SE/GaugeUp", 0.4f);
                }
            }).SetEase(Ease.InQuad).Play();
            actors.ForEach(actor =>
            {
                var actorObject = Object.Instantiate((GameObject) Resources.Load("Prefabs/GUI/ResultActor"),
                    Vector3.zero, Quaternion.identity, _actorTransform);
                var image = actorObject.transform.Find("Image").GetComponent<Image>();
                image.sprite =
                    MonsterDicionary.GetSprite(actor.monsterId);
                image.GetComponent<RectTransform>().localScale = MonsterDicionary.GetScale(actor.monsterId);
                image.SetNativeSize();
                var bar = actorObject.transform.Find("ExpBar").GetComponent<Slider>();
                bar.value = (float) actor.hasExp / actor.needExp;
                var needExpText = actorObject.transform.Find("ExpBar/ExpText/Exp/NeedExpText")
                    .GetComponent<TextMeshProUGUI>();
                needExpText.text = $"/{actor.needExp.ToString()}";
                var hasExpText = actorObject.transform.Find("ExpBar/ExpText/Exp/HasExpText")
                    .GetComponent<TextMeshProUGUI>();
                hasExpText.text = actor.hasExp.ToString();
                var addExpText = actorObject.transform.Find("ExpBar/AddExpText").GetComponent<TextMeshProUGUI>();
                addExpText.text = $"+{addExp}";
                var levelText = actorObject.transform.Find("ExpBar/LvText/Level").GetComponent<TextMeshProUGUI>();
                levelText.text = actor.level.ToString();

                // TODO
                float virtualHasExp = actor.hasExp;
                float virtualNeedExp = actor.needExp;
                int virtualLevel = actor.level;
                var currentValue = 0f;
                var duration = GetAddExpDuration(actor, addExp);
                DOVirtual.Float(0, addExp, duration, value =>
                {
                    var addValue = value - currentValue;
                    currentValue = value;
                    virtualHasExp += addValue;
                    if (virtualHasExp >= virtualNeedExp)
                    {
                        virtualHasExp -= virtualNeedExp;
                        virtualLevel++;
                        virtualNeedExp = ActorLogic.CalcNeedExp(actor.monsterId, virtualLevel);
                        needExpText.text = $"/{((int) virtualNeedExp).ToString()}";
                        levelText.text = virtualLevel.ToString();
                        LevelUpPopUp(actorObject.transform);
                        PlaySe.GetInstance().Play("SE/LevelUp");
                    }

                    hasExpText.text = ((int) virtualHasExp).ToString();
                    bar.value = virtualHasExp / virtualNeedExp;
                }).SetEase(Ease.InQuad).Play();
            });
            return subject;
        }

        private float GetAddExpDuration(BattlerSerializable battler, int addExp)
        {
            var duration = 2f;

            var levelUpSize = ActorLogic.VirtualLevelUpSize(battler.uniqId, addExp);
            Debug.Log(levelUpSize);
            if (levelUpSize > 0)
            {
                duration *= levelUpSize * 0.5f;
                if (duration > 9f)
                {
                    return 9f;
                }
            }

            return duration;
        }

        private void LevelUpPopUp(Transform parentTransform)
        {
            var levelUpTextPrefab = Object.Instantiate((GameObject) Resources.Load("Prefabs/Text/LevelUpText"),
                parentTransform);
            levelUpTextPrefab.transform.AddPosX(Random.Range(-0.5f, 0.5f));
            levelUpTextPrefab.GetComponent<TextMeshProUGUI>().alpha = 0;
            levelUpTextPrefab.GetComponent<TextMeshProUGUI>().DOFade(1f, 0.2f).Play();
            levelUpTextPrefab.GetComponent<RectTransform>()
                .DOLocalMoveY(levelUpTextPrefab.GetComponent<RectTransform>().localPosition.y + 10, 0.2f).Play()
                .OnComplete(
                    () =>
                    {
                        levelUpTextPrefab.GetComponent<RectTransform>()
                            .DOLocalMoveY(levelUpTextPrefab.GetComponent<RectTransform>().localPosition.y + 10, 0.2f)
                            .Play();
                        levelUpTextPrefab.GetComponent<TextMeshProUGUI>().DOFade(0f, 0.2f).Play();
                    });
        }

    }
}