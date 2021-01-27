using System.Collections.Generic;
using System.Linq;
using Consts.Enums;
using Dictionary;
using Model;
using Serializable;
using UniRx;
using Utils;
using View.Battle;
using Random = UnityEngine.Random;

namespace Battle
{
    
    public class TimeLine
    {
        /// <summary>
        /// タイムラインで利用するシリアライズデータ
        /// </summary>
        private List<TimelineSerializableData> _timelineData = new List<TimelineSerializableData>();
        /// <summary>
        /// タイムラインのゴール
        /// 平均速度の10倍
        /// </summary>
        private int _timeline;
        /// <summary>
        /// 平均素早さ
        /// </summary>
        private int _spdAverage;
        /// <summary>
        /// タイムラインのスケジュール
        /// </summary>
        List<int> _timelineSchedule = new List<int>();

        private const int TIMELINE_RATE = 10;
        private const int SCHEDULE_SIZE = 8;

        private TimelineView _timelineView;
        
        public void Initialize()
        {
            _timelineSchedule = new List<int>();
            _timelineData = new List<TimelineSerializableData>();
            MemberDataModel memberData = MemberDataModel.Instance;
            ActorDataModel actorData = ActorDataModel.Instance;
            memberData.Data.ForEach(x =>
            {
                BattlerSerializable battler = actorData.ByIndex(x.index);
                _timelineData.Add(new TimelineSerializableData
                {
                    id = _timelineData.Count,
                    uniqId = battler.uniqId,
                    battlerType = BattlerEnum.BattlerType.Actor,
                    timeLine = Random.Range(battler.parameter.spd / 2,
                        battler.parameter.spd)
                });
            });
            EnemyDataModel enemyData = EnemyDataModel.Instance;
            enemyData.Data.ForEach(x =>
            {
                _timelineData.Add(new TimelineSerializableData
                {
                    id = _timelineData.Count,
                    uniqId = x.uniqId,
                    battlerType = BattlerEnum.BattlerType.Enemy,
                    timeLine = Random.Range(x.parameter.spd / 2, x.parameter.spd)
                });
            });
            TimelineCalc();
            for (int i = 0; i < SCHEDULE_SIZE; i++)
            {
                TimeLineForward();
            }
            
            _timelineView = new TimelineView();
            _timelineView.Initialize(this);
            
        }

        /// <summary>
        /// タイムラインを次に送る
        /// </summary>
        public AsyncSubject<Unit> TimelineNext()
        {
            AsyncSubject<Unit> subject = new AsyncSubject<Unit>();
            TimelineCalc();
            _timelineSchedule.Remove(_timelineSchedule.First());
            var id = TimeLineForward();
            _timelineView.DepopSchedule();
            _timelineView.AddCard(BattleDictionary.GetTimelineById(_timelineData, id)).Subscribe(_ =>
            {
                subject.OnNext(Unit.Default);
                subject.OnCompleted();
            });
            return subject;
        }

        /// <summary>
        /// タイムラインをすすめる
        /// </summary>
        private int TimeLineForward()
        {
            bool timelineFlg = false;
            int id = 0;
            while (!timelineFlg)
            {
                foreach (var x in _timelineData)
                {
                    var battler = BattlerDictionary.GetBattlerByUniqId(x.uniqId);
                    //死んでない場合のみ
                    if (BattlerDictionary.IsDead(battler) == false)
                    {
                        int rate = _timelineSchedule.FindAll(i => i == x.id).Count + 1;
                        x.timeLine += (battler.parameter.spd + ((_spdAverage - battler.parameter.spd) / 2)) / rate;

                        if (x.timeLine > _timeline)
                        {
                            x.timeLine = 0;
                            _timelineSchedule.Add(x.id);
                            id = x.id;
                            timelineFlg = true;
                            break;
                        }
                    }
                }
            }
            return id;
        }

        /// <summary>
        /// タイムラインスケジュールから対象バトラーを削除する
        /// </summary>
        /// <returns></returns>
        public AsyncSubject<Unit> TimelineScheduleRemove()
        {
            AsyncSubject<Unit> subject = new AsyncSubject<Unit>();
            List<int> deleteIds = new List<int>();
            _timelineData.ForEach(x =>
            {
                if (BattlerDictionary.IsDeadByUniqId(x.uniqId))
                {
                    deleteIds.AddRange(_timelineSchedule.FindAll(id => id == x.id));
                    _timelineSchedule.RemoveAll(id => id == x.id);
                }
            });

            if (deleteIds.Count != 0)
            {
                _timelineView.RemoveSchedule(deleteIds).Subscribe(_ =>
                {
                    FillTimelines().Subscribe(__ =>
                    {
                        subject.OnNext(Unit.Default);
                        subject.OnCompleted();
                    });
                });
            }
            else
                ObservableUtils.AsyncSubjectTimeZeroCompleted(subject);
            
            return subject;
            
            /*
            AsyncSubject<Unit> subject = new AsyncSubject<Unit>();
            if (battlers.Count != 0)
            {
                List<int> deleteIds = new List<int>();
                battlers.ForEach(battler =>
                {
                    var timeline = _timelineData.Find(x => BattlerDictionary.GetBattlerForUniqId(x.uniqId) == battler);
                    if (timeline != null)
                    {
                        deleteIds.Add(timeline.id);
                        _timelineSchedule.RemoveAll(x => x == timeline.id);
                    }
                });

                if (deleteIds.Count != 0)
                {
                    _timelineView.RemoveSchedule(deleteIds).Subscribe(_ =>
                    {
                        FillTimelines().Subscribe(__ =>
                        {
                            subject.OnNext(Unit.Default);
                            subject.OnCompleted();
                        });
                    });
                }
                else
                    ObservableUtils.AsyncSubjectTimeZeroCompleted(subject);
            }
            else
                ObservableUtils.AsyncSubjectTimeZeroCompleted(subject);

            return subject;
            */
        }
        
        /// <summary>
        /// タイムラインを埋める
        /// </summary>
        /// <returns></returns>
        private AsyncSubject<Unit> FillTimelines()
        {
            AsyncSubject<Unit> subject = new AsyncSubject<Unit>();
            var size = SCHEDULE_SIZE - _timelineSchedule.Count();
            if (size != 0)
            {
                List<TimelineSerializableData> addLists = new List<TimelineSerializableData>();
                for (int i = 0; i < size; i++)
                {
                    var id = TimeLineForward();
                    addLists.Add(_timelineData[id]);
                }
                _timelineView.AddCards(addLists).Subscribe(_ =>
                {
                    subject.OnNext(Unit.Default);
                    subject.OnCompleted();
                });
            }
            else
            {
                ObservableUtils.AsyncSubjectTimeZeroCompleted(subject);
            }

            return subject;
        }

        /// <summary>
        /// タイムラインのゴールを計算する
        /// </summary>
        private void TimelineCalc()
        {
            _timeline = 0;
            _timelineData.ForEach(x =>
            {
                var battler = BattlerDictionary.GetBattlerByUniqId(x.uniqId);
                _timeline += battler.parameter.spd;
            });
            _spdAverage = _timeline / _timelineData.Count;
            _timeline = _spdAverage * TIMELINE_RATE;
        }

        public List<TimelineSerializableData> TimelineData
        {
            get => _timelineData;
            set => _timelineData = value;
        }

        public List<int> TimelineSchedule
        {
            get => _timelineSchedule;
            set => _timelineSchedule = value;
        }
    }
}