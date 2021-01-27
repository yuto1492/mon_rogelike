using System;
using System.Collections.Generic;
using System.Linq;
using Battle;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using DG.Tweening.Core;
using DG.Tweening.Plugins.Options;
using Dictionary;
using Serializable;
using UniRx;
using UnityEngine;
using UnityEngine.UI;
using Object = UnityEngine.Object;

namespace View.Battle
{
    public struct TimelineViewStruct
    {
        public GameObject Card;
        public int Id;
    }
    public class TimelineView
    {
        private List<TimelineViewStruct> _turnCards = new List<TimelineViewStruct>();
        private const float OFFSET_X = 135;
        private const int SCHEDULE_SIZE = 8;
        public void Initialize(TimeLine timeline)
        {
            List<TimelineSerializableData> timelineData = timeline.TimelineData;
            List<int> schedule = timeline.TimelineSchedule;
            for (int v = 0; v < SCHEDULE_SIZE; v++)
            {
                var item = BattleDictionary.GetTimelineById(timelineData, schedule[v]);
                AddCard(item);
            }
        }

        public AsyncSubject<Unit> AddCard(TimelineSerializableData item)
        {
            AsyncSubject<Unit> subject = new AsyncSubject<Unit>();

            var turnCard = CreateCard(item);
            AddCardAnimation(turnCard.transform, _turnCards.Count).OnComplete(() =>
            {
                ActiveCardGrow();
                subject.OnNext(Unit.Default);
                subject.OnCompleted();
            });
            return subject;
        }

        public AsyncSubject<Unit> AddCards(List<TimelineSerializableData> items)
        {
            AsyncSubject<Unit> subject = new AsyncSubject<Unit>();
            Func<UniTask> asyncFunc = async () =>
            {
                var doTweenList = new DOTweenList();
                items.ForEach(item =>
                {
                    var turnCard = CreateCard(item);
                    doTweenList.Add(AddCardAnimation(turnCard.transform, _turnCards.Count));
                });
                await doTweenList;
                ActiveCardGrow();
                subject.OnNext(Unit.Default);
                subject.OnCompleted();
            };
            asyncFunc();
            return subject;
        }

        private void ActiveCardGrow()
        {
            //TODO
        }

        private GameObject CreateCard(TimelineSerializableData item)
        {
            GameObject turnCard;
            if (BattleDictionary.IsActor(item.uniqId))
            {
                turnCard = Object.Instantiate((GameObject) Resources.Load("Prefabs/Battle/TurnCard"),
                    Vector3.zero, Quaternion.identity, GameObject.Find("Timeline/TurnCards").transform);
            }
            else
            {
                turnCard = Object.Instantiate((GameObject) Resources.Load("Prefabs/Battle/EnemyTurnCard"),
                    Vector3.zero, Quaternion.identity, GameObject.Find("Timeline/TurnCards").transform);
            }
            GameObject battlerImage = turnCard.transform.Find("Mask/BattlerImage").gameObject;
            string monsterId = BattlerDictionary.GetBattlerByUniqId(item.uniqId).monsterId;
            MonsterSerializable monster = MonsterDicionary.GetMonsterData(monsterId);
            var sprite = Resources.Load<Sprite>(monster.imageData.spritePath);
            //Sprite sprite = AssetDatabase.LoadAssetAtPath<Sprite>(monster.imageData.spritePath);
            var image = battlerImage.GetComponent<Image>();
            image.sprite = sprite;
            battlerImage.GetComponent<RectTransform>().sizeDelta =
                new Vector3(sprite.rect.width, sprite.rect.height);
            battlerImage.GetComponent<RectTransform>().localPosition = new Vector3(monster.imageData.timelineCard.x,
                monster.imageData.timelineCard.y, 0);
            battlerImage.GetComponent<RectTransform>().localScale =
                new Vector3(monster.imageData.timelineCard.scaleX, monster.imageData.timelineCard.scaleY, 0);
            _turnCards.Add(new TimelineViewStruct()
            {
                Card = turnCard,
                Id = item.id
            });
            return turnCard;
        }

        private TweenerCore<Vector3, Vector3, VectorOptions> AddCardAnimation(Transform turnCard, int index)
        {
            float posX = -490 + ((index - 1) * OFFSET_X);
            turnCard.GetComponent<RectTransform>().localPosition = new Vector3(posX + 150, 0, 0);
            Vector3 targetPos = new Vector3(posX, 0, 0);
            return DOTween.To(() => turnCard.GetComponent<RectTransform>().localPosition,
                (x) => turnCard.GetComponent<RectTransform>().localPosition = x, targetPos, 0.2f).Play();
        }

        /// <summary>
        /// 先頭を削除する
        /// </summary>
        public void DepopSchedule()
        {
            GameObject turnCard = _turnCards[0].Card;
            var localPosition = turnCard.GetComponent<RectTransform>().localPosition;
            var targetPosition = localPosition + new Vector3(0, +300f, 0);
            DOTween.To(() => turnCard.GetComponent<RectTransform>().localPosition,
                (x) => turnCard.GetComponent<RectTransform>().localPosition = x, targetPosition, 0.4f).Play();
            DOTween.To(() => turnCard.GetComponent<CanvasGroup>().alpha,
                (x) => turnCard.GetComponent<CanvasGroup>().alpha = x, 0f, 0.4f).Play().OnComplete(() =>
            {
                Object.Destroy(turnCard);
            });
            _turnCards.RemoveAt(0);

            foreach (var (x, index) in _turnCards.Select((x, index) => (x, index)))
            {
                LeftScroll(x.Card, index);
            }
        }

        public AsyncSubject<Unit> RemoveSchedule(List<int> ids)
        {
            AsyncSubject<Unit> subject = new AsyncSubject<Unit>();

            Func<UniTask> asyncFunc = async () =>
            {
                var deleteCards = new List<GameObject>();
                var doTweenList = new DOTweenList();
                ids.ForEach(id =>
                {
                    var cards = _turnCards.FindAll(x => x.Id == id);
                    cards.ForEach(card =>
                    {
                        var localPosition = card.Card.GetComponent<RectTransform>().localPosition;
                        var targetPosition = localPosition + new Vector3(0, +300f, 0);
                        doTweenList.Add(DOTween.To(() => card.Card.GetComponent<RectTransform>().localPosition,
                            (x) => card.Card.GetComponent<RectTransform>().localPosition = x, targetPosition, 0.4f).Play());
                        doTweenList.Add(DOTween.To(() => card.Card.GetComponent<CanvasGroup>().alpha,
                            (x) => card.Card.GetComponent<CanvasGroup>().alpha = x, 0f, 0.4f).Play());
                        _turnCards.Remove(card);
                        deleteCards.Add(card.Card);
                    });
                });
                await doTweenList.PlayForward();
                deleteCards.ForEach(card =>
                {
                    Object.Destroy(card);
                });
                LeftJustified().Subscribe(_ =>
                {
                    subject.OnNext(Unit.Default);
                    subject.OnCompleted();
                });
            };
            asyncFunc();
            return subject;
        }

        /// <summary>
        /// 空きを詰める
        /// </summary>
        /// <returns></returns>
        public AsyncSubject<Unit> LeftJustified()
        {
            AsyncSubject<Unit> subject = new AsyncSubject<Unit>();
            Func<UniTask> asyncFunc = async () =>
            {
                var doTweenList = new DOTweenList();
                foreach (var (card, index) in _turnCards.Select((card, index) => (card, index)))
                {
                    var rect = card.Card.GetComponent<RectTransform>();
                    float posX = -490 + (index * OFFSET_X);
                    Vector3 pos = new Vector3(posX, 0, 0);
                    doTweenList.Add(DOTween.To(() => rect.localPosition, (x) => rect.localPosition = x, pos, 0.4f)
                        .Play());
                }

                await doTweenList;
                subject.OnNext(Unit.Default);
                subject.OnCompleted();
            };
            asyncFunc();
            return subject;
        }

        private void LeftScroll(GameObject turnCard,int index)
        {
            var localPosition = turnCard.GetComponent<RectTransform>().localPosition;
            var targetPosition = localPosition + new Vector3(-OFFSET_X, 0, 0);
            DOTween.To(() => turnCard.GetComponent<RectTransform>().localPosition,
    (x) => turnCard.GetComponent<RectTransform>().localPosition = x, targetPosition, 0.2f).Play();
        }
    }
}